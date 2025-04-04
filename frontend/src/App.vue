<template>
  <ConfigProvider :theme="themeConfig">
    <a-config-provider :locale="antdLocale">
      <div id="app">
        <hbt-error-alert
          v-model:visible="showError"
          :type="errorType"
          :message="errorMessage"
          :description="errorDescription"
          :show-retry="true"
          @retry="handleRetry"
          @close="handleErrorClose"
        />
        <router-view></router-view>
      </div>
    </a-config-provider>
  </ConfigProvider>
</template>

<script lang="ts" setup>
import 'ant-design-vue/dist/reset.css'
import { ConfigProvider } from 'ant-design-vue'
import { onMounted, computed, watch, onUnmounted } from 'vue'
import { theme } from 'ant-design-vue'
import { useThemeStore } from '@/stores/theme'
import { useHolidayStore } from '@/stores/holiday'
import { useAppStore } from '@/stores/app'
import zhCN from 'ant-design-vue/es/locale/zh_CN'
import enUS from 'ant-design-vue/es/locale/en_US'
import { initAutoLogout, clearAutoLogout } from '@/utils/autoLogout'
import { useDictStore } from '@/stores/dict'
import { useWebSocketStore } from '@/stores/websocket'
import HbtErrorAlert from '@/components/Business/ErrorAlert/index.vue'

const themeStore = useThemeStore()
const holidayStore = useHolidayStore()
const appStore = useAppStore()
const wsStore = useWebSocketStore()
const isDark = computed(() => themeStore.isDarkMode)
const currentTheme = computed(() => holidayStore.holidayTheme)
const isMemorialMode = computed(() => currentTheme.value?.id === 'memorial')

// 根据当前语言获取 Ant Design Vue 的语言包
const antdLocale = computed(() => {
  switch (appStore.language) {
    case 'en-US':
      return enUS
    case 'zh-CN':
    default:
      return zhCN
  }
})

// 监听语言变化，刷新页面以应用新的语言
watch(() => appStore.language, () => {
  window.location.reload()
})

// 计算主题配置
const themeConfig = computed(() => {
  const baseConfig = {
    algorithm: isDark.value ? theme.darkAlgorithm : theme.defaultAlgorithm,
  }

  // 如果是纪念模式，应用纪念模式的主题配置
  if (isMemorialMode.value && currentTheme.value?.theme) {
    return {
      ...baseConfig,
      token: {
        colorPrimary: currentTheme.value.theme.colorPrimary,
        colorBgContainer: currentTheme.value.theme.colorBgContainer,
        colorBgLayout: currentTheme.value.theme.colorBgLayout,
        colorText: currentTheme.value.theme.colorText,
        colorTextSecondary: currentTheme.value.theme.colorTextSecondary,
        colorBorder: currentTheme.value.theme.colorBorder,
        colorSplit: currentTheme.value.theme.colorSplit,
      }
    }
  }
  // 如果是其他节日主题
  else if (currentTheme.value?.theme) {
    return {
      ...baseConfig,
      token: {
        colorPrimary: currentTheme.value.theme.colorPrimary,
        colorBgContainer: currentTheme.value.theme.colorBgContainer,
        colorBgLayout: currentTheme.value.theme.colorBgLayout,
        colorText: currentTheme.value.theme.colorText,
        colorTextSecondary: currentTheme.value.theme.colorTextSecondary,
        colorBorder: currentTheme.value.theme.colorBorder,
        colorSplit: currentTheme.value.theme.colorSplit,
      }
    }
  }

  return baseConfig
})

// 错误提示相关
const showError = ref(false)
const errorType = ref<'warning' | 'error'>('warning')
const errorMessage = ref('')
const errorDescription = ref('')

// 监听 WebSocket 错误
const handleWebSocketError = () => {
  if (wsStore.error) {
    errorType.value = 'error'
    errorMessage.value = '连接错误'
    errorDescription.value = wsStore.error
    showError.value = true
  }
}

// 监听 WebSocket 连接状态
const handleWebSocketConnection = () => {
  if (!wsStore.connected) {
    errorType.value = 'warning'
    errorMessage.value = '连接断开'
    errorDescription.value = '正在尝试重新连接...'
    showError.value = true
  } else {
    showError.value = false
  }
}

// 处理重试
const handleRetry = () => {
  wsStore.connect()
}

// 处理关闭错误提示
const handleErrorClose = () => {
  showError.value = false
}

// 组件挂载时连接 WebSocket
onMounted(() => {
  const dictStore = useDictStore()
  dictStore.clearCache()
  themeStore.initTheme()
  holidayStore.initHolidayTheme()
  document.documentElement.style.colorScheme = isDark.value ? 'dark' : 'light'
  initAutoLogout()
  wsStore.connect()
})

onUnmounted(() => {
  clearAutoLogout()
  wsStore.disconnect()
})

watch(isDark, (newValue) => {
  document.documentElement.style.colorScheme = newValue ? 'dark' : 'light'
})

// 监听纪念模式的变化
watch(isMemorialMode, (newValue) => {
  if (newValue) {
    document.documentElement.style.filter = currentTheme.value?.theme?.filter_css || ''
    document.body.classList.add('memorial-mode')
  } else {
    document.documentElement.style.filter = ''
    document.body.classList.remove('memorial-mode')
  }
})

// 监听 WebSocket 状态变化
watch(() => wsStore.error, handleWebSocketError)
watch(() => wsStore.connected, handleWebSocketConnection)
</script>

<style>
#app {
  width: 100%;
  height: 100vh;
  background-color: var(--ant-color-bg-layout);
  color: var(--ant-color-text);
}

body {
  margin: 0;
  padding: 0;
  background-color: var(--ant-color-bg-layout);
  color: var(--ant-color-text);
}

.theme-dark {
  background-color: var(--ant-color-bg-layout);
  color: var(--ant-color-text);
  min-height: 100vh;
}

:root {
  color-scheme: light dark;
}

/* 添加全局主题变量应用 */
* {
  transition: background-color 0.3s, color 0.3s, filter 0.3s;
}

/* 纪念模式样式 */
body.memorial-mode,
body.memorial-mode #app,
body.memorial-mode .ant-dropdown,
body.memorial-mode .ant-modal-root,
body.memorial-mode .ant-message,
body.memorial-mode .ant-notification,
body.memorial-mode .ant-drawer {
  filter: grayscale(100%) contrast(90%) brightness(90%);
  transition: filter 0.3s ease;
}

/* 确保所有容器都使用主题颜色 */
.ant-layout {
  background: var(--ant-color-bg-layout);
}

.ant-layout-content {
  background: var(--ant-color-bg-container);
}

.ant-card {
  background: var(--ant-color-bg-container);
  border-color: var(--ant-color-border);
}

.ant-menu {
  background: var(--ant-color-bg-container);
  border-color: var(--ant-color-border);
}

.ant-table {
  background: var(--ant-color-bg-container);
  color: var(--ant-color-text);
}

.ant-modal-content {
  background: var(--ant-color-bg-container);
  color: var(--ant-color-text);
}

.ant-drawer-content {
  background: var(--ant-color-bg-container);
  color: var(--ant-color-text);
}

/* 确保所有容器都使用主题颜色 */
.ant-dropdown-menu {
  background: var(--ant-color-bg-container);
  border-color: var(--ant-color-border);
}

.ant-dropdown-menu-item {
  color: var(--ant-color-text);
  
  &:hover {
    background: var(--ant-color-bg-container-hover);
  }
}

.ant-dropdown-menu-item-group-title {
  color: var(--ant-color-text-secondary);
}
</style> 