import type { HbtBaseEntity, HbtPagedQuery, HbtPagedResult } from '@/types/common'

/**
 * 部门查询参数
 */
export interface DeptQuery extends HbtPagedQuery {
  deptName?: string;
  status?: number;
}

/**
 * 部门信息
 */
export interface Dept extends HbtBaseEntity {
  deptId?: number;
  deptName: string;
  parentId: number;
  orderNum: number;
  leader: string;
  phone: string;
  email: string;
  status: number;
  children?: Dept[];
}

/**
 * 部门创建参数
 */
export interface DeptCreate {
  deptName: string;
  parentId: number;
  orderNum: number;
  leader?: string;
  phone?: string;
  email?: string;
  status: number;
}

/**
 * 部门更新参数
 */
export interface DeptUpdate extends DeptCreate {
  deptId: number;
}

/**
 * 部门树形节点
 */
export interface DeptTreeNode {
  id: number;
  label: string;
  children?: DeptTreeNode[];
}

/**
 * 部门状态更新参数
 */
export interface DeptStatus {
  deptId: number;
  status: number;
}

/**
 * 部门导出参数
 */
export interface DeptExport {
  deptId: number;
  parentId: number;
  ancestors: string;
  deptName: string;
  orderNum: number;
  leader: string;
  phone: string;
  email: string;
  status: number;
}

/**
 * 部门分页结果
 */
export type DeptPageResult = HbtPagedResult<Dept> 