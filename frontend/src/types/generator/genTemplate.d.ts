//===================================================================
// 项目名 : Lean.Hbt
// 文件名 : genTemplate.d.ts
// 创建者 : Claude
// 创建时间: 2024-04-24
// 版本号 : v1.0.0
// 描述    : 代码生成模板类型定义
//===================================================================

import type { HbtBaseEntity, HbtPagedQuery, HbtPagedResult } from '@/types/common'

/**
 * 代码生成模板对象
 */
export interface HbtGenTemplate extends HbtBaseEntity {
  /** 模板ID */
  genTemplateId: number
  /** 模板名称 */
  templateName: string
  /** 模板类型（1：后端代码，2：前端代码，3：SQL代码） */
  templateType: number
  /** 模板分类（1：实体类，2：控制器，3：服务层，4：数据访问层，5：视图，6：API，7：其他） */
  templateCategory: number
  /** 模板文件名 */
  fileName: string
  /** 模板内容 */
  content: string
  /** 生成路径 */
  genPath: string
  /** 生成规则（1：覆盖，2：增量） */
  genRule: number
  /** 排序号 */
  orderNum: number

  /** 状态（0：停用，1：正常） */
  status: number
}

/**
 * 代码生成模板查询参数
 */
export interface HbtGenTemplateQuery extends HbtPagedQuery {
  /** 模板名称 */
  templateName?: string
  /** 模板类型 */
  templateType?: number
  /** 模板分类 */
  templateCategory?: number
  /** 状态 */
  status?: number
  /** 开始时间 */
  beginTime?: string
  /** 结束时间 */
  endTime?: string
}

/**
 * 创建代码生成模板参数
 */
export interface HbtGenTemplateCreate {
  /** 模板名称 */
  templateName: string
  /** 模板类型 */
  templateType: number
  /** 模板分类 */
  templateCategory: number
  /** 模板文件名 */
  fileName: string
  /** 模板内容 */
  content: string
  /** 生成路径 */
  genPath: string
  /** 生成规则 */
  genRule: number
  /** 排序号 */
  orderNum: number
  /** 备注 */
  remark?: string
  /** 状态 */
  status: number
}

/**
 * 更新代码生成模板参数
 */
export interface HbtGenTemplateUpdate extends HbtGenTemplateCreate {
  /** 模板ID */
  genTemplateId: number
}

/**
 * 代码生成模板模板
 */
export interface HbtGenTemplateTemplate {
  templateName: string
  templateType: string
  templateCategory: string
  fileName: string
  content: string
  genPath: string
  genRule: string
  orderNum: string
  remark: string
  status: string
}

/**
 * 代码生成模板导入参数
 */
export interface HbtGenTemplateImport {
  templateName: string
  templateType: number
  templateCategory: number
  fileName: string
  content: string
  genPath: string
  genRule: number
  orderNum: number
  status: number
  remark?: string
}

/**
 * 代码生成模板导出参数
 */
export interface HbtGenTemplateExport {
  templateName: string
  templateType: number
  templateCategory: number
  fileName: string
  content: string
  genPath: string
  genRule: number
  orderNum: number
  status: number
  remark?: string
  createTime: string
}

/**
 * 代码生成模板分页结果
 */
export type HbtGenTemplatePageResult = HbtPagedResult<HbtGenTemplate> 