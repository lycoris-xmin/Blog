import { createRouter, createWebHistory } from 'vue-router';
import { getPageRoutes, getkeepAliveComponents, getNavMenus } from './routes';
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

const routes = getPageRoutes();

const routerconfig = {
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [...routes]
  // scrollBehavior(to, from, savedPosition) {
  //   if (savedPosition) {
  //     return savedPosition;
  //   } else {
  //     return { top: 0, behavior: 'smooth' };
  //   }
  // }
};

const $router = createRouter(routerconfig);

$router.beforeEach(async to => {
  nProgress.start();
  if (to.matched.length == 0) {
    return { name: 'notfound' };
  }

  if (!to.path.startsWith('/post')) {
    if (to.meta.title) {
      if (!document.title.includes(to.meta.title)) {
        document.title = `${to.meta.title}_${web.name}`;
      }
    } else {
      document.title = web.name;
    }
  }

  if (to.meta.autuorize && (!stores.authorize.token || !stores.user.state)) {
    return { name: 'home' };
  }
});

$router.afterEach(() => {
  nProgress.done();
});

export const router = $router;

export const keepAliveComponents = getkeepAliveComponents();

export const navMenus = getNavMenus();
