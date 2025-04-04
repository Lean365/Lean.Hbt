# 后端多语言开发规范

## 1. 全局异常处理实现

### 异常过滤器
```csharp
public class HbtGlobalExceptionFilter : IExceptionFilter
{
    private readonly IHbtI18nService _i18nService;

    public HbtGlobalExceptionFilter(IHbtI18nService i18nService)
    {
        _i18nService = i18nService;
    }

    public async Task OnExceptionAsync(ExceptionContext context)
    {
        // 获取当前语言
        var langCode = Thread.CurrentThread.CurrentUICulture.Name;
        
        if (context.Exception is BusinessException businessException)
        {
            // 业务异常转换为多语言
            var message = await _i18nService.GetTranslationAsync(businessException.MessageKey, langCode);
            context.Result = new JsonResult(new { 
                code = businessException.Code, 
                message = message 
            });
        }
        else if (context.Exception is ValidationException validationException)
        {
            // 验证异常转换为多语言
            var errors = await TranslateValidationErrors(validationException.Errors, langCode);
            context.Result = new JsonResult(new { 
                code = 400, 
                message = "validation.error", 
                errors = errors 
            });
        }
    }
}
```

## 2. 业务异常多语言实现

### 自定义异常基类
```csharp
public abstract class HbtException : Exception
{
    public string MessageKey { get; }
    public object[] MessageParams { get; }

    protected HbtException(string messageKey, params object[] messageParams)
    {
        MessageKey = messageKey;
        MessageParams = messageParams;
    }
}

public class BusinessException : HbtException
{
    public int Code { get; }

    public BusinessException(string messageKey, int code = 400, params object[] messageParams) 
        : base(messageKey, messageParams)
    {
        Code = code;
    }
}
```

### 使用示例
```csharp
// 抛出业务异常
throw new BusinessException("user.not.found", 404, userId);

// 对应的翻译文件
{
    "user.not.found": "用户 {0} 不存在"
}
```

## 3. 验证异常多语言实现

### FluentValidation配置
```csharp
public class UserValidator : AbstractValidator<UserDto>
{
    private readonly IHbtI18nService _i18nService;

    public UserValidator(IHbtI18nService i18nService)
    {
        _i18nService = i18nService;

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage(async _ => await _i18nService.GetTranslationAsync("validation.user.name.required"))
            .MaximumLength(50)
            .WithMessage(async _ => await _i18nService.GetTranslationAsync("validation.user.name.maxlength"));

        RuleFor(x => x.Email)
            .EmailAddress()
            .WithMessage(async _ => await _i18nService.GetTranslationAsync("validation.user.email.invalid"));
    }
}
```

## 4. 响应消息多语言实现

### 统一响应处理
```csharp
public class HbtResult<T>
{
    public int Code { get; set; }
    public string Message { get; set; }
    public T Data { get; set; }

    public static async Task<HbtResult<T>> SuccessAsync(
        IHbtI18nService i18nService,
        T data = default,
        string messageKey = "common.success")
    {
        return new HbtResult<T>
        {
            Code = 200,
            Message = await i18nService.GetTranslationAsync(messageKey),
            Data = data
        };
    }

    public static async Task<HbtResult<T>> ErrorAsync(
        IHbtI18nService i18nService,
        string messageKey,
        int code = 400)
    {
        return new HbtResult<T>
        {
            Code = code,
            Message = await i18nService.GetTranslationAsync(messageKey)
        };
    }
}
```

### 控制器基类
```csharp
public abstract class HbtBaseController : ControllerBase
{
    protected readonly IHbtI18nService _i18nService;

    protected HbtBaseController(IHbtI18nService i18nService)
    {
        _i18nService = i18nService;
    }

    protected async Task<IActionResult> SuccessAsync<T>(T data = default, string messageKey = "common.success")
    {
        return Ok(await HbtResult<T>.SuccessAsync(_i18nService, data, messageKey));
    }

    protected async Task<IActionResult> ErrorAsync(string messageKey, int code = 400)
    {
        return BadRequest(await HbtResult<object>.ErrorAsync(_i18nService, messageKey, code));
    }
}
```

## 5. 中间件中的语言切换

### 语言中间件
```csharp
public class HbtLanguageMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IHbtI18nService _i18nService;

    public HbtLanguageMiddleware(RequestDelegate next, IHbtI18nService i18nService)
    {
        _next = next;
        _i18nService = i18nService;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // 1. 从请求头获取语言
        var langCode = context.Request.Headers["Accept-Language"].FirstOrDefault();

        // 2. 从Cookie获取语言
        if (string.IsNullOrEmpty(langCode))
        {
            langCode = context.Request.Cookies["Language"];
        }

        // 3. 使用默认语言
        if (string.IsNullOrEmpty(langCode))
        {
            langCode = await _i18nService.GetDefaultLanguageCode();
        }

        // 设置当前线程的语言文化
        var culture = new CultureInfo(langCode);
        Thread.CurrentThread.CurrentUICulture = culture;
        Thread.CurrentThread.CurrentCulture = culture;

        await _next(context);
    }
}
```

## 6. 使用示例

### 在控制器中
```csharp
[ApiController]
[Route("api/[controller]")]
public class UserController : HbtBaseController
{
    private readonly IUserService _userService;

    public UserController(IHbtI18nService i18nService, IUserService userService) 
        : base(i18nService)
    {
        _userService = userService;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateUserDto dto)
    {
        try
        {
            var user = await _userService.CreateAsync(dto);
            return await SuccessAsync(user, "user.create.success");
        }
        catch (BusinessException ex)
        {
            return await ErrorAsync(ex.MessageKey, ex.Code);
        }
    }
}
```

### 在服务层中
```csharp
public class UserService : IUserService
{
    private readonly IHbtI18nService _i18nService;

    public async Task<UserDto> CreateAsync(CreateUserDto dto)
    {
        if (await IsUserNameExists(dto.UserName))
        {
            throw new BusinessException("user.name.duplicate");
        }

        // 创建用户逻辑...
    }
}
```
