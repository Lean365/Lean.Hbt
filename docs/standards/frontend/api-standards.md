# API 接口规范

## 1. 基本规范

### 接口路径规范
- 使用 RESTful 风格的 API 设计
- 使用小写字母，单词间用连字符（-）分隔
- 使用复数形式表示资源集合

```typescript
// 推荐
/api/users
/api/user-roles
/api/auth/login

// 避免
/api/User
/api/getUserList
/api/user_roles
```

### HTTP 方法使用
- GET：获取资源
- POST：创建资源
- PUT：更新资源（全量更新）
- PATCH：更新资源（部分更新）
- DELETE：删除资源

## 2. 请求规范

### 请求参数
- GET 请求使用 URL 参数
- POST/PUT/PATCH 请求使用 JSON 格式的请求体
- 文件上传使用 multipart/form-data

```typescript
// GET 请求示例
/api/users?page=1&pageSize=10&status=active

// POST 请求示例
{
  "username": "admin",
  "email": "admin@example.com",
  "role": "admin"
}
```

### 请求头规范
```typescript
{
  'Content-Type': 'application/json',
  'Authorization': 'Bearer ${token}',
  'Accept-Language': 'zh-CN'
}
```

## 3. 响应规范

### 响应格式
```typescript
{
  "code": 200,          // 业务状态码
  "message": "success", // 状态描述
  "data": {            // 响应数据
    // ...
  }
}
```

### 状态码使用
- 200：成功
- 201：创建成功
- 400：请求参数错误
- 401：未授权
- 403：禁止访问
- 404：资源不存在
- 500：服务器错误

## 4. 与 Ant Design Vue 集成

### 请求封装
```typescript
import { message } from 'ant-design-vue';
import axios from 'axios';

// 创建 axios 实例
const request = axios.create({
  baseURL: '/api',
  timeout: 10000
});

// 响应拦截器
request.interceptors.response.use(
  response => {
    const { code, message: msg, data } = response.data;
    if (code === 200) {
      return data;
    }
    message.error(msg);
    return Promise.reject(new Error(msg));
  },
  error => {
    message.error(error.message);
    return Promise.reject(error);
  }
);
```

### 与组件集成
```typescript
import { ref } from 'vue';
import { message } from 'ant-design-vue';

// 表格数据加载
const loading = ref(false);
const tableData = ref([]);

const loadData = async (params: any) => {
  loading.value = true;
  try {
    const data = await request.get('/users', { params });
    tableData.value = data.list;
  } catch (error) {
    message.error('加载数据失败');
  } finally {
    loading.value = false;
  }
};
```

## 5. 错误处理规范

### 错误响应格式
```typescript
{
  "code": 400,
  "message": "参数错误",
  "errors": [
    {
      "field": "username",
      "message": "用户名不能为空"
    }
  ]
}
```

### 错误处理方式
```typescript
import { message, Modal } from 'ant-design-vue';

const handleError = (error: any) => {
  if (error.response) {
    const { status } = error.response;
    switch (status) {
      case 401:
        // 跳转到登录页
        break;
      case 403:
        Modal.error({
          title: '无权访问',
          content: '您没有权限执行此操作'
        });
        break;
      default:
        message.error(error.message);
    }
  }
};
```

## 6. 接口文档规范

### 文档格式
```typescript
/**
 * 用户登录
 * @url POST /api/auth/login
 * @param {object} data
 * @param {string} data.username - 用户名
 * @param {string} data.password - 密码
 * @returns {Promise<{token: string}>} 登录成功返回token
 */
```

### Swagger/OpenAPI 规范
- 使用 Swagger/OpenAPI 编写接口文档
- 保持文档与代码的同步更新
- 提供在线调试功能

## 7. 安全规范

### 身份认证
- 使用 JWT Token
- Token 在请求头中传递
- 敏感接口使用 HTTPS

### 数据安全
- 密码等敏感数据加密传输
- 防止 XSS 和 CSRF 攻击
- 接口访问频率限制

## 8. 版本控制

### 版本号规范
```typescript
// URL 中的版本号
/api/v1/users

// 请求头中的版本号
'Api-Version': '1.0'
```

### 兼容性处理
- 保持向下兼容
- 重大变更使用新的版本号
- 提供版本升级指南

## 9. 性能优化

### 请求优化
- 合理使用缓存
- 减少不必要的请求
- 压缩响应数据

### 数据处理
- 分页加载
- 按需加载
- 数据预加载

## 10. 测试规范

### 接口测试
- 单元测试
- 集成测试
- 性能测试

### 测试用例
```typescript
describe('用户接口测试', () => {
  it('登录成功', async () => {
    const response = await request.post('/auth/login', {
      username: 'admin',
      password: '123456'
    });
    expect(response.code).toBe(200);
    expect(response.data.token).toBeDefined();
  });
});
```
