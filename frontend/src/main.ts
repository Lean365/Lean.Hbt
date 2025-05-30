//===================================================================
// 项目名 : Lean.Hbt
// 文件名 : main.ts
// 创建者 : Claude
// 创建时间: 2024-03-20
// 版本号 : v1.0.0
// 描述    : 应用程序入口文件
//===================================================================

import { createApp, watch } from 'vue'
import { createPinia } from 'pinia'
import Antd from 'ant-design-vue'
import 'ant-design-vue/dist/reset.css'
import App from './App.vue'
import router from './router'
import i18n from './locales'
import { useAppStore } from '@/stores/app'
import { useSettingStore } from '@/stores/settings'
import { setupPermission } from './directives/permission'
import setupIcons from '@/utils/icons'
import { useUserStore } from './stores/user'
import { useMenuStore } from './stores/menu'
import { getToken } from './utils/auth'
import { enUS } from 'date-fns/locale'

// 初始化函数
async function bootstrap() {
  try {
    const app = createApp(App)
    const pinia = createPinia()

    // 初始化状态管理
    app.use(pinia)
    
    // 初始化应用配置
    const appStore = useAppStore()
    const settingStore = useSettingStore()
    
    // 注册 Ant Design Vue
    app.use(Antd)

    // 设置图标
    setupIcons(app)

    // 注册国际化
    app.use(i18n)
    
    // 设置默认语言包
    app.config.globalProperties.$dateLocale = enUS

    // 注册权限指令
    setupPermission(app)

    // 如果有token，预加载用户信息和菜单
    const token = getToken()
    if (token) {
      console.log('[应用] 检测到token，开始预加载用户信息和菜单')
      try {
        // 获取 store 实例
        const userStore = useUserStore()
        const menuStore = useMenuStore()

        // 加载用户信息
        await userStore.getUserInfo()
        
        // 加载菜单并注册路由
        console.log('[应用] 开始加载菜单')
        const menus = await menuStore.reloadMenus(router)
        if (menus && menus.length > 0) {
          console.log('[应用] 菜单加载完成，路由注册完成')
        } else {
          console.warn('[应用] 未加载到菜单数据')
        }
      } catch (error) {
        console.error('[应用] 预加载失败:', error)
      }
    }

    // 注册路由（确保在动态路由加载完成后再注册）
    app.use(router)

    // 挂载应用
    app.mount('#app')
    
    console.log('[应用] 初始化完成')
  } catch (error) {
    console.error('[应用] 初始化失败:', error)
  }
}

// 启动应用
bootstrap()