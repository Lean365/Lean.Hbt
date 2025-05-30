import request from '@/utils/request'
import type { HbtApiResponse } from '@/types/common'
import type { 
  HbtWorkflowHistoryQuery, 
  HbtWorkflowHistory,
  HbtWorkflowHistoryCreate,
  HbtWorkflowHistoryUpdate,
  HbtWorkflowHistoryPagedResult
} from '@/types/workflow/workflowHistory'

// 获取工作流历史列表
export function getWorkflowHistoryList(params: HbtWorkflowHistoryQuery) {
  return request<HbtApiResponse<HbtWorkflowHistoryPagedResult>>({
    url: '/api/HbtWorkflowHistory/list',
    method: 'get',
    params
  })
}

// 获取工作流历史详情
export function getWorkflowHistory(id: number) {
  return request<HbtApiResponse<HbtWorkflowHistory>>({
    url: `/api/HbtWorkflowHistory/${id}`,
    method: 'get'
  })
}

// 创建工作流历史
export function createWorkflowHistory(data: HbtWorkflowHistoryCreate) {
  return request<HbtApiResponse<any>>({
    url: '/api/HbtWorkflowHistory',
    method: 'post',
    data
  })
}

// 更新工作流历史
export function updateWorkflowHistory(data: HbtWorkflowHistoryUpdate) {
  return request<HbtApiResponse<any>>({
    url: '/api/HbtWorkflowHistory',
    method: 'put',
    data
  })
}

// 删除工作流历史
export function deleteWorkflowHistory(id: number) {
  return request<HbtApiResponse<any>>({
    url: `/api/HbtWorkflowHistory/${id}`,
    method: 'delete'
  })
}

// 批量删除工作流历史
export function batchDeleteWorkflowHistory(ids: number[]) {
  return request<HbtApiResponse<any>>({
    url: '/api/HbtWorkflowHistory/batch',
    method: 'delete',
    data: ids
  })
}

// 导入工作流历史
export function importWorkflowHistory(file: File, sheetName: string = 'Sheet1') {
  const formData = new FormData()
  formData.append('file', file)
  return request<HbtApiResponse<any>>({
    url: '/api/HbtWorkflowHistory/import',
    method: 'post',
    data: formData,
    params: { sheetName },
    headers: {
      'Content-Type': 'multipart/form-data'
    }
  })
}

// 导出工作流历史
export function exportWorkflowHistory(params: HbtWorkflowHistoryQuery, sheetName: string = 'Sheet1') {
  return request({
    url: '/api/HbtWorkflowHistory/export',
    method: 'get',
    params: { ...params, sheetName },
    responseType: 'blob'
  })
}

// 获取工作流历史导入模板
export function getWorkflowHistoryTemplate(sheetName: string = 'Sheet1') {
  return request({
    url: '/api/HbtWorkflowHistory/template',
    method: 'get',
    params: { sheetName },
    responseType: 'blob'
  })
}

// 获取工作流实例的历史记录
export function getHistoriesByWorkflowInstance(workflowInstanceId: number) {
  return request<HbtApiResponse<HbtWorkflowHistory[]>>({
    url: `/api/HbtWorkflowHistory/instance/${workflowInstanceId}`,
    method: 'get'
  })
}

// 获取工作流节点的历史记录
export function getHistoriesByWorkflowNode(workflowNodeId: number) {
  return request<HbtApiResponse<HbtWorkflowHistory[]>>({
    url: `/api/HbtWorkflowHistory/node/${workflowNodeId}`,
    method: 'get'
  })
}

// 获取用户的操作历史记录
export function getHistoriesByOperator(operatorId: number) {
  return request<HbtApiResponse<HbtWorkflowHistory[]>>({
    url: `/api/HbtWorkflowHistory/operator/${operatorId}`,
    method: 'get'
  })
}

// 清理历史记录
export function cleanupHistories(days: number) {
  return request<HbtApiResponse<any>>({
    url: '/api/HbtWorkflowHistory/cleanup',
    method: 'post',
    params: { days }
  })
} 