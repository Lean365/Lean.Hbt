import { createI18n } from 'vue-i18n'

// 导入管理模块翻译
import adminTranslationZhCN from './admin/translation/zh-CN'
import adminTranslationEnUS from './admin/translation/en-US'
import adminLanguageZhCN from './admin/language/zh-CN'
import adminLanguageEnUS from './admin/language/en-US'
import adminDictTypeZhCN from './admin/dicttype/zh-CN'
import adminDictTypeEnUS from './admin/dicttype/en-US'

// 导入代码生成器翻译
import generatorZhCN from './generator/zh-CN'
import generatorEnUS from './generator/en-US'

// 导入公共翻译
import commonZhCN from './common/zh-CN'
import commonEnUS from './common/en-US'

// 导入仪表盘翻译
import dashboardZhCN from './dashboard/zh-CN'
import dashboardEnUS from './dashboard/en-US'

// 导入错误页面翻译
import errorZhCN from './error/zh-CN'
import errorEnUS from './error/en-US'

// 导入页脚翻译
import footerZhCN from './components/footer/zh-CN'
import footerEnUS from './components/footer/en-US'

// 导入页头翻译
import headerZhCN from './components/header/zh-CN'
import headerEnUS from './components/header/en-US'

// 导入节日翻译
import holidayZhCN from './components/holiday/zh-CN'
import holidayEnUS from './components/holiday/en-US'

// 导入首页翻译
import homeZhCN from './home/zh-CN'
import homeEnUS from './home/en-US'

// 导入身份认证翻译
import identityAuthZhCN from './identity/auth/zh-CN'
import identityAuthEnUS from './identity/auth/en-US'

// 导入身份菜单翻译
import identityMenuZhCN from './identity/menu/zh-CN'
import identityMenuEnUS from './identity/menu/en-US'

// 导入本地化翻译
import localeZhCN from './components/locale/zh-CN'
import localeEnUS from './components/locale/en-US'

// 导入菜单翻译
import menuArSA from './components/menu/ar-SA'
import menuEnUS from './components/menu/en-US'
import menuEsES from './components/menu/es-ES'
import menuFrFR from './components/menu/fr-FR'
import menuJaJP from './components/menu/ja-JP'
import menuKoKR from './components/menu/ko-KR'
import menuRuRU from './components/menu/ru-RU'
import menuZhCN from './components/menu/zh-CN'
import menuZhTw from './components/menu/zh-TW'

// 导入分页翻译
import paginationZhCN from './components/pagination/zh-CN'
import paginationEnUS from './components/pagination/en-US'

// 导入表格翻译
import tableZhCN from './components/table/zh-CN'
import tableEnUS from './components/table/en-US'
import columnSettingsZhCN from './components/table/column-settings/zh-CN'
import columnSettingsEnUS from './components/table/column-settings/en-US'

// 导入查询翻译
import queryZhCN from './components/query/zh-CN'
import queryEnUS from './components/query/en-US'

// 导入路由翻译
import routerZhCN from './router/zh-CN'
import routerEnUS from './router/en-US'

// 导入主题翻译
import themeZhCN from './components/theme/zh-CN'
import themeEnUS from './components/theme/en-US'

// 导入身份认证相关翻译
import identityUserZhCN from './identity/user/zh-CN'
import identityUserEnUS from './identity/user/en-US'
import identityRoleZhCN from './identity/role/zh-CN'
import identityRoleEnUS from './identity/role/en-US'
import identityDeptZhCN from './identity/dept/zh-CN'
import identityDeptEnUS from './identity/dept/en-US'
import identityPostZhCN from './identity/post/zh-CN'
import identityPostEnUS from './identity/post/en-US'
import identityTenantZhCN from './identity/tenant/zh-CN'
import identityTenantEnUS from './identity/tenant/en-US'

// 导入组件翻译
import iconZhCN from './components/icon/zh-CN'
import iconEnUS from './components/icon/en-US'

// 合并所有翻译
const messages = {
  'ar-SA': {
    ...menuArSA // 菜单翻译
  },
  'en-US': {
    admin: {
      ...adminTranslationEnUS.admin,
      ...adminLanguageEnUS.admin,
      ...adminDictTypeEnUS.admin
    },
    ...generatorEnUS,
    ...commonEnUS,
    ...dashboardEnUS,
    ...errorEnUS,
    ...footerEnUS,
    ...headerEnUS,
    ...holidayEnUS,
    ...homeEnUS,
    identity: {
      ...identityAuthEnUS.identity,
      ...identityMenuEnUS.identity,
      ...identityUserEnUS.identity,
      ...identityRoleEnUS.identity,
      ...identityDeptEnUS.identity,
      ...identityPostEnUS.identity,
      ...identityTenantEnUS.identity
    },
    ...iconEnUS,
    ...localeEnUS,
    ...menuEnUS,
    ...paginationEnUS,
    table: {
      ...tableEnUS.table,
      ...columnSettingsEnUS.table
    },
    ...queryEnUS,
    ...routerEnUS,
    ...themeEnUS
  },
  'es-ES': {
    ...menuEsES // 菜单翻译
  },
  'fr-FR': {
    ...menuFrFR // 菜单翻译
  },
  'ja-JP': {
    ...menuJaJP // 菜单翻译
  },
  'ko-KR': {
    ...menuKoKR // 菜单翻译
  },
  'ru-RU': {
    ...menuRuRU // 菜单翻译
  },

  'zh-CN': {
    admin: {
      ...adminTranslationZhCN.admin,
      ...adminLanguageZhCN.admin,
      ...adminDictTypeZhCN.admin
    },
    ...generatorZhCN,
    ...commonZhCN,
    ...dashboardZhCN,
    ...errorZhCN,
    ...footerZhCN,
    ...headerZhCN,
    ...holidayZhCN,
    ...homeZhCN,
    identity: {
      ...identityAuthZhCN.identity,
      ...identityMenuZhCN.identity,
      ...identityUserZhCN.identity,
      ...identityRoleZhCN.identity,
      ...identityDeptZhCN.identity,
      ...identityPostZhCN.identity,
      ...identityTenantZhCN.identity
    },
    ...iconZhCN,
    ...localeZhCN,
    ...menuZhCN,
    ...paginationZhCN,
    table: {
      ...tableZhCN.table,
      ...columnSettingsZhCN.table
    },
    ...queryZhCN,
    ...routerZhCN,
    ...themeZhCN
  },
  'zh-TW': {
    ...menuZhTw // 菜单翻译
  }
}

// 创建i18n实例
const i18n = createI18n({
  legacy: false, // 使用Composition API模式
  locale: localStorage.getItem('language') || 'zh-CN',
  fallbackLocale: 'zh-CN',
  messages,
  globalInjection: true, // 全局注入 $t 函数
  silentTranslationWarn: true, // 关闭翻译警告
  missingWarn: false, // 关闭缺少翻译警告
  silentFallbackWarn: true, // 关闭回退翻译警告
  // 添加语言环境映射
  availableLocales: ['zh-CN', 'en-US'],
  fallbackLocaleChain: {
    zh: ['zh-CN', 'en-US'],
    'zh-CN': ['zh-CN', 'en-US'],
    en: ['en-US', 'zh-CN'],
    'en-US': ['en-US', 'zh-CN']
  }
})

export default i18n
