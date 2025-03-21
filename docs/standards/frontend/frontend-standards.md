# 前端开发规范

## 1. 项目结构规范

### 目录结构
```
src/
├── api/                # API接口目录
├── assets/            # 静态资源目录
│   ├── icons/         # 图标文件
│   ├── images/        # 图片文件
│   └── styles/        # 样式文件
├── components/        # 公共组件目录
├── hooks/             # 自定义Hook目录
├── layouts/           # 布局组件目录
├── locales/           # 国际化资源目录
├── router/            # 路由配置目录
├── store/             # 状态管理目录
├── utils/             # 工具函数目录
└── views/             # 页面组件目录
```

## 2. 命名规范

### 文件命名
- 组件文件：使用PascalCase
  - `UserList.vue`
  - `LoginForm.vue`
  - `HeaderNavigation.vue`
- 工具文件：使用camelCase
  - `request.ts`
  - `auth.ts`
  - `dateFormat.ts`
- 样式文件：使用kebab-case
  - `main-layout.less`
  - `user-profile.scss`
  - `common-variables.css`

### 组件命名
- 组件名使用PascalCase
```typescript
export default {
  name: 'UserProfile',
  // ...
}
```

### 变量命名
- 普通变量：使用camelCase
```typescript
const userName = 'admin'
const userRole = ['admin', 'user']
```
- 常量：使用UPPER_SNAKE_CASE
```typescript
const API_BASE_URL = '/api'
const MAX_FILE_SIZE = 5 * 1024 * 1024
```
- 私有变量：使用_前缀
```typescript
const _privateMethod = () => {}
```

### Props命名
- 使用camelCase
```typescript
props: {
  userInfo: {
    type: Object,
    required: true
  },
  isLoading: {
    type: Boolean,
    default: false
  }
}
```

### 事件命名
- 使用kebab-case
```typescript
this.$emit('item-click', id)
this.$emit('status-change', status)
```

## 3. API接口规范

### 接口文件组织
```typescript
// api/user.ts
export const userApi = {
  login: (data: LoginParams) => request.post('/auth/login', data),
  getUserInfo: () => request.get('/user/info'),
  updateUser: (data: UserInfo) => request.put('/user/update', data)
}
```

### 接口命名
- 获取数据：getXxx
- 创建数据：createXxx
- 更新数据：updateXxx
- 删除数据：deleteXxx
- 批量操作：batchXxx

## 4. 样式规范

### 类名命名
- 使用BEM命名规范
```css
.block {}
.block__element {}
.block--modifier {}
```

### 样式文件组织
```less
// 变量
@primary-color: #1890ff;

// 混入
.text-ellipsis() {
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

// 组件样式
.user-card {
  &__header {
    .text-ellipsis();
  }
  
  &__content {
    // ...
  }
  
  &--active {
    // ...
  }
}
```

## 5. TypeScript规范

### 类型定义
- 接口名以I开头
```typescript
interface IUserInfo {
  id: number
  name: string
  role: string[]
}
```
- 类型别名以T开头
```typescript
type TStatus = 'pending' | 'success' | 'error'
```

### 枚举定义
```typescript
enum UserStatus {
  Active = 1,
  Inactive = 0
}
```

## 6. 代码格式化

### ESLint配置
```javascript
module.exports = {
  root: true,
  extends: [
    'plugin:vue/vue3-essential',
    '@vue/typescript/recommended'
  ],
  rules: {
    'no-console': process.env.NODE_ENV === 'production' ? 'warn' : 'off',
    'no-debugger': process.env.NODE_ENV === 'production' ? 'warn' : 'off'
  }
}
```

### Prettier配置
```json
{
  "semi": false,
  "singleQuote": true,
  "printWidth": 100,
  "trailingComma": "none",
  "arrowParens": "avoid"
}
```

## 7. Git提交规范

### 提交信息格式
```
<type>(<scope>): <subject>

<body>

<footer>
```

### 类型说明
- feat: 新功能
- fix: 修复Bug
- docs: 文档更新
- style: 代码格式调整
- refactor: 代码重构
- perf: 性能优化
- test: 测试相关
- chore: 构建过程或辅助工具的变动

## 8. 性能优化规范

### 组件优化
- 合理使用computed
- 适当使用v-show代替v-if
- 使用keep-alive缓存组件
- 避免深层组件嵌套

### 打包优化
- 路由懒加载
- 第三方库按需引入
- 图片资源压缩
- 开启gzip压缩

## 9. 安全规范

### XSS防护
- 输入过滤
- 输出转义
- 使用v-html需谨慎

### 敏感信息
- 禁止明文存储密码
- 使用https传输
- 及时清理本地存储
