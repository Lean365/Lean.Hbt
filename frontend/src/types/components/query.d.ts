// 查询字段类型定义
export type FieldType = 'input' | 'select' | 'date' | 'dateRange' | 'number'

// 查询字段选项接口
export interface QueryFieldOption {
  label: string
  value: string | number | boolean
}

// 查询字段接口
export interface QueryField {
  name: string
  label: string
  type: 'input' | 'select' | 'date' | 'datetime' | 'number'
  placeholder?: string
  options?: QueryFieldOption[]
  defaultValue?: any
  props?: Record<string, any>
} 