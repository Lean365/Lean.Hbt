//===================================================================
// 项目名 : Lean.Hbt
// 文件名 : HbtWorkflowHistoryDto.cs
// 创建者 : Lean365
// 创建时间: 2024-01-23 12:00
// 版本号 : V1.0.0
// 描述    : 工作流历史数据传输对象
//===================================================================

namespace Lean.Hbt.Application.Dtos.Workflow
{
    /// <summary>
    /// 工作流历史基础DTO
    /// </summary>
    /// <remarks>
    /// 创建者: Lean365
    /// 创建时间: 2024-01-23
    /// </remarks>
    public class HbtWorkflowHistoryDto
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public HbtWorkflowHistoryDto()
        {
            WorkflowHistoryId = 0;
            WorkflowInstanceId = 0;
            WorkflowNodeId = 0;
            OperationType = 0;
            OperatorId = 0;
            OperatorName = string.Empty;
            OperationResult = 0;
            OperationComment = string.Empty;
            Remark = string.Empty;
            CreateBy = string.Empty;
            UpdateBy = string.Empty;
        }

        /// <summary>
        /// 历史ID
        /// </summary>
        [AdaptMember("Id")]
        public long WorkflowHistoryId { get; set; }

        /// <summary>
        /// 工作流实例ID
        /// </summary>
        public long WorkflowInstanceId { get; set; }

        /// <summary>
        /// 工作流节点ID
        /// </summary>
        public long WorkflowNodeId { get; set; }

        /// <summary>
        /// 操作类型
        /// </summary>
        public int OperationType { get; set; }

        /// <summary>
        /// 操作人ID
        /// </summary>
        public long OperatorId { get; set; }

        /// <summary>
        /// 操作人名称
        /// </summary>
        public string OperatorName { get; set; }

        /// <summary>
        /// 操作结果
        /// </summary>
        public int OperationResult { get; set; }

        /// <summary>
        /// 操作意见
        /// </summary>
        public string OperationComment { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string? Remark { get; set; }

        /// <summary>
        /// 创建者
        /// </summary>
        public string CreateBy { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 修改者
        /// </summary>
        public string UpdateBy { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime? UpdateTime { get; set; }
    }

    /// <summary>
    /// 工作流历史查询DTO
    /// </summary>
    /// <remarks>
    /// 创建者: Lean365
    /// 创建时间: 2024-01-23
    /// </remarks>
    public class HbtWorkflowHistoryQueryDto : HbtPagedQuery
    {
        /// <summary>
        /// 工作流实例ID
        /// </summary>
        public long? WorkflowInstanceId { get; set; }

        /// <summary>
        /// 工作流节点ID
        /// </summary>
        public long? WorkflowNodeId { get; set; }

        /// <summary>
        /// 操作类型
        /// </summary>
        public int? OperationType { get; set; }

        /// <summary>
        /// 操作人ID
        /// </summary>
        public long? OperatorId { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? StartTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndTime { get; set; }
    }

    /// <summary>
    /// 工作流历史创建DTO
    /// </summary>
    /// <remarks>
    /// 创建者: Lean365
    /// 创建时间: 2024-01-23
    /// </remarks>
    public class HbtWorkflowHistoryCreateDto
    {
        /// <summary>
        /// 工作流实例ID
        /// </summary>
        public long WorkflowInstanceId { get; set; }

        /// <summary>
        /// 工作流节点ID
        /// </summary>
        public long WorkflowNodeId { get; set; }

        /// <summary>
        /// 操作类型
        /// </summary>
        public int OperationType { get; set; }

        /// <summary>
        /// 操作人ID
        /// </summary>
        public long OperatorId { get; set; }

        /// <summary>
        /// 操作结果
        /// </summary>
        public string? OperationResult { get; set; }

        /// <summary>
        /// 操作意见
        /// </summary>
        public string? OperationComment { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string? Remark { get; set; }
    }

    /// <summary>
    /// 工作流历史更新DTO
    /// </summary>
    /// <remarks>
    /// 创建者: Lean365
    /// 创建时间: 2024-01-23
    /// </remarks>
    public class HbtWorkflowHistoryUpdateDto
    {
        /// <summary>
        /// 历史ID
        /// </summary>
        [AdaptMember("Id")]
        public long WorkflowHistoryId { get; set; }

        /// <summary>
        /// 操作结果
        /// </summary>
        public string? OperationResult { get; set; }

        /// <summary>
        /// 操作意见
        /// </summary>
        public string? OperationComment { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string? Remark { get; set; }
    }

    /// <summary>
    /// 工作流历史导入DTO
    /// </summary>
    /// <remarks>
    /// 创建者: Lean365
    /// 创建时间: 2024-01-23
    /// </remarks>
    public class HbtWorkflowHistoryImportDto
    {
        /// <summary>
        /// 工作流实例ID
        /// </summary>
        public long WorkflowInstanceId { get; set; }

        /// <summary>
        /// 工作流节点ID
        /// </summary>
        public long WorkflowNodeId { get; set; }

        /// <summary>
        /// 操作类型
        /// </summary>
        public string? OperationType { get; set; }

        /// <summary>
        /// 操作人ID
        /// </summary>
        public long OperatorId { get; set; }

        /// <summary>
        /// 操作结果
        /// </summary>
        public string? OperationResult { get; set; }

        /// <summary>
        /// 操作意见
        /// </summary>
        public string? OperationComment { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string? Remark { get; set; }
    }

    /// <summary>
    /// 工作流历史导出DTO
    /// </summary>
    /// <remarks>
    /// 创建者: Lean365
    /// 创建时间: 2024-01-23
    /// </remarks>
    public class HbtWorkflowHistoryExportDto
    {
        /// <summary>
        /// 工作流实例ID
        /// </summary>
        public long WorkflowInstanceId { get; set; }

        /// <summary>
        /// 工作流节点ID
        /// </summary>
        public long WorkflowNodeId { get; set; }

        /// <summary>
        /// 操作类型
        /// </summary>
        public string? OperationType { get; set; }

        /// <summary>
        /// 操作人ID
        /// </summary>
        public long OperatorId { get; set; }

        /// <summary>
        /// 操作人名称
        /// </summary>
        public string? OperatorName { get; set; }

        /// <summary>
        /// 操作结果
        /// </summary>
        public string? OperationResult { get; set; }

        /// <summary>
        /// 操作意见
        /// </summary>
        public string? OperationComment { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string? Remark { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }

    /// <summary>
    /// 工作流历史导入模板DTO
    /// </summary>
    /// <remarks>
    /// 创建者: Lean365
    /// 创建时间: 2024-01-23
    /// </remarks>
    public class HbtWorkflowHistoryTemplateDto
    {
        /// <summary>
        /// 操作类型
        /// </summary>
        public string? OperationType { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string? Remark { get; set; }
    }
}