import toast from '../../utils/toast';
import { router } from '../../router';
import { stores } from '../../stores';
import { dashboardUrlPrefix } from '../../config.json';

const reject = (data, statusCode = 200) => {
  return Promise.reject({
    data,
    statusCode
  });
};

let routeToTime = void 0;

const toLoginPage = (
  call = () => {
    toast.error('登录过期,请重新登录', {
      max: 1
    });
  }
) => {
  if (location.pathname != `${dashboardUrlPrefix}/login`) {
    call();

    if (!routeToTime) {
      clearTimeout(routeToTime);
    }
    routeToTime = setTimeout(() => {
      router.push({
        name: 'login'
      });
    }, 1500);
  }

  stores.authorize.setUserLogoutState();
};

const refreshToken = service => {
  if (!stores.authorize.refreshToken) {
    toLoginPage();
    return reject(void 0, -22);
  }

  const config = {
    method: 'post',
    url: '/authentication/dashboard/refresh/token',
    data: {
      refreshToken: stores.authorize.refreshToken
    }
  };

  return service(config);
};

let isRefreshing = false; // 标记是否正在刷新令牌
let refreshSubscribers = []; // 用于存储等待刷新的请求

const success = (resp, service) => {
  if (!resp.data) {
    return reject({
      resCode: -99,
      resMsg: '系统异常,请稍候再试'
    });
  }

  if (resp.data.resCode == undefined || resp.data.resCode == null) {
    return reject({
      resCode: -99,
      resMsg: '响应异常'
    });
  }

  if (resp.data.resCode === -99) {
    return reject({
      resCode: -99,
      resMsg: resp.data.resMsg || '操作失败'
    });
  }

  if (resp.data.resCode == -21) {
    if (resp.config.url.includes('/authentication/sso/login')) {
      return Promise.resolve(resp);
    }

    const originalRequest = resp.config;

    if (!isRefreshing) {
      isRefreshing = true;
      console.log('--------------- refresh ---------------');
      console.log('isRefreshing = ' + isRefreshing);
      console.log(resp.config);
      console.log('--------------- refresh ---------------');
      return refreshToken(service)
        .then(res => {
          if (res.data.resCode != 0) {
            toLoginPage();
            return reject(void 0, -1);
          }

          stores.authorize.setUserLoginState(res.data.data);
          originalRequest.headers['X-Real-User'] = res.data.data.token;

          if (refreshSubscribers && refreshSubscribers.length) {
            refreshSubscribers.forEach(subscriber => subscriber(res.data.data.token));
          }

          return service(originalRequest);
        })
        .catch(err => {
          toLoginPage();
          return reject(err, err?.statusCode);
        })
        .finally(() => {
          isRefreshing = false;
          refreshSubscribers = [];
        });
    } else {
      // 已经在刷新令牌，将原始请求加入等待队列
      return new Promise(resolve => {
        refreshSubscribers.push(newToken => {
          originalRequest.headers['X-Real-User'] = newToken;
          resolve(service(originalRequest));
        });
      });
    }
  }

  return Promise.resolve(resp);
};

const error = err => {
  if (err && err.response) {
    // 刷新令牌返回错误的直接返回登录页
    if (err.config.url.includes('/authentication/refresh/token')) {
      toLoginPage();
      return reject(
        {
          err: err,
          response: err.response
        },
        err.response.status
      );
    }

    if (err.response.status == 400 || err.response.status == 401 || err.response.status == 403) {
      if (err.response.status == 400) {
        toLoginPage(
          toast.error('服务异常，系统将退出登录', {
            max: 1
          })
        );
      } else {
        toLoginPage();
      }
      return reject(
        {
          err: err,
          response: err.response
        },
        err.response.status
      );
    }

    switch (err.response.status) {
      case 404:
        err.message = '请求错误,未找到该资源';
        break;
      case 405:
        err.message = '您暂时没有权限访问';
        break;
      case 408:
        err.message = '网络连接异常，请求超时';
        break;
      case 500:
        err.message = '服务器繁忙，请稍后再试';
        break;
      case 502:
        err.message = '网络错误';
        break;
      case 503:
        err.message = '服务不可用';
        break;
      case 504:
        err.message = '网络连接异常，请求超时';
        break;
      default:
        err.message = '系统繁忙，请稍后再试';
    }

    toast.error(err.message, {
      max: 1
    });

    return reject(
      {
        err: err,
        response: err.response,
        messate: err.message
      },
      err.response.status
    );
  }

  toast.warn('服务器异常', {
    max: 1
  });

  router.push({
    name: 'server-error',
    params: {
      code: 500
    }
  });

  return reject(
    {
      err: err
    },
    0
  );
};

export default {
  success,
  error
};
