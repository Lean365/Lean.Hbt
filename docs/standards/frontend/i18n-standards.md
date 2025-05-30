# 前端多语言开发规范

## 1. 目录结构规范

### 基础目录结构
```
src/
└── locales/                # 多语言文件根目录
    ├── admin/             # 管理模块
    │   ├── zh-cn/        # 简体中文
    │   │   ├── common.ts # 公共翻译
    │   │   └── menu.ts   # 菜单翻译
    │   └── en-us/        # 英文
    │       ├── common.ts
    │       └── menu.ts
    ├── app/              # 应用模块
    │   ├── zh-cn/
    │   │   ├── dashboard.ts
    │   │   └── workspace.ts
    │   └── en-us/
    │       ├── dashboard.ts
    │       └── workspace.ts
    ├── user/             # 用户模块
    │   ├── zh-cn/
    │   │   ├── profile.ts
    │   │   └── settings.ts
    │   └── en-us/
    │       ├── profile.ts
    │       └── settings.ts
    ├── menu/             # 菜单模块
    │   ├── zh-cn/
    │   │   └── index.ts
    │   └── en-us/
    │       └── index.ts
    └── index.ts          # 统一导入所有模块的语言包
```

### 语言文件组织
```typescript
// locales/index.ts
import adminZhCN from './admin/zh-cn';
import adminEnUS from './admin/en-us';
import appZhCN from './app/zh-cn';
import appEnUS from './app/en-us';
import userZhCN from './user/zh-cn';
import userEnUS from './user/en-us';
import menuZhCN from './menu/zh-cn';
import menuEnUS from './menu/en-us';

// 合并语言包
export const messages = {
  'zh-CN': {
    admin: adminZhCN,
    app: appZhCN,
    user: userZhCN,
    menu: menuZhCN
  },
  'en-US': {
    admin: adminEnUS,
    app: appEnUS,
    user: userEnUS,
    menu: menuEnUS
  }
};

export default messages;
```

### 模块语言文件示例
```typescript
// admin/zh-cn/common.ts
export default {
  title: '管理后台',
  welcome: '欢迎使用',
  logout: '退出登录'
};

// admin/zh-cn/menu.ts
export default {
  dashboard: '仪表盘',
  users: '用户管理',
  settings: '系统设置'
};

// admin/zh-cn/index.ts
import common from './common';
import menu from './menu';

export default {
  common,
  menu
};
```

### 语言文件格式
```typescript
// zh-CN.ts
export default {
  common: {
    // 公共翻译
  },
  menu: {
    // 菜单翻译
  },
  form: {
    // 表单相关
  }
};

// modules/admin/zh-CN.ts
export default {
  title: '管理后台',
  dashboard: {
    // 仪表盘翻译
  },
  settings: {
    // 设置翻译
  }
};
```

## 2. 键值命名规范

### 命名规则
- 使用点号(.)分隔的命名空间
- 模块名作为顶级命名空间
- 使用小写字母和连字符(-)
- 保持层级不超过4层

```typescript
// 推荐的键值格式
{
  "common": {
    "buttons": {
      "save": "保存",
      "cancel": "取消",
      "confirm": "确认"
    },
    "messages": {
      "success": "操作成功",
      "error": "操作失败"
    }
  },
  "user": {
    "form": {
      "username": "用户名",
      "password": "密码"
    },
    "messages": {
      "login-success": "登录成功",
      "login-error": "登录失败"
    }
  }
}
```

### 模块划分
- common: 公共翻译
- menu: 菜单和路由
- components: 组件相关
- validation: 表单验证
- messages: 提示消息
- pages: 页面特定文案

## 3. 语言包组织

### 导出格式
```typescript
// locales/zh-CN/index.ts
import common from './common';
import menu from './menu';
import user from './user';

export default {
  ...common,
  ...menu,
  ...user
};

// locales/index.ts
import zhCN from './zh-CN';
import enUS from './en-US';

export default {
  'zh-CN': zhCN,
  'en-US': enUS
};
```

## 4. 与 Ant Design Vue 集成

### 组件语言配置
```typescript
import { ConfigProvider } from 'ant-design-vue';
import zhCN from 'ant-design-vue/es/locale/zh_CN';
import enUS from 'ant-design-vue/es/locale/en_US';

const locales = {
  'zh-CN': zhCN,
  'en-US': enUS
};

// 在根组件中配置
<template>
  <a-config-provider :locale="locales[currentLocale]">
    <app />
  </a-config-provider>
</template>
```

### 日期和数字格式化
```typescript
// 日期格式化配置
const dateFormats = {
  'zh-CN': {
    short: 'YYYY-MM-DD',
    long: 'YYYY年MM月DD日 HH:mm:ss'
  },
  'en-US': {
    short: 'MM/DD/YYYY',
    long: 'MMMM DD, YYYY HH:mm:ss'
  }
};

// 数字格式化配置
const numberFormats = {
  'zh-CN': {
    currency: {
      style: 'currency',
      currency: 'CNY'
    }
  },
  'en-US': {
    currency: {
      style: 'currency',
      currency: 'USD'
    }
  }
};
```

## 5. 请求规范

### 请求头设置
```typescript
// 在请求拦截器中设置语言
axios.interceptors.request.use(config => {
  config.headers['Accept-Language'] = getCurrentLanguage();
  return config;
});
```

### 响应处理
```typescript
// 处理后端返回的多语言消息
axios.interceptors.response.use(
  response => {
    const { message, messageKey } = response.data;
    // 优先使用前端翻译
    if (messageKey) {
      return {
        ...response.data,
        message: i18n.t(messageKey)
      };
    }
    return response.data;
  }
);
```

## 6. 使用规范

### 在组件中使用
```vue
<template>
  <div>
    <!-- 基础用法 -->
    <span>{{ t('common.title') }}</span>
    
    <!-- 带参数翻译 -->
    <span>{{ t('common.welcome', { name: username }) }}</span>
    
    <!-- 复数翻译 -->
    <span>{{ tc('common.items', count) }}</span>
  </div>
</template>

<script setup lang="ts">
import { useI18n } from 'vue-i18n';

const { t, tc } = useI18n();
</script>
```

### 在 JS/TS 中使用
```typescript
import i18n from '@/locales';

// 直接使用
const message = i18n.global.t('common.messages.success');

// 在函数中使用
function showSuccess() {
  message.success(i18n.global.t('common.messages.save-success'));
}
```

## 7. 翻译管理

### 翻译文件格式
```typescript
// common.ts
export default {
  common: {
    buttons: {
      save: '保存',
      cancel: '取消'
    },
    messages: {
      success: '操作成功',
      error: '操作失败'
    }
  }
} as const; // 使用 as const 确保类型安全
```

### 类型支持
```typescript
// types/i18n.d.ts
import { DefineLocaleMessage } from 'vue-i18n';

declare module 'vue-i18n' {
  export interface DefineLocaleMessage {
    common: {
      buttons: {
        save: string;
        cancel: string;
      };
      messages: {
        success: string;
        error: string;
      };
    };
  }
}
```

## 8. 最佳实践

1. 使用 TypeScript 确保类型安全
2. 保持键值结构清晰和一致
3. 避免硬编码文本
4. 使用命名空间避免键值冲突
5. 定期同步和更新翻译内容
6. 提供默认语言回退机制
7. 考虑添加语言切换功能
8. 使用 i18n-ally 插件辅助开发 

## 9. 动态翻译管理

### 数据库存储结构
```sql
CREATE TABLE i18n_translations (
    id          BIGINT PRIMARY KEY,
    module      VARCHAR(50),    -- 模块名称
    lang        VARCHAR(10),    -- 语言代码
    key         VARCHAR(100),   -- 翻译键
    value       TEXT,          -- 翻译值
    created_at  DATETIME,      -- 创建时间
    updated_at  DATETIME       -- 更新时间
);
```

### 动态加载机制
```typescript
// i18n/dynamic.ts
import { i18n } from './index';
import { fetchTranslations } from '@/api/i18n';

// 动态加载翻译
export async function loadDynamicTranslations(lang: string, module?: string) {
  try {
    // 从后端获取翻译数据
    const translations = await fetchTranslations(lang, module);
    
    // 合并到现有翻译中
    i18n.global.mergeLocaleMessage(lang, translations);
    
    return true;
  } catch (error) {
    console.error('Failed to load translations:', error);
    return false;
  }
}

// 定期更新翻译
export function setupDynamicTranslations(interval = 5 * 60 * 1000) {
  // 初始加载
  loadDynamicTranslations(i18n.global.locale.value);
  
  // 定期刷新
  setInterval(() => {
    loadDynamicTranslations(i18n.global.locale.value);
  }, interval);
}
```

### 缓存策略
```typescript
// i18n/cache.ts
const CACHE_KEY = 'i18n_translations';
const CACHE_EXPIRY = 24 * 60 * 60 * 1000; // 24小时

// 缓存翻译数据
export function cacheTranslations(lang: string, translations: Record<string, any>) {
  const cache = {
    timestamp: Date.now(),
    data: translations
  };
  localStorage.setItem(`${CACHE_KEY}_${lang}`, JSON.stringify(cache));
}

// 获取缓存的翻译
export function getCachedTranslations(lang: string) {
  const cached = localStorage.getItem(`${CACHE_KEY}_${lang}`);
  if (!cached) return null;
  
  const { timestamp, data } = JSON.parse(cached);
  if (Date.now() - timestamp > CACHE_EXPIRY) {
    localStorage.removeItem(`${CACHE_KEY}_${lang}`);
    return null;
  }
  
  return data;
}
```

### 使用示例
```typescript
// main.ts
import { createI18n } from 'vue-i18n';
import { setupDynamicTranslations } from './i18n/dynamic';
import { getCachedTranslations } from './i18n/cache';

// 创建 i18n 实例
const i18n = createI18n({
  legacy: false,
  locale: 'zh-CN',
  fallbackLocale: 'en-US',
  messages: {
    'zh-CN': getCachedTranslations('zh-CN') || {}, // 优先使用缓存
    'en-US': getCachedTranslations('en-US') || {}
  }
});

// 设置动态加载
setupDynamicTranslations();

// 切换语言时加载动态翻译
watch(
  () => i18n.global.locale.value,
  (newLang) => {
    loadDynamicTranslations(newLang);
  }
);
```

### API 接口设计
```typescript
// api/i18n.ts
import request from '@/utils/request';

// 获取翻译数据
export function fetchTranslations(lang: string, module?: string) {
  return request({
    url: '/api/i18n/translations',
    method: 'GET',
    params: {
      lang,
      module
    }
  });
}

// 更新翻译
export function updateTranslation(data: {
  lang: string;
  key: string;
  value: string;
  module?: string;
}) {
  return request({
    url: '/api/i18n/translations',
    method: 'POST',
    data
  });
}
```

### 性能优化建议
1. 按模块懒加载翻译数据
2. 使用本地缓存减少请求
3. 增量更新而不是全量加载
4. 设置合理的刷新间隔
5. 添加加载失败重试机制
6. 优先使用静态翻译文件
7. 仅动态加载易变内容
8. 添加加载失败重试机制
9. 优先使用静态翻译文件
10. 仅动态加载易变内容 

