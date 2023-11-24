import { createRouter, createWebHistory } from 'vue-router';
import { getPcPageRoutes, getPcKeepAliveComponents, getPcNavMenus } from './pc-routes';
import { getMobilePageRoutes, getMobileKeepAliveComponents, getMobileNavMenus } from './mobile-routes';
import nProgress from 'nprogress';
import { stores } from '../stores';
import { isMobile } from '../utils/tool';

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
  routes: []
  // scrollBehavior(to, from, savedPosition) {
  //   if (savedPosition) {
  //     return savedPosition;
  //   } else {
  //     return { top: 0, behavior: 'smooth' };
  //   }
  // }
};

const mobileNavigator = isMobile(navigator.userAgent);
if (mobileNavigator) {
  const routes = getMobilePageRoutes();
  routerconfig.routes = [...routes];
} else {
  const routes = getPcPageRoutes();
  routerconfig.routes = [...routes];
}

const $router = createRouter(routerconfig);

$router.beforeEach(async to => {
  nProgress.start();
  if (to.matched.length == 0) {
    return { name: 'notfound' };
  }

  if (to.meta.autuorize && (!stores.authorize.token || !stores.user.state)) {
    return { name: 'home' };
  }
});

$router.afterEach(() => {
  nProgress.done();
});

export const router = $router;

export const keepAliveComponents = mobileNavigator ? getMobileKeepAliveComponents() : getPcKeepAliveComponents();

export const navMenus = mobileNavigator ? getMobileNavMenus : getPcNavMenus();
