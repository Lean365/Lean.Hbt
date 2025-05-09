//===================================================================
// 项目名 : Lean.Hbt 
// 文件名 : IHbtWorkflowHistoryService.cs
// 创建者 : Lean365
// 创建时间: 2024-01-23 12:00
// 版本号 : V1.0.0
// 描述   : 工作流历史服务接口
//===================================================================

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Lean.Hbt.Application.Dtos.Workflow;
using Lean.Hbt.Common.Models;

namespace Lean.Hbt.Application.Services.Workflow
{
    /// <summary>
    /// 工作流历史服务接口
    /// </summary>
    /// <remarks>
    /// 创建者: Lean365
    /// 创建时间: 2024-01-23
    /// </remarks>
    public interface IHbtWorkflowHistoryService
    {
        /// <summary>
        /// 获取工作流历史分页列表
        /// </summary>
        /// <param name="query">查询条件</param>
        /// <returns>分页结果</returns>
        Task<HbtPagedResult<HbtWorkflowHistoryDto>> GetListAsync(HbtWorkflowHistoryQueryDto query);

        /// <summary>
        /// 获取工作流历史详情
        /// </summary>
        /// <param name="id">工作流历史ID</param>
        /// <returns>工作流历史详情</returns>
        Task<HbtWorkflowHistoryDto> GetByIdAsync(long id);

        /// <summary>
        /// 创建工作流历史
        /// </summary>
        /// <param name="input">创建信息</param>
        /// <returns>新创建的工作流历史ID</returns>
        Task<long> CreateAsync(HbtWorkflowHistoryCreateDto input);

        /// <summary>
        /// 更新工作流历史
        /// </summary>
        /// <param name="input">更新信息</param>
        /// <returns>是否成功</returns>
        Task<bool> UpdateAsync(HbtWorkflowHistoryUpdateDto input);

        /// <summary>
        /// 删除工作流历史
        /// </summary>
        /// <param name="id">工作流历史ID</param>
        /// <returns>是否成功</returns>
        Task<bool> DeleteAsync(long id);

        /// <summary>
        /// 批量删除工作流历史
        /// </summary>
        /// <param name="ids">工作流历史ID数组</param>
        /// <returns>是否成功</returns>
        Task<bool> BatchDeleteAsync(long[] ids);

        /// <summary>
        /// 导入工作流历史
        /// </summary>
        /// <param name="fileStream">Excel文件流</param>
        /// <param name="sheetName">工作表名称</param>
        /// <returns>导入的历史记录列表</returns>
        Task<List<HbtWorkflowHistoryDto>> ImportAsync(Stream fileStream, string sheetName = "Sheet1");

        /// <summary>
        /// 导出工作流历史
        /// </summary>
        /// <param name="data">要导出的数据集合</param>
        /// <param name="sheetName">工作表名称</param>
        /// <returns>Excel文件字节数组</returns>
        Task<(string fileName, byte[] content)> ExportAsync(IEnumerable<HbtWorkflowHistoryDto> data, string sheetName = "Sheet1");

        /// <summary>
        /// 获取工作流历史导入模板
        /// </summary>
        /// <param name="sheetName">工作表名称</param>
        /// <returns>Excel模板文件字节数组</returns>
        Task<(string fileName, byte[] content)> GetTemplateAsync(string sheetName = "Sheet1");

        /// <summary>
        /// 获取工作流实例的历史记录
        /// </summary>
        /// <param name="workflowInstanceId">工作流实例ID</param>
        /// <returns>历史记录列表</returns>
        Task<List<HbtWorkflowHistoryDto>> GetHistoriesByWorkflowInstanceAsync(long workflowInstanceId);

        /// <summary>
        /// 获取工作流节点的历史记录
        /// </summary>
        /// <param name="workflowNodeId">工作流节点ID</param>
        /// <returns>历史记录列表</returns>
        Task<List<HbtWorkflowHistoryDto>> GetHistoriesByWorkflowNodeAsync(long workflowNodeId);

        /// <summary>
        /// 获取用户的操作历史记录
        /// </summary>
        /// <param name="operatorId">操作人ID</param>
        /// <returns>历史记录列表</returns>
        Task<List<HbtWorkflowHistoryDto>> GetHistoriesByOperatorAsync(long operatorId);

        /// <summary>
        /// 清理历史记录
        /// </summary>
        /// <param name="days">保留天数</param>
        /// <returns>清理数量</returns>
        Task<int> CleanupHistoriesAsync(int days);
    }
} 