import { dashboardUrlPrefix } from '../config.json';

const menusRoutes = [
  {
    isRoute: true,
    path: `${dashboardUrlPrefix}/dashboard`,
    name: 'dashboard',
    component: () => import('../views/index/index.vue'),
    meta: {
      title: '首页',
      keepAlive: true,
      icon: 'element-plus'
    }
  },
  {
    isRoute: false,
    title: '文章管理',
    icon: 'document',
    routes: [
      {
        path: `${dashboardUrlPrefix}/post`,
        name: 'post',
        component: () => import('../views/post/index.vue'),
        meta: {
          title: '文章列表'
        }
      },
      {
        path: `${dashboardUrlPrefix}/blog/markdown`,
        name: 'post-markdown',
        component: () => import('../views/post/createorupdate.vue'),
        meta: {
          title: '文章编写'
        }
      },
      {
        path: `${dashboardUrlPrefix}/blog/category`,
        name: 'category',
        component: () => import('../views/category/index.vue'),
        meta: {
          title: '文章分类',
          keepAlive: true
        }
      },
      {
        path: `${dashboardUrlPrefix}/blog/comment`,
        name: 'comment',
        component: () => import('../views/comment/index.vue'),
        meta: {
          title: '文章评论',
          keepAlive: true
        }
      }
    ]
  },
  {
    isRoute: false,
    title: '网站管理',
    icon: 'coin',
    routes: [
      {
        path: `${dashboardUrlPrefix}/talks`,
        name: 'talks',
        component: () => import('../views/talks/index.vue'),
        meta: {
          title: '说说管理',
          keepAlive: true
        }
      },
      {
        path: `${dashboardUrlPrefix}/message`,
        name: 'message',
        component: () => import('../views/leavemessage/index.vue'),
        meta: {
          title: '留言管理',
          keepAlive: true
        }
      },
      {
        path: `${dashboardUrlPrefix}/navigation`,
        name: 'navigation',
        component: () => import('../views/navigation/index.vue'),
        meta: {
          title: '网站收录',
          keepAlive: true
        }
      },
      {
        path: `${dashboardUrlPrefix}/friendlink`,
        name: 'friendlink',
        component: () => import('../views/friendlink/index.vue'),
        meta: {
          title: '友链管理',
          keepAlive: true
        }
      },
      {
        path: `${dashboardUrlPrefix}/file`,
        name: 'filemanage',
        component: () => import('../views/filemanage/index.vue'),
        meta: {
          title: '文件管理',
          keepAlive: false
        }
      }
    ]
  },
  {
    isRoute: false,
    title: '网站日志',
    icon: 'pie-chart',
    routes: [
      {
        path: `${dashboardUrlPrefix}/log/request`,
        name: 'statistics-request',
        component: () => import('../views/request-log/index.vue'),
        meta: {
          title: '请求日志',
          keepAlive: true
        }
      },
      {
        path: `${dashboardUrlPrefix}/log/browse`,
        name: 'statistics-browse',
        component: () => import('../views/browse-log/index.vue'),
        meta: {
          title: '浏览日志',
          keepAlive: true
        }
      },
      {
        path: `${dashboardUrlPrefix}/referer/statistics`,
        name: 'statistics-referer',
        component: () => import('../views/referer-statistics/index.vue'),
        meta: {
          title: '来源统计',
          keepAlive: true
        }
      }
    ]
  },
  {
    isRoute: false,
    title: '关 于',
    icon: 'connection',
    routes: [
      {
        isRoute: true,
        path: `${dashboardUrlPrefix}/about/web`,
        name: 'about-web',
        component: () => import('../views/about-web/index.vue'),
        meta: {
          title: '关于本站',
          keepAlive: true,
          icon: 'chrome-filled',
          autuorize: true
        }
      },
      {
        isRoute: true,
        path: `${dashboardUrlPrefix}/about/me`,
        name: 'about-me',
        component: () => import('../views/about-me/index.vue'),
        meta: {
          title: '关于我',
          keepAlive: true,
          icon: 'avatar',
          autuorize: true
        }
      }
    ]
  },
  {
    isRoute: false,
    title: '系统管理',
    icon: 'setting',
    routes: [
      {
        path: `${dashboardUrlPrefix}/user`,
        name: 'user',
        component: () => import('../views/user/index.vue'),
        meta: {
          title: '用户管理',
          autuorize: true,
          keepAlive: true
        }
      },
      {
        isRoute: true,
        path: `${dashboardUrlPrefix}/access/control`,
        name: 'accesscontrol',
        component: () => import('../views/accesscontrol/index.vue'),
        meta: {
          title: '访问管控',
          autuorize: true,
          keepAlive: true
        }
      },
      {
        isRoute: true,
        path: `${dashboardUrlPrefix}/system/settings`,
        name: 'settings',
        component: () => import('../views/system/index.vue'),
        meta: {
          title: '系统设置',
          autuorize: true,
          keepAlive: true
        }
      }
    ]
  }
];

const getPageRoutes = () => {
  let routes = [];
  for (let item of menusRoutes) {
    if (item.isRoute) {
      routes.push({
        path: item.path,
        name: item.name,
        component: item.component,
        meta: item.meta
      });
    } else {
      for (let children of item.routes) {
        routes.push({
          path: children.path,
          name: children.name,
          component: children.component,
          meta: children.meta
        });
      }
    }
  }

  return routes;
};

export const pageRoutes = [
  {
    path: `${dashboardUrlPrefix}`,
    name: 'backstage-layout',
    component: () => import('../views/layout/index.vue'),
    redirect: `${dashboardUrlPrefix}/dashboard`,
    meta: {
      title: '',
      keepAlive: false,
      icon: 'home-outlined'
    },
    children: [...getPageRoutes()]
  },
  {
    path: `${dashboardUrlPrefix}/login`,
    name: 'login',
    component: () => import('../views/login/index.vue'),
    meta: {
      keepAlive: false,
      title: '登录'
    }
  },
  {
    path: `${dashboardUrlPrefix}/screen/lock`,
    name: 'screen-lock',
    component: () => import('../views/layout/screen-lock.vue'),
    meta: {
      keepAlive: false,
      title: '锁屏'
    }
  }
];

export const getMenus = () => {
  let menus = [];
  for (let item of menusRoutes) {
    if (item.isRoute) {
      menus.push({
        name: item.meta.title,
        icon: item.meta.icon,
        path: item.path,
        component: {
          keepAlive: item.meta.keepAlive || false,
          name: item.name
        }
      });
    } else {
      let menu = {
        name: item.title,
        icon: item.icon,
        routes: []
      };

      for (let children of item.routes) {
        menu.routes.push({
          name: children.meta.title,
          path: children.path,
          component: {
            keepAlive: children.meta.keepAlive || false,
            name: children.name
          }
        });
      }

      menus.push(menu);
    }
  }

  return menus;
};
