<template>
  <a-modal
    :open="open"
    :title="title"
    width="1300px"
    @update:open="handleClose"
    @ok="handleSubmit"
    :confirm-loading="loading"
  >
    <a-tabs v-model:activeKey="activeKey">
      <a-tab-pane key="basic" :tab="t('generator.table.tab.basic')">
        <basic-info v-model="formData" :id="props.id" />
      </a-tab-pane>
      <a-tab-pane key="generate" :tab="t('generator.table.tab.generate')">
        <generate-info v-model="formData" :id="props.id" />
      </a-tab-pane>
      <a-tab-pane key="field" :tab="t('generator.table.tab.field')">
        <field-info v-model="formData" :id="props.id" />
      </a-tab-pane>
    </a-tabs>
  </a-modal>
</template>

<script lang="ts" setup>
import { ref, watch } from 'vue'
import { useI18n } from 'vue-i18n'
import type { FormInstance } from 'ant-design-vue'
import type { HbtGenTable } from '@/types/generator/genTable'
import { getTable, createTable, updateTable } from '@/api/generator/genTable'
import BasicInfo from './BasicInfo.vue'
import GenerateInfo from './GenerateInfo.vue'
import FieldInfo from './FieldInfo.vue'

const { t } = useI18n()

const props = defineProps<{
  open: boolean
  title: string
  id?: number
}>()

const emit = defineEmits<{
  (e: 'update:open', value: boolean): void
  (e: 'success'): void
}>()

const formRef = ref<FormInstance>()
const loading = ref(false)

// 表单数据
const formData = ref<HbtGenTable>({
  id: 0,
  createBy: '',
  createTime: '',
  updateBy: '',
  updateTime: '',
  deleteBy: '',
  deleteTime: '',
  isDeleted: 0,
  remark: '',
  databaseName: '',
  tableName: '',
  tableComment: '',
  baseNamespace: '',
  tplCategory: 'crud',
  subTableName: undefined,
  subTableFkName: undefined,
  treeCode: '',
  treeName: '',
  treeParentCode: '',
  moduleName: '',
  businessName: '',
  functionName: '',
  author: '',
  genType: '0',
  genPath: '',
  parentMenuId: 0,
  sortType: 'asc',
  sortField: '',
  permsPrefix: '',
  generateMenu: 1,
  frontTpl: 1,
  btnStyle: 1,
  frontStyle: 24,
  status: 1,
  entityClassName: '',
  entityNamespace: '',
  dtoType: [],
  dtoNamespace: '',
  dtoClassName: '',
  serviceNamespace: '',
  iServiceClassName: '',
  serviceClassName: '',
  iRepositoryNamespace: '',
  iRepositoryClassName: '',
  repositoryNamespace: '',
  repositoryClassName: '',
  controllerNamespace: '',
  controllerClassName: '',
  options: {
    isSqlDiff: 1,
    isSnowflakeId: 1,
    isRepository: 0,
    crudGroup: [1, 2, 3, 4, 5, 6, 7, 8]
  },
  columns: [],
  subTable: undefined,
  tenantId: 0
})

// 表单校验规则
const rules = {
  tableName: [{ required: true, message: t('generator.table.required.name') }],
  tableComment: [{ required: true, message: t('generator.table.required.comment') }],
  baseNamespace: [{ required: true, message: t('generator.table.required.baseNamespace') }],
  moduleName: [{ required: true, message: t('generator.table.required.moduleName') }],
  businessName: [{ required: true, message: t('generator.table.required.businessName') }],
  functionName: [{ required: true, message: t('generator.table.required.functionName') }],
  author: [{ required: true, message: t('generator.table.required.author') }],
  genMode: [{ required: true, message: t('generator.table.required.genMode') }],
  genPath: [{ required: true, message: t('generator.table.required.genPath') }]
}

// 当前激活的标签页
const activeKey = ref('basic')

// 监听表格ID变化
watch(() => props.id, async (newVal) => {
  console.log('TableForm - id 变化:', newVal, '类型:', typeof newVal)
  console.log('TableForm - 当前 props:', props)
  console.log('TableForm - 当前 formData:', formData.value)
  
  if (!newVal || typeof newVal !== 'number' || isNaN(newVal)) {
    console.log('TableForm - id 无效，等待有效值')
    formData.value = {
      id: 0,
      createBy: '',
      createTime: '',
      updateBy: '',
      updateTime: '',
      deleteBy: '',
      deleteTime: '',
      isDeleted: 0,
      remark: '',
      databaseName: '',
      tableName: '',
      tableComment: '',
      baseNamespace: '',
      tplCategory: 'crud',
      subTableName: undefined,
      subTableFkName: undefined,
      treeCode: '',
      treeName: '',
      treeParentCode: '',
      moduleName: '',
      businessName: '',
      functionName: '',
      author: '',
      genType: '0',
      genPath: '',
      parentMenuId: 0,
      sortType: 'asc',
      sortField: '',
      permsPrefix: '',
      generateMenu: 1,
      frontTpl: 1,
      btnStyle: 1,
      frontStyle: 24,
      status: 1,
      entityClassName: '',
      entityNamespace: '',
      dtoType: [],
      dtoNamespace: '',
      dtoClassName: '',
      serviceNamespace: '',
      iServiceClassName: '',
      serviceClassName: '',
      iRepositoryNamespace: '',
      iRepositoryClassName: '',
      repositoryNamespace: '',
      repositoryClassName: '',
      controllerNamespace: '',
      controllerClassName: '',
      options: {
        isSqlDiff: 1,
        isSnowflakeId: 1,
        isRepository: 0,
        crudGroup: [1, 2, 3, 4, 5, 6, 7, 8]
      },
      columns: [],
      subTable: undefined,
      tenantId: 0
    } as HbtGenTable
    return
  }

  try {
    console.log('TableForm - 开始获取表信息，id:', newVal)
    const res = await getTable(newVal)
    console.log('TableForm - 获取表信息响应:', res)
    
    if (!res || !res.data) {
      console.error('TableForm - 获取表信息响应无效:', res)
      return
    }

    if (res.data.code !== 200) {
      console.error('TableForm - 获取表信息失败:', res.data.msg)
      return
    }

    if (!res.data.data) {
      console.error('TableForm - 获取表信息数据为空')
      return
    }

    // 确保所有必需字段都有值，并设置正确的tableId
    formData.value = {
      ...res.data.data,
      id: res.data.data.id || 0,
      genPath: res.data.data.genPath || '',
      columns: res.data.data.columns?.map(column => ({
        ...column,
        tableId: res.data.data.id // 确保每个列都设置了正确的tableId
      })) || []
    } as HbtGenTable
    console.log('TableForm - 设置表单数据:', formData.value)
  } catch (error) {
    console.error('TableForm - 获取表信息失败:', error)
  }
}, { immediate: true })

// 监听表单数据变化
watch(
  () => formData.value,
  (newVal) => {
    console.log('TableForm - 表单数据变化:', newVal)
  },
  { deep: true }
)

// 关闭对话框
const handleClose = () => {
  emit('update:open', false)
}

// 提交表单
const handleSubmit = async () => {
  try {
    await formRef.value?.validate()
    loading.value = true
    
    const api = props.id ? updateTable : createTable
    const data = {
      id: formData.value.id,
      databaseName: formData.value.databaseName,
      tableName: formData.value.tableName,
      tableComment: formData.value.tableComment,
      baseNamespace: formData.value.baseNamespace,
      tplCategory: formData.value.tplCategory,
      subTableName: formData.value.subTableName,
      subTableFkName: formData.value.subTableFkName,
      treeCode: formData.value.treeCode,
      treeName: formData.value.treeName,
      treeParentCode: formData.value.treeParentCode,
      moduleName: formData.value.moduleName,
      businessName: formData.value.businessName,
      functionName: formData.value.functionName,
      author: formData.value.author,
      genType: formData.value.genType,
      genPath: formData.value.genPath,
      parentMenuId: formData.value.parentMenuId,
      sortType: formData.value.sortType,
      sortField: formData.value.sortField,
      permsPrefix: formData.value.permsPrefix,
      generateMenu: formData.value.generateMenu,
      frontTpl: formData.value.frontTpl,
      btnStyle: formData.value.btnStyle,
      frontStyle: formData.value.frontStyle,
      status: formData.value.status,
      entityClassName: formData.value.entityClassName,
      entityNamespace: formData.value.entityNamespace,
      dtoType: formData.value.dtoType,
      dtoNamespace: formData.value.dtoNamespace,
      dtoClassName: formData.value.dtoClassName,
      serviceNamespace: formData.value.serviceNamespace,
      iServiceClassName: formData.value.iServiceClassName,
      serviceClassName: formData.value.serviceClassName,
      iRepositoryNamespace: formData.value.iRepositoryNamespace,
      iRepositoryClassName: formData.value.iRepositoryClassName,
      repositoryNamespace: formData.value.repositoryNamespace,
      repositoryClassName: formData.value.repositoryClassName,
      controllerNamespace: formData.value.controllerNamespace,
      controllerClassName: formData.value.controllerClassName,
      options: formData.value.options,
      columns: formData.value.columns?.map(column => ({
        ...column,
        tableId: formData.value.id
      })),
      subTable: formData.value.subTable,
      tenantId: formData.value.tenantId || 0,
      createBy: formData.value.createBy || '',
      createTime: formData.value.createTime || '',
      isDeleted: formData.value.isDeleted || 0
    }
    const res = await api(data)
    
    if (res.data.code === 200) {
      emit('success')
      handleClose()
    }
  } catch (error) {
    console.error('提交失败:', error)
  } finally {
    loading.value = false
  }
}

const resetForm = () => {
  formData.value = {
    id: 0,
    createBy: '',
    createTime: '',
    updateBy: '',
    updateTime: '',
    deleteBy: '',
    deleteTime: '',
    isDeleted: 0,
    remark: '',
    databaseName: '',
    tableName: '',
    tableComment: '',
    baseNamespace: '',
    tplCategory: 'crud',
    subTableName: undefined,
    subTableFkName: undefined,
    treeCode: '',
    treeName: '',
    treeParentCode: '',
    moduleName: '',
    businessName: '',
    functionName: '',
    author: '',
    genType: '0',
    genPath: '',
    parentMenuId: 0,
    sortType: 'asc',
    sortField: '',
    permsPrefix: '',
    generateMenu: 1,
    frontTpl: 1,
    btnStyle: 1,
    frontStyle: 24,
    status: 1,
    entityClassName: '',
    entityNamespace: '',
    dtoType: [],
    dtoNamespace: '',
    dtoClassName: '',
    serviceNamespace: '',
    iServiceClassName: '',
    serviceClassName: '',
    iRepositoryNamespace: '',
    iRepositoryClassName: '',
    repositoryNamespace: '',
    repositoryClassName: '',
    controllerNamespace: '',
    controllerClassName: '',
    options: {
      isSqlDiff: 1,
      isSnowflakeId: 1,
      isRepository: 0,
      crudGroup: [1, 2, 3, 4, 5, 6, 7, 8]
    },
    columns: [],
    subTable: undefined,
    tenantId: 0
  }
}
</script>

<style lang="less" scoped>
:deep(.ant-form-item-label) {
  font-weight: bold;
}
</style>
