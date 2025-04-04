//===================================================================
// 项目名 : Lean.Hbt
// 文件名 : HbtWorkflowVariableService.cs
// 创建者 : Lean365
// 创建时间: 2024-01-23 12:00
// 版本号 : V1.0.0
// 描述   : 工作流变量服务实现类
//===================================================================

#nullable enable

using Lean.Hbt.Application.Dtos.Workflow;
using Lean.Hbt.Common.Exceptions;
using Lean.Hbt.Common.Helpers;
using Lean.Hbt.Domain.Entities.Workflow;
using Lean.Hbt.Domain.IServices.Admin;
using Lean.Hbt.Domain.Repositories;
using Lean.Hbt.Domain.Utils;
using SqlSugar;

namespace Lean.Hbt.Application.Services.Workflow
{
    /// <summary>
    /// 工作流变量服务实现类
    /// </summary>
    /// <remarks>
    /// 创建者: Lean365
    /// 创建时间: 2024-01-23
    /// 功能说明:
    /// 1. 提供工作流变量的增删改查服务
    /// 2. 支持工作流变量的导入导出功能
    /// 3. 实现工作流变量的作用域管理
    /// 4. 提供变量值的获取和设置功能
    /// </remarks>
    public class HbtWorkflowVariableService : IHbtWorkflowVariableService
    {
        private readonly IHbtRepository<HbtWorkflowVariable> _variableRepository;
        private readonly IHbtLogger _logger;
        private readonly IHbtLocalizationService _localization;

        static HbtWorkflowVariableService()
        {
            // 配置Mapster映射规则
            TypeAdapterConfig<HbtWorkflowVariable, HbtWorkflowVariableDto>.NewConfig()
                .Map(dest => dest.WorkflowNodeId, src => src.NodeId)
                .Map(dest => dest.VariableScope, src => src.Scope);

            TypeAdapterConfig<HbtWorkflowVariable, HbtWorkflowVariableExportDto>.NewConfig()
                .Map(dest => dest.VariableTypeName, src => src.VariableType)
                .Map(dest => dest.VariableScopeName, src => src.Scope.ToString());

            TypeAdapterConfig<HbtWorkflowVariableCreateDto, HbtWorkflowVariable>.NewConfig()
                .Map(dest => dest.NodeId, src => src.WorkflowNodeId)
                .Map(dest => dest.Scope, src => src.VariableScope);

            TypeAdapterConfig<HbtWorkflowVariableUpdateDto, HbtWorkflowVariable>.NewConfig()
                .Ignore(dest => dest.WorkflowInstanceId)
                .Ignore(dest => dest.NodeId)
                .Ignore(dest => dest.VariableName)
                .Ignore(dest => dest.VariableType)
                .Ignore(dest => dest.Scope);
        }

        /// <summary>
        /// 构造函数，注入所需依赖
        /// </summary>
        /// <param name="variableRepository">工作流变量仓储接口</param>
        /// <param name="logger">日志服务</param>
        /// <param name="localization">本地化服务</param>
        public HbtWorkflowVariableService(
            IHbtRepository<HbtWorkflowVariable> variableRepository,
            IHbtLogger logger,
            IHbtLocalizationService localization)
        {
            _variableRepository = variableRepository;
            _logger = logger;
            _localization = localization;
        }

        /// <summary>
        /// 获取工作流变量分页列表
        /// </summary>
        /// <param name="query">查询条件，包含：
        /// 1. WorkflowInstanceId - 工作流实例ID
        /// 2. WorkflowNodeId - 工作流节点ID
        /// 3. VariableName - 变量名称
        /// 4. VariableType - 变量类型
        /// 5. VariableScope - 变量作用域
        /// 6. PageIndex - 页码
        /// 7. PageSize - 每页记录数</param>
        /// <returns>返回分页后的工作流变量列表</returns>
        public async Task<HbtPagedResult<HbtWorkflowVariableDto>> GetListAsync(HbtWorkflowVariableQueryDto query)
        {
            var exp = Expressionable.Create<HbtWorkflowVariable>();

            if (query?.WorkflowInstanceId.HasValue == true)
                exp = exp.And(x => x.WorkflowInstanceId == query.WorkflowInstanceId.Value);

            if (query?.WorkflowNodeId.HasValue == true)
                exp = exp.And(x => x.NodeId == query.WorkflowNodeId.Value);

            if (!string.IsNullOrEmpty(query?.VariableName))
                exp = exp.And(x => x.VariableName.Contains(query.VariableName));

            if (query?.VariableType.HasValue == true)
                exp = exp.And(x => x.VariableType == query.VariableType.Value.ToString());

            if (query?.VariableScope.HasValue == true)
                exp = exp.And(x => x.Scope == query.VariableScope.Value);

            var result = await _variableRepository.GetPagedListAsync(
                exp.ToExpression(),
                query?.PageIndex ?? 1,
                query?.PageSize ?? 10,
                x => x.Id,
                OrderByType.Desc);

            return new HbtPagedResult<HbtWorkflowVariableDto>
            {
                TotalNum = result.TotalNum,
                PageIndex = query?.PageIndex ?? 1,
                PageSize = query?.PageSize ?? 10,
                Rows = result.Rows.Adapt<List<HbtWorkflowVariableDto>>()
            };
        }

        /// <summary>
        /// 根据ID获取工作流变量详情
        /// </summary>
        /// <param name="id">工作流变量ID</param>
        /// <returns>工作流变量详情DTO</returns>
        /// <exception cref="HbtException">当工作流变量不存在时抛出异常</exception>
        public async Task<HbtWorkflowVariableDto> GetByIdAsync(long id)
        {
            var variable = await _variableRepository.GetByIdAsync(id);
            if (variable == null)
                throw new HbtException(_localization.L("WorkflowVariable.NotFound"));

            return variable.Adapt<HbtWorkflowVariableDto>();
        }

        /// <summary>
        /// 创建新的工作流变量
        /// </summary>
        /// <param name="input">工作流变量创建DTO，包含变量的基本信息</param>
        /// <returns>新创建的工作流变量ID</returns>
        /// <exception cref="ArgumentNullException">当输入参数为空时抛出异常</exception>
        /// <exception cref="HbtException">当工作流变量创建失败时抛出异常</exception>
        public async Task<long> CreateAsync(HbtWorkflowVariableCreateDto input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            // 检查变量名称是否已存在
            await HbtValidateUtils.ValidateFieldExistsAsync(_variableRepository, "VariableName", input.VariableName);

            var variable = input.Adapt<HbtWorkflowVariable>();

            var result = await _variableRepository.CreateAsync(variable);
            if (result <= 0)
                throw new HbtException(_localization.L("WorkflowVariable.Create.Failed"));

            _logger.Info(_localization.L("WorkflowVariable.Created.Success", variable.Id));
            return variable.Id;
        }

        /// <summary>
        /// 更新工作流变量信息
        /// </summary>
        /// <param name="input">工作流变量更新DTO，包含需要更新的变量信息</param>
        /// <returns>更新是否成功</returns>
        /// <exception cref="ArgumentNullException">当输入参数为空时抛出异常</exception>
        /// <exception cref="HbtException">当工作流变量不存在或更新失败时抛出异常</exception>
        public async Task<bool> UpdateAsync(HbtWorkflowVariableUpdateDto input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            var variable = await _variableRepository.GetByIdAsync(input.WorkflowVariableId);
            if (variable == null)
                throw new HbtException(_localization.L("WorkflowVariable.NotFound"));

            input.Adapt(variable);
            var result = await _variableRepository.UpdateAsync(variable);
            if (result <= 0)
                throw new HbtException(_localization.L("WorkflowVariable.Update.Failed"));

            _logger.Info(_localization.L("WorkflowVariable.Updated.Success", variable.Id));
            return true;
        }

        /// <summary>
        /// 删除指定工作流变量
        /// </summary>
        /// <param name="id">要删除的工作流变量ID</param>
        /// <returns>删除是否成功</returns>
        /// <exception cref="HbtException">当工作流变量不存在或删除失败时抛出异常</exception>
        public async Task<bool> DeleteAsync(long id)
        {
            var variable = await _variableRepository.GetByIdAsync(id);
            if (variable == null)
                throw new HbtException(_localization.L("WorkflowVariable.NotFound"));

            var result = await _variableRepository.DeleteAsync(variable);
            if (result <= 0)
                throw new HbtException(_localization.L("WorkflowVariable.Delete.Failed"));

            _logger.Info(_localization.L("WorkflowVariable.Deleted.Success", id));
            return true;
        }

        /// <summary>
        /// 批量删除工作流变量
        /// </summary>
        /// <param name="ids">要删除的工作流变量ID数组</param>
        /// <returns>删除是否成功</returns>
        /// <exception cref="ArgumentNullException">当输入ID数组为空时抛出异常</exception>
        /// <exception cref="HbtException">当批量删除失败时抛出异常</exception>
        public async Task<bool> BatchDeleteAsync(long[] ids)
        {
            if (ids == null || ids.Length == 0)
                throw new ArgumentNullException(nameof(ids));

            var exp = Expressionable.Create<HbtWorkflowVariable>();
            exp = exp.And(x => ids.Contains(x.Id));

            var result = await _variableRepository.DeleteAsync(exp.ToExpression());
            if (result <= 0)
                throw new HbtException(_localization.L("WorkflowVariable.BatchDelete.Failed"));

            _logger.Info(_localization.L("WorkflowVariable.BatchDeleted.Success", string.Join(",", ids)));
            return true;
        }

        /// <summary>
        /// 导出工作流变量(单个Sheet)
        /// </summary>
        public async Task<byte[]> ExportAsync(IEnumerable<HbtWorkflowVariableDto> data, string sheetName = "Sheet1")
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            var exportData = data.Adapt<List<HbtWorkflowVariableExportDto>>();
            return await HbtExcelHelper.ExportAsync(exportData, sheetName);
        }

        /// <summary>
        /// 导入工作流变量(单个Sheet)
        /// </summary>
        public async Task<List<HbtWorkflowVariableDto>> ImportAsync(Stream fileStream, string sheetName = "Sheet1")
        {
            if (fileStream == null)
                throw new ArgumentNullException(nameof(fileStream));

            var importData = await HbtExcelHelper.ImportAsync<HbtWorkflowVariableImportDto>(fileStream, sheetName);
            var variables = importData.Adapt<List<HbtWorkflowVariable>>();

            foreach (var variable in variables)
            {
                await _variableRepository.CreateAsync(variable);
            }

            return variables.Adapt<List<HbtWorkflowVariableDto>>();
        }

        /// <summary>
        /// 导出工作流变量(多个Sheet)
        /// </summary>
        public async Task<byte[]> ExportMultiSheetAsync(Dictionary<string, IEnumerable<HbtWorkflowVariableDto>> sheets)
        {
            if (sheets == null)
                throw new ArgumentNullException(nameof(sheets));

            var exportSheets = new Dictionary<string, IEnumerable<HbtWorkflowVariableExportDto>>();
            foreach (var sheet in sheets)
            {
                exportSheets[sheet.Key] = sheet.Value.Adapt<List<HbtWorkflowVariableExportDto>>();
            }

            return await HbtExcelHelper.ExportMultiSheetAsync(exportSheets);
        }

        /// <summary>
        /// 导入工作流变量(多个Sheet)
        /// </summary>
        public async Task<Dictionary<string, List<HbtWorkflowVariableDto>>> ImportMultiSheetAsync(Stream fileStream)
        {
            if (fileStream == null)
                throw new ArgumentNullException(nameof(fileStream));

            var importData = await HbtExcelHelper.ImportMultiSheetAsync<HbtWorkflowVariableImportDto>(fileStream);
            var result = new Dictionary<string, List<HbtWorkflowVariableDto>>();

            foreach (var sheet in importData)
            {
                var variables = sheet.Value.Adapt<List<HbtWorkflowVariable>>();
                foreach (var variable in variables)
                {
                    await _variableRepository.CreateAsync(variable);
                }
                result[sheet.Key] = variables.Adapt<List<HbtWorkflowVariableDto>>();
            }

            return result;
        }

        /// <summary>
        /// 获取工作流变量导入模板
        /// </summary>
        /// <param name="sheetName">工作表名称</param>
        /// <returns>Excel模板文件字节数组</returns>
        public async Task<byte[]> GetTemplateAsync(string sheetName = "Sheet1")
        {
            var template = new List<HbtWorkflowVariableExportDto>();
            return await HbtExcelHelper.ExportAsync(template, sheetName);
        }

        /// <summary>
        /// 获取工作流实例的所有变量
        /// </summary>
        /// <param name="workflowInstanceId">工作流实例ID</param>
        /// <returns>变量列表</returns>
        public async Task<List<HbtWorkflowVariableDto>> GetVariablesByWorkflowInstanceAsync(long workflowInstanceId)
        {
            var variables = await _variableRepository.GetListAsync(x => x.WorkflowInstanceId == workflowInstanceId);
            return variables.Adapt<List<HbtWorkflowVariableDto>>();
        }

        /// <summary>
        /// 获取工作流节点的所有变量
        /// </summary>
        /// <param name="workflowNodeId">工作流节点ID</param>
        /// <returns>变量列表</returns>
        public async Task<List<HbtWorkflowVariableDto>> GetVariablesByWorkflowNodeAsync(long workflowNodeId)
        {
            var variables = await _variableRepository.GetListAsync(x => x.NodeId == workflowNodeId);
            return variables.Adapt<List<HbtWorkflowVariableDto>>();
        }

        /// <summary>
        /// 获取工作流变量值
        /// </summary>
        /// <param name="workflowInstanceId">工作流实例ID</param>
        /// <param name="variableName">变量名称</param>
        /// <returns>变量值</returns>
        /// <exception cref="HbtException">当变量不存在时抛出异常</exception>
        public async Task<string> GetVariableValueAsync(long workflowInstanceId, string variableName)
        {
            var variable = await _variableRepository.SqlSugarClient.Queryable<HbtWorkflowVariable>()
                .FirstAsync(x => x.WorkflowInstanceId == workflowInstanceId &&
                               x.VariableName == variableName);

            if (variable == null)
                throw new HbtException(_localization.L("WorkflowVariable.NotFound"));

            return variable.VariableValue;
        }

        /// <summary>
        /// 设置工作流变量值
        /// </summary>
        /// <param name="workflowInstanceId">工作流实例ID</param>
        /// <param name="variableName">变量名称</param>
        /// <param name="variableValue">变量值</param>
        /// <returns>是否成功</returns>
        /// <exception cref="HbtException">当变量不存在或更新失败时抛出异常</exception>
        public async Task<bool> SetVariableValueAsync(long workflowInstanceId, string variableName, string variableValue)
        {
            var variable = await _variableRepository.SqlSugarClient.Queryable<HbtWorkflowVariable>()
                .FirstAsync(x => x.WorkflowInstanceId == workflowInstanceId &&
                               x.VariableName == variableName);

            if (variable == null)
                throw new HbtException(_localization.L("WorkflowVariable.NotFound"));

            variable.VariableValue = variableValue;

            var result = await _variableRepository.UpdateAsync(variable);
            if (result <= 0)
                throw new HbtException(_localization.L("WorkflowVariable.UpdateValue.Failed"));

            _logger.Info(_localization.L("WorkflowVariable.UpdatedValue.Success", variable.Id));
            return true;
        }
    }
}