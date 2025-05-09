import type { HbtBaseEntity, HbtPagedQuery, HbtPagedResult, HbtApiResponse } from '@/types/common'
import type { MenuProps } from 'ant-design-vue'

/**
 * 菜单类型枚举
 */
export enum HbtMenuType {
  /** 目录 */
  Directory = 0,
  /** 菜单 */
  Menu = 1,
  /** 按钮 */
  Button = 2
}

/**
 * 菜单查询参数
 */
export interface MenuQuery extends HbtPagedQuery {
  menuName?: string;
  status?: number;
}

/**
 * 后端返回的菜单项类型
 */
export interface Menu {
  menuName: string
  transKey?: string
  parentId?: number
  orderNum: number
  path: string
  component: string
  queryParams?: string
  isExternal?: number
  isCache?: number
  menuType: number
  visible?: number
  status?: number
  perms?: string
  icon?: string
  tenantId?: number
  remark?: string
  children?: Menu[]
  createTime?: string
  menuId: number
}

/**
 * 前端菜单项类型
 */
export type MenuNode = Required<NonNullable<MenuProps['items']>>[number] & {
  /** 菜单ID */
  menuId: string;
  /** 父菜单ID */
  parentId: string | null;
  /** 菜单路由路径 */
  path: string;
  /** 菜单组件路径 */
  component?: string;
  /** 权限标识 */
  perms?: string;
  /** 子菜单 */
  children?: MenuNode[];
};

/**
 * 创建菜单参数
 */
export interface MenuCreate {
  menuName: string;
  transKey?: string;
  parentId?: number;
  orderNum: number;
  path?: string;
  component?: string;
  queryParams?: string;
  /** 是否外链（0=否 1=是） */
  isExternal?: number;
  /** 是否缓存（0=不缓存 1=缓存） */
  isCache?: number;
  /** 菜单类型（0=目录 1=菜单 2=按钮） */
  menuType: number;
  /** 是否可见（0=显示 1=隐藏） */
  visible?: number;
  /** 状态（0=正常 1=停用） */
  status: number;
  perms?: string;
  icon?: string;
  tenantId?: number;
  remark?: string;
}

/**
 * 更新菜单参数
 */
export interface MenuUpdate extends MenuCreate {
  menuId: number;
}

/**
 * 菜单状态更新参数
 */
export interface MenuStatus {
  menuId: number;
  status: number;
}

/**
 * 菜单排序更新参数
 */
export interface MenuOrder {
  menuId: number;
  orderNum: number;
}

/**
 * 菜单分页结果
 */
export type MenuPageResult = HbtPagedResult<Menu> 