using Lean.Hbt.Domain.IServices.Caching;

namespace Lean.Hbt.WebApi.Middlewares
{
    /// <summary>
    /// CSRF防护中间件
    /// </summary>
    public class HbtCsrfMiddleware
    {
        private readonly RequestDelegate _next;

        /// <summary>
        /// 日志服务
        /// </summary>
        protected readonly IHbtLogger _logger;

        private readonly IHbtRedisCache _redisCache;
        private const string CSRF_HEADER = "X-CSRF-Token";
        private const string CSRF_COOKIE = "XSRF-TOKEN";
        private const string CSRF_CACHE_PREFIX = "csrf:token:";
        private const int TOKEN_EXPIRE_MINUTES = 30;

        public HbtCsrfMiddleware(
            RequestDelegate next,
            IHbtLogger logger,

            IHbtRedisCache redisCache)
        {
            _next = next;
            _logger = logger;
            _redisCache = redisCache;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value?.ToLowerInvariant() ?? "";
            var method = context.Request.Method;
            _logger.Info($"[CSRF] Processing request: {method} {path}");

            // 检查是否需要跳过CSRF验证
            var pathWithoutQuery = path.Split('?')[0];
            _logger.Info($"[CSRF] Path analysis - Full: {path}, Without query: {pathWithoutQuery}");

            // 检查是否是SignalR请求
            if (pathWithoutQuery.StartsWith("/signalr", StringComparison.OrdinalIgnoreCase))
            {
                _logger.Info($"[CSRF] Skipping CSRF check for SignalR path: {pathWithoutQuery}");
                await _next(context);
                return;
            }

            // 检查其他需要跳过的路径
            var shouldSkip = ShouldSkipCsrf(pathWithoutQuery);
            _logger.Info($"[CSRF] Should skip CSRF check: {shouldSkip}");

            if (shouldSkip)
            {
                _logger.Info($"[CSRF] Skipping CSRF check for path: {pathWithoutQuery}");
                await _next(context);
                return;
            }

            // 验证CSRF Token
            var requestToken = context.Request.Headers[CSRF_HEADER].ToString();
            var cookieToken = context.Request.Cookies[CSRF_COOKIE];

            _logger.Info($"[CSRF] Request Token: {requestToken}");
            _logger.Info($"[CSRF] Cookie Token: {cookieToken}");

            // 验证所有请求的Token
            // 首先检查请求头中是否存在Token
            if (string.IsNullOrEmpty(requestToken))
            {
                _logger.Warn("[CSRF] Missing token in request header");
                context.Response.StatusCode = 403;
                await context.Response.WriteAsJsonAsync(new { message = "请求头中缺少CSRF Token" });
                return;
            }

            // 如果Cookie中存在Token，则验证两者是否匹配
            if (!string.IsNullOrEmpty(cookieToken))
            {
                var cacheKey = $"{CSRF_CACHE_PREFIX}{cookieToken}";
                var cachedToken = await _redisCache.GetAsync<string>(cacheKey);

                if (!string.IsNullOrEmpty(cachedToken) && requestToken != cookieToken)
                {
                    _logger.Warn($"[CSRF] Token mismatch - Request: {requestToken}, Cookie: {cookieToken}");
                    context.Response.StatusCode = 403;
                    await context.Response.WriteAsJsonAsync(new { message = "CSRF Token验证失败" });
                    return;
                }

                // Token有效，刷新过期时间
                if (!string.IsNullOrEmpty(cachedToken))
                {
                    await _redisCache.SetAsync(cacheKey, cookieToken, TimeSpan.FromMinutes(TOKEN_EXPIRE_MINUTES));
                    await _next(context);
                    return;
                }
            }

            // 生成新Token的情况：
            // 1. 没有Cookie Token
            // 2. Token已过期
            var newToken = GenerateToken();
            await SetCsrfToken(context, newToken);

            // 如果是因为Cookie Token无效而生成了新Token，返回403让客户端使用新Token重试
            _logger.Warn("[CSRF] Invalid or expired cookie token");
            context.Response.StatusCode = 403;

            await _next(context);
        }

        private bool ShouldSkipCsrf(string path)
        {
            // 跳过的路径列表
            var skipPaths = new[]
            {
                "/api/hbtauth/login",
                "/api/hbtauth/logout",
                "/api/hbtauth/check-login",  // 添加check-login路径
                "/api/hbtlanguage/supported",
                "/api/hbtonlineuser/force-offline",
                "/swagger",
                "/_framework",
                "/_vs"
            };

            // 移除路径中的HTTP方法前缀（如 "POST "）
            var pathWithoutMethod = path.Split(' ').Last();

            foreach (var skipPath in skipPaths)
            {
                if (pathWithoutMethod.Equals(skipPath, StringComparison.OrdinalIgnoreCase))
                {
                    _logger.Info($"[CSRF] Path matches skip pattern: {skipPath}");
                    return true;
                }
            }

            _logger.Info($"[CSRF] Path does not match any skip patterns");
            return false;
        }

        private string GenerateToken()
        {
            var randomBytes = new byte[32];
            using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }
            return Convert.ToBase64String(randomBytes);
        }

        private async Task SetCsrfToken(HttpContext context, string token)
        {
            _logger.Info($"[CSRF] Setting CSRF token cookie: {token}");

            // 设置Cookie
            context.Response.Cookies.Append(CSRF_COOKIE, token, new CookieOptions
            {
                HttpOnly = false, // 允许JavaScript访问
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Path = "/",
                MaxAge = TimeSpan.FromMinutes(TOKEN_EXPIRE_MINUTES),
                Domain = null // 让浏览器自动设置域名
            });

            // 在响应头中也返回Token
            context.Response.Headers.Append(CSRF_HEADER, token);

            // 缓存Token
            var cacheKey = $"{CSRF_CACHE_PREFIX}{token}";
            _logger.Info($"[CSRF] 开始缓存Token: {cacheKey}");
            await _redisCache.SetAsync(cacheKey, token, TimeSpan.FromMinutes(TOKEN_EXPIRE_MINUTES));
            var result = await _redisCache.GetAsync<string>(cacheKey);
            _logger.Info($"[CSRF] Token缓存结果: {result}");

            _logger.Info($"[CSRF] Generated new CSRF token: {token}");
        }
    }
}