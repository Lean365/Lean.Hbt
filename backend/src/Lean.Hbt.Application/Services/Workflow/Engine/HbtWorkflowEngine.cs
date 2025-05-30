#nullable enable

//===================================================================
// 项目名 : Lean.Hbt
// 文件名 : HbtWorkflowEngine.cs
// 创建者 : Lean365
// 创建时间: 2024-01-23 12:00
// 版本号 : V1.0.0
// 描述    : 工作流引擎实现
//===================================================================

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Lean.Hbt.Application.Dtos.Workflow;
using Lean.Hbt.Common.Enums;
using Lean.Hbt.Domain.Entities.Workflow;
using Lean.Hbt.Domain.Repositories;
using Mapster;
using Lean.Hbt.Application.Services.Workflow.Engine.Executors;
using Lean.Hbt.Application.Services.Workflow.Engine.Expressions;
using Lean.Hbt.Domain.Data;

namespace Lean.Hbt.Application.Services.Workflow.Engine
{
    /// <summary>
    /// 工作流引擎实现
    /// </summary>
    public class HbtWorkflowEngine : IHbtWorkflowEngine
    {
        private readonly IHbtDbContext _dbContext;
        private readonly IHbtRepository<HbtWorkflowInstance> _instanceRepository;
        private readonly IHbtRepository<HbtWorkflowNode> _nodeRepository;
        private readonly IHbtRepository<HbtWorkflowTransition> _transitionRepository;
        private readonly IHbtRepository<HbtWorkflowVariable> _variableRepository;
        private readonly IHbtRepository<HbtWorkflowDefinition> _definitionRepository;
        private readonly IEnumerable<IHbtWorkflowNodeExecutor> _nodeExecutors;
        private readonly IHbtRepository<HbtWorkflowParallelBranch> _parallelBranchRepository;
        private readonly IHbtWorkflowExpressionEngine _expressionEngine;

        /// <summary>
        /// 构造函数
        /// </summary>
        public HbtWorkflowEngine(
            IHbtDbContext dbContext,
            IHbtRepository<HbtWorkflowInstance> instanceRepository,
            IHbtRepository<HbtWorkflowNode> nodeRepository,
            IHbtRepository<HbtWorkflowTransition> transitionRepository,
            IHbtRepository<HbtWorkflowVariable> variableRepository,
            IHbtRepository<HbtWorkflowDefinition> definitionRepository,
            IEnumerable<IHbtWorkflowNodeExecutor> nodeExecutors,
            IHbtRepository<HbtWorkflowParallelBranch> parallelBranchRepository,
            IHbtWorkflowExpressionEngine expressionEngine)
        {
            _dbContext = dbContext;
            _instanceRepository = instanceRepository;
            _nodeRepository = nodeRepository;
            _transitionRepository = transitionRepository;
            _variableRepository = variableRepository;
            _definitionRepository = definitionRepository;
            _nodeExecutors = nodeExecutors;
            _parallelBranchRepository = parallelBranchRepository;
            _expressionEngine = expressionEngine;
        }

        /// <inheritdoc/>
        public async Task<long> StartAsync(long definitionId, string title, long initiatorId, string formData, Dictionary<string, object>? variables = null)
        {
            try
            {
                _dbContext.BeginTran();

                // 获取工作流定义
                var definition = await _definitionRepository.GetByIdAsync(definitionId);
                if (definition == null)
                {
                    throw new InvalidOperationException("工作流定义不存在");
                }

                // 获取开始节点
                var startNode = await _nodeRepository.GetFirstAsync(x => 
                    x.WorkflowDefinitionId == definitionId && 
                    x.NodeType == 1); // 1 表示开始节点

                if (startNode == null)
                {
                    throw new InvalidOperationException("工作流定义中未找到开始节点");
                }

                // 创建工作流实例
                var instance = new HbtWorkflowInstance
                {
                    WorkflowDefinitionId = definitionId,
                    WorkflowTitle = title,
                    InitiatorId = initiatorId,
                    CurrentNodeId = startNode.Id,
                    FormData = formData,
                    Status = 1, // 1 表示运行中
                    StartTime = DateTime.Now
                };

                await _instanceRepository.CreateAsync(instance);

                // 保存变量
                if (variables != null)
                {
                    await SaveVariablesAsync(instance.Id, variables);
                }

                // 执行开始节点
                var result = await ExecuteNodeAsync(instance.Id, startNode.Id);
                if (!result.Success)
                {
                    throw new InvalidOperationException($"执行开始节点失败: {result.ErrorMessage}");
                }

                _dbContext.CommitTran();
                return instance.Id;
            }
            catch (Exception)
            {
                _dbContext.RollbackTran();
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task SuspendAsync(long instanceId)
        {
            var instance = await _instanceRepository.GetByIdAsync(instanceId);
            if (instance == null)
            {
                throw new InvalidOperationException("工作流实例不存在");
            }

            if (instance.Status != 1) // 1 表示运行中
            {
                throw new InvalidOperationException("只有运行中的工作流实例才能暂停");
            }

            instance.Status = 3; // 3 表示已挂起
            await _instanceRepository.UpdateAsync(instance);
        }

        /// <inheritdoc/>
        public async Task ResumeAsync(long instanceId)
        {
            var instance = await _instanceRepository.GetByIdAsync(instanceId);
            if (instance == null)
            {
                throw new InvalidOperationException("工作流实例不存在");
            }

            if (instance.Status != 3) // 3 表示已挂起
            {
                throw new InvalidOperationException("只有已暂停的工作流实例才能恢复");
            }

            instance.Status = 1; // 1 表示运行中
            await _instanceRepository.UpdateAsync(instance);
        }

        /// <inheritdoc/>
        public async Task TerminateAsync(long instanceId, string reason)
        {
            var instance = await _instanceRepository.GetByIdAsync(instanceId);
            if (instance == null)
            {
                throw new InvalidOperationException("工作流实例不存在");
            }

            if (instance.Status != 1 && instance.Status != 3) // 1 表示运行中, 3 表示已挂起
            {
                throw new InvalidOperationException("只有运行中或已暂停的工作流实例才能终止");
            }

            instance.Status = 4; // 4 表示已终止
            instance.EndTime = DateTime.Now;
            instance.Remark = reason;
            await _instanceRepository.UpdateAsync(instance);
        }

        /// <inheritdoc/>
        public async Task<HbtWorkflowNodeResult> ExecuteNodeAsync(long instanceId, long nodeId, Dictionary<string, object>? variables = null)
        {
            // 获取实例
            var instance = await _instanceRepository.GetByIdAsync(instanceId);
            if (instance == null)
            {
                throw new InvalidOperationException("工作流实例不存在");
            }

            // 检查实例状态
            if (instance.Status != 1) // 1 表示运行中
            {
                throw new InvalidOperationException("工作流实例状态不正确");
            }

            // 获取节点
            var node = await _nodeRepository.GetByIdAsync(nodeId);
            if (node == null)
            {
                throw new InvalidOperationException("工作流节点不存在");
            }

            // 保存变量
            if (variables != null)
            {
                await SaveVariablesAsync(instanceId, variables, nodeId);
            }

            // 查找合适的执行器
            var executor = _nodeExecutors.FirstOrDefault(x => x.CanHandle(node.NodeType));
            if (executor == null)
            {
                throw new InvalidOperationException($"未找到节点类型 {node.NodeType} 的执行器");
            }

            // 执行节点
            var result = await executor.ExecuteAsync(instance, node, variables);

            // 如果执行成功且有输出变量，保存输出变量
            if (result.Success && result.OutputVariables != null)
            {
                await SaveVariablesAsync(instanceId, result.OutputVariables, nodeId);
            }

            return result;
        }

        /// <inheritdoc/>
        public async Task<HbtWorkflowTransitionResult> ExecuteTransitionAsync(long instanceId, long transitionId, Dictionary<string, object>? variables = null)
        {
            try
            {
                _dbContext.BeginTran();

                // 获取实例
                var instance = await _instanceRepository.GetByIdAsync(instanceId);
                if (instance == null)
                {
                    throw new InvalidOperationException("工作流实例不存在");
                }

                // 检查实例状态
                if (instance.Status != 1) // 1 表示运行中
                {
                    throw new InvalidOperationException("工作流实例状态不正确");
                }

                // 获取转换
                var transition = await _transitionRepository.GetByIdAsync(transitionId);
                if (transition == null)
                {
                    throw new InvalidOperationException("工作流转换不存在");
                }

                // 保存变量
                if (variables != null)
                {
                    await SaveVariablesAsync(instanceId, variables);
                }

                // 检查转换条件
                if (!string.IsNullOrEmpty(transition.Condition))
                {
                    var allVariables = await GetVariablesAsync(instanceId);
                    var conditionResult = await _expressionEngine.EvaluateAsync(transition.Condition, allVariables);
                    if (!(conditionResult is bool boolResult && boolResult))
                    {
                        throw new InvalidOperationException("转换条件不满足");
                    }
                }

                // 更新实例当前节点
                instance.CurrentNodeId = transition.TargetNodeId;
                await _instanceRepository.UpdateAsync(instance);

                // 执行目标节点
                var result = await ExecuteNodeAsync(instanceId, transition.TargetNodeId);
                if (!result.Success)
                {
                    throw new InvalidOperationException($"执行目标节点失败: {result.ErrorMessage}");
                }

                _dbContext.CommitTran();
                return new HbtWorkflowTransitionResult { Success = true };
            }
            catch (Exception)
            {
                _dbContext.RollbackTran();
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<HbtWorkflowInstanceStatusDto> GetStatusAsync(long instanceId)
        {
            var instance = await _instanceRepository.GetByIdAsync(instanceId);
            if (instance == null)
            {
                throw new InvalidOperationException("工作流实例不存在");
            }

            var currentNode = await _nodeRepository.GetByIdAsync(instance.CurrentNodeId);

            return new HbtWorkflowInstanceStatusDto
            {
                WorkflowInstanceId = instance.Id,
                Status = instance.Status,
                CurrentNodeId = instance.CurrentNodeId,
                CurrentNodeName = currentNode?.NodeName ?? string.Empty,
                AvailableOperations = GetAvailableOperations(instance),
                StatusDescription = instance.Status
            };
        }

        /// <inheritdoc/>
        public async Task<List<HbtWorkflowTransitionDto>> GetAvailableTransitionsAsync(long instanceId)
        {
            var instance = await _instanceRepository.GetByIdAsync(instanceId);
            if (instance == null)
            {
                throw new InvalidOperationException("工作流实例不存在");
            }

            if (instance.Status != 1)
            {
                return new List<HbtWorkflowTransitionDto>();
            }

            var transitions = await _transitionRepository.GetListAsync(x => x.SourceNodeId == instance.CurrentNodeId);
            return transitions.Adapt<List<HbtWorkflowTransitionDto>>();
        }

        /// <inheritdoc/>
        public async Task<Dictionary<string, object>> GetVariablesAsync(long instanceId, long? nodeId = null)
        {
            var variables = new Dictionary<string, object>();

            // 获取实例级变量
            var instanceVariables = await _variableRepository.GetListAsync(x =>
                x.WorkflowInstanceId == instanceId &&
                x.Scope == 1); // 1 表示全局范围

            foreach (var variable in instanceVariables)
            {
                variables[variable.VariableName] = variable.VariableValue;
            }

            // 获取节点级变量
            if (nodeId.HasValue)
            {
                var nodeVariables = await _variableRepository.GetListAsync(x =>
                    x.WorkflowInstanceId == instanceId &&
                    x.NodeId == nodeId &&
                    x.Scope == 2); // 2 表示节点范围

                foreach (var variable in nodeVariables)
                {
                    variables[variable.VariableName] = variable.VariableValue;
                }
            }

            return variables;
        }

        /// <inheritdoc/>
        public async Task SetVariablesAsync(long instanceId, Dictionary<string, object> variables, long? nodeId = null)
        {
            foreach (var kvp in variables)
            {
                var variable = new HbtWorkflowVariable
                {
                    WorkflowInstanceId = instanceId,
                    NodeId = nodeId,
                    VariableName = kvp.Key,
                    VariableValue = kvp.Value.ToString() ?? string.Empty,
                    Scope = nodeId.HasValue ? 2 : 1
                };

                await _variableRepository.CreateAsync(variable);
            }
        }

        private async Task SaveVariablesAsync(long instanceId, Dictionary<string, object> variables, long? nodeId = null)
        {
            foreach (var kvp in variables)
            {
                var variable = new HbtWorkflowVariable
                {
                    WorkflowInstanceId = instanceId,
                    NodeId = nodeId,
                    VariableName = kvp.Key,
                    VariableValue = kvp.Value.ToString() ?? string.Empty,
                    Scope = nodeId.HasValue ? 2 : 1
                };

                await _variableRepository.CreateAsync(variable);
            }
        }

        private List<string> GetAvailableOperations(HbtWorkflowInstance instance)
        {
            var operations = new List<string>();

            switch (instance.Status)
            {
                case 1: // 1 表示运行中
                    operations.Add("suspend");
                    operations.Add("terminate");
                    break;

                case 3: // 3 表示已挂起
                    operations.Add("resume");
                    operations.Add("terminate");
                    break;

                case 0: // 0 表示草稿
                    operations.Add("submit");
                    operations.Add("delete");
                    break;
            }

            return operations;
        }

        /// <summary>
        /// 更新工作流实例状态
        /// </summary>
        private async Task UpdateInstanceStatusAsync(long instanceId, int status)
        {
            var instance = await _instanceRepository.GetByIdAsync(instanceId);
            if (instance != null)
            {
                instance.Status = status;
                await _instanceRepository.UpdateAsync(instance);
            }
        }
    }
}