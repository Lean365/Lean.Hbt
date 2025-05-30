import request from '@/utils/request'
import type { HbtApiResponse } from '@/types/common'
import type { HbtDept, HbtDeptQuery, HbtDeptCreate, HbtDeptUpdate, HbtDeptTemplate, HbtDeptImport, HbtDeptExport } from '@/types/identity/dept'

/**
 * 获取部门分页列表
 */
export function getPagedList(params: HbtDeptQuery) {
  return request<HbtApiResponse<HbtDept[]>>({
    url: '/api/HbtDept/list',
    method: 'get',
    params
  })
}

/**
 * 获取部门详情
 */
export function getDept(deptId: number) {
  return request<HbtApiResponse<HbtDept>>({
    url: `/api/HbtDept/${deptId}`,
    method: 'get'
  })
}

/**
 * 创建部门
 */
export function createDept(data: HbtDeptCreate) {
  return request<HbtApiResponse<number>>({
    url: '/api/HbtDept',
    method: 'post',
    data
  })
}

/**
 * 更新部门
 */
export function updateDept(data: HbtDeptUpdate) {
  return request<HbtApiResponse<boolean>>({
    url: '/api/HbtDept',
    method: 'put',
    data
  })
}

/**
 * 删除部门
 */
export function deleteDept(deptId: number) {
  return request<HbtApiResponse<boolean>>({
    url: `/api/HbtDept/${deptId}`,
    method: 'delete'
  })
}

/**
 * 获取部门树形数据
 */
export function getDeptTree(params?: HbtDeptQuery) {
  return request<HbtApiResponse<HbtDept[]>>({
    url: '/api/HbtDept/tree',
    method: 'get',
    params
  })
}

/**
 * 获取部门选项
 */
export function getDeptOptions() {
  return request<HbtApiResponse<{ label: string; value: number }[]>>({
    url: '/api/HbtDept/options',
    method: 'get'
  })
}

/**
 * 批量删除部门
 */
export function batchDeleteDept(deptIds: number[]) {
  return request<HbtApiResponse<boolean>>({
    url: '/api/HbtDept/batch',
    method: 'delete',
    data: deptIds
  })
}

/**
 * 导出部门数据
 */
export function exportDept(params?: HbtDeptQuery) {
  return request<Blob>({
    url: '/api/HbtDept/export',
    method: 'get',
    params,
    responseType: 'blob',
  })
}

/**
 * 导入部门数据
 */
export function importDept(file: File) {
  const formData = new FormData()
  formData.append('file', file)
  return request<HbtApiResponse<{ success: number; fail: number }>>({
    url: '/api/HbtDept/import',
    method: 'post',
    data: formData,
    headers: {
      'Content-Type': 'multipart/form-data'
    }
  })
}

/**
 * 下载部门导入模板
 */
export function getTemplate() {
  return request<Blob>({
    url: '/api/HbtDept/template',
    method: 'get',
    responseType: 'blob'
  })
}

/**
 * 更新部门状态
 */
export function updateDeptStatus(data: { deptId: number; status: number }) {
  return request<HbtApiResponse<boolean>>({
    url: `/api/HbtDept/${data.deptId}/status`,
    method: 'put',
    params: { status: data.status }
  })
} 