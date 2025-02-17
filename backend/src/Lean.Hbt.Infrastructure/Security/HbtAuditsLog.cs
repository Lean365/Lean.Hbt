//===================================================================
// 项目名 : Lean.Hbt
// 文件名 : HbtAuditLog.cs
// 创建者 : Lean365
// 创建时间: 2024-01-16 10:00
// 版本号 : V.0.0.1
// 描述    : 审计日志实现
//===================================================================

using Lean.Hbt.Domain.Entities.Audit;
using Lean.Hbt.Domain.IServices;
using Lean.Hbt.Infrastructure.Data.Contexts;

namespace Lean.Hbt.Infrastructure.Security
{
    /// <summary>
    /// 审计日志实现
    /// </summary>
    public class HbtAuditsLog : IHbtAuditsLog
    {
        private readonly IHbtLogger _logger;
        private readonly HbtDbContext _context;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="context"></param>
        public HbtAuditsLog(IHbtLogger logger, HbtDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        /// <summary>
        /// 记录操作日志
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="userName"></param>
        /// <param name="module"></param>
        /// <param name="operation"></param>
        /// <param name="method"></param>
        /// <param name="parameters"></param>
        /// <param name="result"></param>
        /// <param name="elapsed"></param>
        /// <returns></returns>
        public async Task LogOperationAsync(long userId, string userName, string module, string operation, string method, string parameters, string result, long elapsed)
        {
            var log = new Domain.Entities.Audit.HbtAuditLog
            {
                UserId = userId,
                UserName = userName,
                Module = module,
                Operation = operation,
                Method = method,
                Parameters = parameters,
                Result = result,
                Elapsed = elapsed,
                IpAddress = GetClientIpAddress(),
                UserAgent = GetUserAgent(),
                CreateTime = DateTime.Now
            };

            try
            {
                var repo = _context.Client.GetSimpleClient<Domain.Entities.Audit.HbtAuditLog>();
                await repo.InsertAsync(log);
                _logger.Info($"记录操作日志成功: {userName} 在 {module} 模块执行了 {operation} 操作, 耗时 {elapsed}ms");
            }
            catch (Exception ex)
            {
                _logger.Error("记录操作日志失败", ex);
                throw;
            }
        }

        /// <summary>
        /// 记录登录日志
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="userName"></param>
        /// <param name="ipAddress"></param>
        /// <param name="userAgent"></param>
        /// <param name="result"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task LogLoginAsync(long userId, string userName, string ipAddress, string userAgent, bool result, string message)
        {
            var log = new HbtLoginLog
            {
                UserId = userId,
                UserName = userName,
                IpAddress = ipAddress,
                UserAgent = userAgent,
                Success = result ? 1 : 0,
                Message = message,
                CreateTime = DateTime.Now
            };

            try
            {
                var repo = _context.Client.GetSimpleClient<HbtLoginLog>();
                await repo.InsertAsync(log);
                _logger.Info($"记录登录日志成功: {userName} 登录{(result ? "成功" : "失败")} - {message}");
            }
            catch (Exception ex)
            {
                _logger.Error("记录登录日志失败", ex);
                throw;
            }
        }

        /// <summary>
        /// 记录异常日志
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="userName"></param>
        /// <param name="method"></param>
        /// <param name="parameters"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        public async Task LogExceptionAsync(long userId, string userName, string method, string parameters, Exception exception)
        {
            var log = new HbtExceptionLog
            {
                UserId = userId,
                UserName = userName,
                Method = method,
                Parameters = parameters,
                ExceptionType = exception.GetType().FullName ?? "Unknown",
                ExceptionMessage = exception.Message ?? "No message",
                StackTrace = exception.StackTrace ?? "No stack trace",
                IpAddress = GetClientIpAddress(),
                UserAgent = GetUserAgent(),
                CreateTime = DateTime.Now
            };

            try
            {
                var repo = _context.Client.GetSimpleClient<HbtExceptionLog>();
                await repo.InsertAsync(log);
                _logger.Error($"记录异常日志成功: {userName} 在执行 {method} 时发生异常", exception);
            }
            catch (Exception ex)
            {
                _logger.Error("记录异常日志失败", ex);
                throw;
            }
        }

        /// <summary>
        /// 获取客户端IP地址
        /// </summary>
        /// <returns></returns>
        private string GetClientIpAddress()
        {
            // TODO: 从HttpContext获取客户端IP地址
            return "127.0.0.1";
        }

        /// <summary>
        /// 获取客户端UserAgent
        /// </summary>
        /// <returns></returns>
        private string GetUserAgent()
        {
            // TODO: 从HttpContext获取用户代理
            return "Unknown";
        }

        /// <summary>
        /// 记录日志
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        public void Error(string message, Exception ex)
        {
            _logger.Error(message, ex);
        }

        /// <summary>
        /// 记录日志
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        public void Fatal(string message, Exception ex)
        {
            _logger.Fatal(message, ex);
        }
    }
}