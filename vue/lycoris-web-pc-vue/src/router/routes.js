import { webSettings } from '../config.json';

const routes = [
  {
    path: '/layout',
    name: 'layout',
    redirect: '/',
    component: () => import('../views/layout/index.vue'),
    children: [
      {
        path: '/',
        name: 'home',
        component: () => import('../views/home/index.vue'),
        meta: {
          keepAlive: true,
          pageName: '首页',
          navHeader: true
        }
      },
      {
        path: '/post/:postid',
        name: 'post',
        component: () => import('../views/post/index.vue'),
        meta: {
          keepAlive: false
        }
      },
      {
        path: '/talks',
        name: 'talks',
        component: () => import('../views/talks/index.vue'),
        meta: {
          keepAlive: true,
          pageName: '说说',
          navHeader: true
        }
      },
      {
        path: '/message',
        name: 'message',
        component: () => import('../views/message/index.vue'),
        meta: {
          keepAlive: true,
          pageName: '留言',
          navHeader: true
        }
      },
      {
        path: '/navigation',
        name: 'navigation',
        component: () => import('../views/navigation/index.vue'),
        meta: {
          keepAlive: true,
          pageName: '网站收录',
          navHeader: true,
          dropdown: '其他'
        }
      },
      {
        path: '/cooperate',
        name: 'cooperate',
        component: () => import('../views/cooperate/index.vue'),
        meta: {
          keepAlive: true,
          pageName: '开发合作',
          navHeader: true,
          dropdown: '其他'
        }
      },
      {
        path: '/other',
        name: 'other',
        component: () => import('../views/other/index.vue'),
        meta: {
          keepAlive: true,
          pageName: '其他',
          navHeader: true,
          dropdown: '其他'
        }
      },
      {
        path: '/friends',
        name: 'friends',
        component: () => import('../views/friendLink/index.vue'),
        meta: {
          keepAlive: true,
          pageName: '友情链接',
          navHeader: true
        }
      },
      {
        path: '/about/web',
        name: 'aboutweb',
        component: () => import('../views/aboutWeb/index.vue'),
        meta: {
          keepAlive: true,
          pageName: '关于本站',
          navHeader: true
        }
      },
      {
        path: '/other/tool/tesseract',
        name: 'tesseract',
        component: () => import('../views/tesseract/index.vue'),
        meta: {
          keepAlive: true
        }
      },
      {
        path: '/user',
        name: 'user',
        component: () => import('../views/user/index.vue'),
        meta: {
          pageName: '个人中心',
          autuorize: true
        }
      }
    ]
  },
  {
    path: '/about/me',
    name: 'aboutme',
    component: () => import('../views/aboutMe/index.vue'),
    meta: {
      title: '关于我',
      keepAlive: true,
      navHeader: true
    }
  },
  {
    path: '/server/notfound',
    name: 'notfound',
    component: () => import('../views/server-notfound.vue'),
    meta: {
      title: '小破站资源丢失啦',
      keepAlive: false
    }
  },
  {
    path: '/server/error',
    name: 'server-error',
    component: () => import('../views/server-error.vue'),
    meta: {
      title: '小破站服务崩溃啦',
      keepAlive: false
    }
  },
  {
    path: '/navigation/jump',
    name: 'nav-jump',
    component: () => import('../views/nav-jump.vue'),
    meta: {
      title: '跳转说明',
      keepAlive: false
    }
  }
];

for (let prop in webSettings.otherPage) {
  if (!webSettings.otherPage[prop]) {
    let index = routes[0].children.findIndex(x => x.name == prop);
    if (index > -1) {
      routes[0].children.splice(index, 1);
    }
  }
}

const metaChange = (data, source) => {
  if (source.meta && Object.keys(source.meta).length > 0) {
    data.meta = data.meta || {};
    for (let key in source.meta) {
      if (key === 'title' || key === 'keepAlive') {
        data.meta[key] = source.meta[key];
      } else if (key === 'pageName' && !data.meta['title']) {
        data.meta[key] = source.meta[key];
      } else if (key === 'type' && !data.meta['type']) {
        data.meta[key] = 'pc';
      } else {
        data.meta['autuorize'] = Object.keys(source.meta).filter(x => x == 'autuorize').length > 0 ? source.meta['autuorize'] : false;
      }
    }
  }
};

export const getPcPageRoutes = () => {
  let config = [];

  for (let item of routes) {
    let route = {
      path: item.path,
      name: item.name,
      component: item.component
    };

    metaChange(route, item);

    if (item.children && item.children.length) {
      route.children = [];

      for (let child of item.children) {
        let children = {
          path: child.path,
          name: child.name,
          component: child.component
        };

        metaChange(children, child);

        route.children.push(children);
      }
    }

    config.push(route);
  }

  return config;
};

export const getPcKeepAliveComponents = () => {
  let compoents = [];

  for (let item of routes) {
    if (item.meta && item.meta.keepAlive) {
      compoents.push(item.name);
    }

    if (item.children && item.children.length) {
      for (let child of item.children) {
        if (child.meta && child.meta.keepAlive) {
          compoents.push(child.name);
        }
      }
    }
  }

  return compoents;
};

export const getPcNavMenus = childrens => {
  let nav = [];

  for (let item of childrens || routes) {
    if (item.children && item.children.length) {
      nav = [...nav, ...getPcNavMenus(item.children)];
    } else if (item.meta.navHeader) {
      if (item.meta.dropdown) {
        //
        const index = nav.findIndex(x => x.name == item.meta.dropdown);
        if (index == -1) {
          nav.push({
            name: item.meta.dropdown,
            dropdown: true,
            dropdownItem: [
              {
                path: item.name,
                name: item.meta.title || item.meta.pageName
              }
            ]
          });
        } else {
          nav[index].dropdownItem.push({
            path: item.name,
            name: item.meta.title || item.meta.pageName
          });
        }
      } else {
        nav.push({
          path: item.name,
          name: item.meta.title || item.meta.pageName
        });
      }
    }
  }
  return nav;
};
