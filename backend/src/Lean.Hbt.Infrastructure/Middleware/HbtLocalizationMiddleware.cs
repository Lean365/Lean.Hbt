//===================================================================
// 项目名 : Lean.Hbt
// 文件名 : HbtLocalizationMiddleware.cs
// 创建者 : Lean365
// 创建时间: 2024-01-22 16:30
// 版本号 : V0.0.1
// 描述   : 本地化中间件
//===================================================================

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Lean.Hbt.Domain.IServices.Admin;
using Lean.Hbt.Domain.IServices;

namespace Lean.Hbt.Infrastructure.Middleware;

/// <summary>
/// 本地化中间件
/// </summary>
public class HbtLocalizationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<HbtLocalizationMiddleware> _logger;
    private const string LANGUAGE_CACHE_KEY = "CurrentLanguage";
    private const string DEFAULT_LANGUAGE = "zh-CN";

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="next">请求委托</param>
    /// <param name="logger">日志服务</param>
    public HbtLocalizationMiddleware(RequestDelegate next, ILogger<HbtLocalizationMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>
    /// 处理请求
    /// </summary>
    /// <param name="context">HTTP上下文</param>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            // 检查是否需要设置语言
            if (!ShouldSetLanguage(context))
            {
                await _next(context);
                return;
            }

            // 从请求作用域中获取本地化服务
            var localizationService = context.RequestServices.GetRequiredService<IHbtLocalizationService>();
            
            // 获取语言设置
            var language = GetLanguage(context);
            
            try
            {
                // 设置语言
                localizationService.SetLanguage(language);
                
                // 缓存语言设置
                context.Items[LANGUAGE_CACHE_KEY] = language;
                
                // 更新Cookie
                UpdateLanguageCookie(context, language);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "设置语言失败: {Language}", language);
                // 使用默认语言
                localizationService.SetLanguage(DEFAULT_LANGUAGE);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "本地化中间件执行失败");
        }
        
        await _next(context);
    }

    /// <summary>
    /// 检查是否需要设置语言
    /// </summary>
    private bool ShouldSetLanguage(HttpContext context)
    {
        // 1. 如果是静态文件请求，不需要设置语言
        if (context.Request.Path.StartsWithSegments("/static") ||
            context.Request.Path.StartsWithSegments("/favicon.ico"))
        {
            return false;
        }

        // 2. 如果已经设置过语言，不需要重复设置
        if (context.Items.ContainsKey(LANGUAGE_CACHE_KEY))
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// 获取语言设置
    /// </summary>
    private string GetLanguage(HttpContext context)
    {
        // 1. 从请求头获取语言
        var langHeader = context.Request.Headers["Accept-Language"].ToString();
        if (!string.IsNullOrEmpty(langHeader))
        {
            return langHeader.Split(',')[0];
        }

        // 2. 从Cookie获取语言
        if (context.Request.Cookies.TryGetValue("lang", out var langCookie))
        {
            return langCookie;
        }

        // 3. 返回默认语言
        return DEFAULT_LANGUAGE;
    }

    /// <summary>
    /// 更新语言Cookie
    /// </summary>
    private void UpdateLanguageCookie(HttpContext context, string language)
    {
        // 如果Cookie不存在或者值不同，则更新Cookie
        if (!context.Request.Cookies.TryGetValue("lang", out var currentLang) || 
            currentLang != language)
        {
            context.Response.Cookies.Append("lang", language, new CookieOptions
            {
                Expires = DateTimeOffset.Now.AddYears(1),
                HttpOnly = true,
                Secure = context.Request.IsHttps,
                SameSite = SameSiteMode.Lax
            });
        }
    }
} 