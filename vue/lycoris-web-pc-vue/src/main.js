import { createApp } from 'vue';
import { pinia } from './stores';
import elementPlus from 'element-plus';
import * as ElementPlusIconsVue from '@element-plus/icons-vue';
import './assets/plugins/nprogress/nprogress.css';
import directives from './directives';

import App from './App.vue';
import { router } from './router';

import './assets/main.css';
import './utils/extensions';

const app = createApp(App);

app.use(elementPlus);

app.use(pinia);

app.use(router);

for (const [key, component] of Object.entries(ElementPlusIconsVue)) {
  app.component(key, component);
}

// 自定义指令
app.use(directives);

router.isReady().then(() => {
  app.mount('#app');
});
