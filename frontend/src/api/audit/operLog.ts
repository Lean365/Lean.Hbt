import request from '@/utils/request'
import type { HbtApiResponse } from '@/types/common'
import type { 
  OperLogQuery, 
  OperLog,
  OperLogExport
} from '@/types/audit/operLog'

// 获取操作日志列表
export function getOperLogList(params: OperLogQuery) {
  return request<HbtApiResponse<OperLog[]>>({
    url: '/api/HbtOperLog',
    method: 'get',
    params
  })
}

// 获取操作日志详情
export function getOperLog(id: number) {
  return request<HbtApiResponse<OperLog>>({
    url: `/api/HbtOperLog/${id}`,
    method: 'get'
  })
}

// 清空操作日志
export function clearOperLog() {
  return request<HbtApiResponse<any>>({
    url: '/api/HbtOperLog/clear',
    method: 'delete'
  })
}

// 导出操作日志
export function exportOperLog(params: OperLogExport, sheetName: string = '操作日志') {
  return request({
    url: '/api/HbtOperLog/export',
    method: 'get',
    params: { ...params, sheetName },
    responseType: 'blob'
  })
} 