import { createApp } from 'vue'
import { createPinia } from 'pinia'
import Antd from 'ant-design-vue'
import 'ant-design-vue/dist/reset.css'
import App from './App.vue'
import router from './router'
import i18n from './locales'

import './assets/styles/index.less'

const app = createApp(App)

// 按照正确的顺序注册插件
app.use(createPinia())
app.use(router)
app.use(i18n)
app.use(Antd)

app.mount('#app') 