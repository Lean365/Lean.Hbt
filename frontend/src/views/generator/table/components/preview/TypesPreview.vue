<template>
  <div class="types-preview">
    <code-preview :code="code" />
  </div>
</template>

<script lang="ts" setup>
defineProps<{
  code: string
}>()
</script>

<style lang="less" scoped>
.types-preview {
  height: 100%;
}
</style> 