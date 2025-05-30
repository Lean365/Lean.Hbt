<!-- 
===================================================================
项目名称: Lean.Hbt
文件名称: ConfigDetail.vue
创建日期: 2024-03-20
描述: 系统配置详情组件
=================================================================== 
-->

<template>
  <hbt-modal
    :open="open"
    :title="t('core.config.title')"
    :width="800"
    @update:open="handleOpenChange"
  >
    <a-descriptions :column="2" bordered>
      <a-descriptions-item :label="t('core.config.fields.configName.label')" :span="1">
        {{ form.configName }}
      </a-descriptions-item>
      <a-descriptions-item :label="t('core.config.fields.configKey.label')" :span="1">
        {{ form.configKey }}
      </a-descriptions-item>
      <a-descriptions-item :label="t('core.config.fields.configValue.label')" :span="2">
        {{ form.configValue }}
      </a-descriptions-item>
      <a-descriptions-item :label="t('core.config.fields.isBuiltin.label')" :span="1">
        <hbt-dict-tag :dict-type="'sys_yes_no'" :value="form.isBuiltin" />
      </a-descriptions-item>
      <a-descriptions-item :label="t('core.config.fields.orderNum.label')" :span="1">
        {{ form.orderNum }}
      </a-descriptions-item>
      <a-descriptions-item :label="t('core.config.fields.status.label')" :span="1">
        <hbt-dict-tag :dict-type="'sys_normal_disable'" :value="form.status" />
      </a-descriptions-item>
      <a-descriptions-item :label="t('table.columns.remark')" :span="2">
        {{ form.remark }}
      </a-descriptions-item>
      <a-descriptions-item :label="t('table.columns.createTime')" :span="1">
        {{ form.createTime }}
      </a-descriptions-item>
      <a-descriptions-item :label="t('table.columns.updateTime')" :span="1">
        {{ form.updateTime }}
      </a-descriptions-item>
      <a-descriptions-item :label="t('table.columns.createBy')" :span="1">
        {{ form.createBy }}
      </a-descriptions-item>
      <a-descriptions-item :label="t('table.columns.updateBy')" :span="1">
        {{ form.updateBy }}
      </a-descriptions-item>
    </a-descriptions>
  </hbt-modal>
</template>

<script lang="ts" setup>
import { ref, reactive, watch } from 'vue'
import { useI18n } from 'vue-i18n'
import { message } from 'ant-design-vue'
import type { HbtConfig } from '@/types/core/config'
import { getHbtConfig } from '@/api/core/config'

const { t } = useI18n()

const props = defineProps<{
  open: boolean
  configId?: number
}>()

const emit = defineEmits<{
  (e: 'update:open', open: boolean): void
}>()

// 表单数据
const form = reactive<HbtConfig>({
  configId: 0,
  configName: '',
  configKey: '',
  configValue: '',
  isBuiltin: 0,
  orderNum: 0,
  remark: '',
  status: 0,
  createTime: '',
  updateTime: '',
  createBy: '',
  updateBy: '',
  isDeleted: 0,
  isEncrypted: 0
})

// 获取配置信息
const getInfo = async (configId: number) => {
  try {
    const res = await getHbtConfig(configId)
    if (res.data.code === 200) {
      Object.assign(form, res.data.data)
    } else {
      message.error(res.data.msg || t('common.failed'))
    }
  } catch (error) {
    console.error(error)
    message.error(t('common.failed'))
  }
}

// 处理对话框显示状态变化
const handleOpenChange = (val: boolean) => {
  emit('update:open', val)
}

// 监听配置ID变化
watch(
  () => props.configId,
  (val) => {
    if (val) {
      getInfo(val)
    }
  }
)
</script>

<style lang="less" scoped>
:deep(.ant-descriptions) {
  padding: 24px;
}

:deep(.ant-descriptions-item-label) {
  width: 120px;
  text-align: right;
  background-color: #fafafa;
}

:deep(.ant-descriptions-item-content) {
  padding: 12px 24px;
}
</style> 