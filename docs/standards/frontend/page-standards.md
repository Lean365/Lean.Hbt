# Vue 页面开发规范

## 1. 页面目录结构

### 基础目录规范
```
src/views/
├── dashboard/           # 仪表盘
│   ├── index.vue       # 主页面
│   ├── components/     # 页面级组件
│   └── composables/    # 页面级组合式函数
├── system/             # 系统管理
│   ├── user/          # 用户管理
│   ├── role/          # 角色管理
│   └── menu/          # 菜单管理
└── business/          # 业务页面
    ├── list/          # 列表页面
    ├── detail/        # 详情页面
    └── form/          # 表单页面
```

### 页面文件结构
```
PageName/
├── index.vue          # 页面主文件
├── components/        # 页面私有组件
├── composables/       # 页面私有组合式函数
├── types/            # 页面类型定义
└── style/            # 页面样式文件
```

## 2. Ant Design Vue 布局规范

### 基础布局
```vue
<template>
  <a-layout>
    <a-layout-header>
      <global-header />
    </a-layout-header>
    <a-layout>
      <a-layout-sider>
        <side-menu />
      </a-layout-sider>
      <a-layout-content>
        <page-container>
          <!-- 页面内容 -->
        </page-container>
      </a-layout-content>
    </a-layout>
  </a-layout>
</template>
```

### 页面容器
```vue
<template>
  <page-container
    :title="pageTitle"
    :breadcrumb="{ routes }"
    :extra="pageExtra"
  >
    <template #extraContent>
      <page-extra-content />
    </template>
    <a-card>
      <!-- 页面主要内容 -->
    </a-card>
  </page-container>
</template>
```

## 3. 主题适配规范

### 主题变量使用
```less
// style/variables.less
@import 'ant-design-vue/es/style/themes/default.less';

// 使用 Ant Design Vue 主题变量
.page-container {
  background-color: @layout-body-background;
  
  .page-header {
    color: @heading-color;
    font-size: @font-size-lg;
  }
  
  .page-content {
    padding: @padding-lg;
  }
}
```

### 响应式设计
```vue
<template>
  <a-row :gutter="[16, 16]">
    <a-col :xs="24" :sm="12" :md="8" :lg="6">
      <a-card><!-- 内容 --></a-card>
    </a-col>
  </a-row>
</template>
```

## 4. 页面组件规范

### 列表页面
```vue
<template>
  <page-container>
    <!-- 搜索表单 -->
    <search-form
      :loading="loading"
      @search="handleSearch"
      @reset="handleReset"
    />
    
    <!-- 数据表格 -->
    <a-table
      :columns="columns"
      :data-source="dataSource"
      :pagination="pagination"
      :loading="loading"
      @change="handleTableChange"
    >
      <!-- 自定义列 -->
      <template #action="{ record }">
        <table-actions :record="record" />
      </template>
    </a-table>
  </page-container>
</template>
```

### 表单页面
```vue
<template>
  <page-container>
    <a-form
      :model="formState"
      :rules="rules"
      layout="vertical"
    >
      <a-row :gutter="24">
        <a-col :span="8">
          <a-form-item label="字段1" name="field1">
            <a-input v-model:value="formState.field1" />
          </a-form-item>
        </a-col>
      </a-row>
      
      <!-- 表单操作 -->
      <form-actions
        :loading="loading"
        @submit="handleSubmit"
        @cancel="handleCancel"
      />
    </a-form>
  </page-container>
</template>
```

## 5. 页面逻辑组织

### 组合式API使用
```typescript
// composables/usePageData.ts
export function usePageData() {
  // 页面状态
  const loading = ref(false);
  const dataSource = ref([]);
  const pagination = reactive({
    current: 1,
    pageSize: 10,
    total: 0
  });

  // 加载数据
  const loadData = async (params: any) => {
    loading.value = true;
    try {
      const { data, total } = await fetchData(params);
      dataSource.value = data;
      pagination.total = total;
    } finally {
      loading.value = false;
    }
  };

  return {
    loading,
    dataSource,
    pagination,
    loadData
  };
}
```

### 页面状态管理
```typescript
// 页面状态
const pageState = reactive({
  selectedKeys: [],
  searchParams: {},
  sortInfo: {},
  filterInfo: {}
});

// 状态持久化
watch(
  () => pageState,
  (val) => {
    localStorage.setItem('pageState', JSON.stringify(val));
  },
  { deep: true }
);
```

## 6. 页面性能优化

### 组件懒加载
```typescript
// 异步组件
const AsyncComponent = defineAsyncComponent(() =>
  import('./components/HeavyComponent.vue')
);

// 路由懒加载
const routes = [
  {
    path: '/dashboard',
    component: () => import('./views/dashboard/index.vue')
  }
];
```

### 虚拟滚动
```vue
<template>
  <a-list>
    <virtual-list
      :data-source="list"
      :item-height="48"
      :container-height="400"
    >
      <template #item="{ item }">
        <a-list-item>{{ item.title }}</a-list-item>
      </template>
    </virtual-list>
  </a-list>
</template>
```

## 7. 页面交互规范

### 加载状态
```vue
<template>
  <a-spin :spinning="loading">
    <page-content />
  </a-spin>
</template>
```

### 错误处理
```typescript
const handleError = (error: Error) => {
  message.error({
    content: error.message,
    duration: 3
  });
};
```

### 操作反馈
```typescript
const handleSuccess = () => {
  message.success('操作成功');
  // 刷新数据
  loadData();
};
```

## 8. 页面权限控制

### 路由权限
```typescript
import { usePermission } from '@/hooks/usePermission';

const { hasPermission } = usePermission();

// 路由守卫
router.beforeEach((to, from, next) => {
  if (to.meta.permission && !hasPermission(to.meta.permission)) {
    next('/403');
  } else {
    next();
  }
});
```

### 组件权限
```vue
<template>
  <a-button
    v-if="hasPermission('user:create')"
    type="primary"
    @click="handleCreate"
  >
    新建用户
  </a-button>
</template>
```

## 9. 页面国际化

### 文本国际化
```typescript
import { useI18n } from 'vue-i18n';

// 在组件中使用
const { t } = useI18n();

const columns = [
  {
    title: t('table.username'),
    dataIndex: 'username'
  }
];
```

### 日期国际化
```vue
<template>
  <a-date-picker
    v-model:value="date"
    :locale="locale"
    format="YYYY-MM-DD"
  />
</template>
```

## 10. 页面测试规范

### 单元测试
```typescript
import { mount } from '@vue/test-utils';
import UserList from './UserList.vue';

describe('UserList.vue', () => {
  it('renders list items', async () => {
    const wrapper = mount(UserList);
    await wrapper.vm.$nextTick();
    
    expect(wrapper.findAll('.user-item')).toHaveLength(10);
  });
});
```

### E2E测试
```typescript
describe('User List Page', () => {
  it('should display user list', () => {
    cy.visit('/users');
    cy.get('.user-table').should('be.visible');
    cy.get('.user-item').should('have.length', 10);
  });
});
```
