//===================================================================
// 项目名 : Lean.Hbt
// 文件名 : IHbtLoginLogService.cs
// 创建者 : Lean365
// 创建时间: 2024-01-20 16:30
// 版本号 : V0.0.1
// 描述   : 登录日志服务接口
//===================================================================

using System.Threading.Tasks;
using Lean.Hbt.Common.Models;
using Lean.Hbt.Application.Dtos.Audit;

namespace Lean.Hbt.Application.Services.Audit
{
    /// <summary>
    /// 登录日志服务接口
    /// </summary>
    /// <remarks>
    /// 创建者: Lean365
    /// 创建时间: 2024-01-20
    /// </remarks>
    public interface IHbtLoginLogService
    {
        /// <summary>
        /// 获取登录日志分页列表
        /// </summary>
        /// <param name="query">查询条件</param>
        /// <returns>分页结果</returns>
        Task<HbtPagedResult<HbtLoginLogDto>> GetListAsync(HbtLoginLogQueryDto query);

        /// <summary>
        /// 获取登录日志详情
        /// </summary>
        /// <param name="logId">日志ID</param>
        /// <returns>登录日志详情</returns>
        Task<HbtLoginLogDto> GetByIdAsync(long logId);

        /// <summary>
        /// 导出登录日志数据
        /// </summary>
        /// <param name="query">查询条件</param>
        /// <param name="sheetName">工作表名称</param>
        /// <returns>Excel文件字节数组</returns>
        Task<(string fileName, byte[] content)> ExportAsync(HbtLoginLogQueryDto query, string sheetName);

        /// <summary>
        /// 清空登录日志
        /// </summary>
        /// <returns>是否成功</returns>
        Task<bool> ClearAsync();

        /// <summary>
        /// 解锁用户
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>任务</returns>
        Task UnlockUserAsync(long userId);
    }
} 