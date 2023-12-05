import { createRouter, createWebHistory } from 'vue-router';
import { pageRoutes, getMenus } from './routes';
import nProgress from 'nprogress';
import { web } from '../config.json';
import { stores } from '../stores';

nProgress.configure({
  easing: 'ease',
  speed: 500,
  showSpinner: false,
  trickleSpeed: 200,
  minimum: 0.3,
  positionUsing: 'translate'
});

const routerconfig = {
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    ...pageRoutes,
    {
      path: '/server/error/:code',
      name: 'server-error',
      component: () => import('../views/server-error.vue'),
      meta: {
        title: '小破站崩溃啦'
      }
    }
  ],
  scrollBehavior(to, from, savedPosition) {
    if (savedPosition) {
      return savedPosition;
    } else {
      return { top: 0, behavior: 'smooth' };
    }
  }
};

const $router = createRouter(routerconfig);

$router.beforeEach(async to => {
  nProgress.start();

  if (to.path == '' || to.path == '/') {
    return stores.authorize.token ? { name: 'dashboard' } : { name: 'login', query: to.query };
  }

  if (to.matched.length == 0) {
    return { name: 'server-error', params: { code: 404 } };
  }

  if (!['login', 'server-error', 'screen-lock', 'resume'].includes(to.name)) {
    if (!document.title.includes('管理后台')) {
      document.title = `管理后台_${stores.webSetting.webName}`;
    }

    // if (stores.authorize.token && stores.screenLock.checkLossOfActivity()) {
    //   return { name: 'screen-lock', query: { path: to.path } };
    // }
  } else {
    if (to.meta.title) {
      document.title = `${to.meta.title}_${stores.webSetting.webName}`;
    } else {
      document.title = stores.webSetting.webName;
    }
  }
});

$router.afterEach(() => {
  nProgress.done();
});

export const router = $router;

export const menus = getMenus();
