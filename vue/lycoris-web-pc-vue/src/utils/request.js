import axios from 'axios';
import { api } from '../config.json';
import { stores } from '../stores';
import toast from './toast';

const service = axios.create({
  baseURL: `${api.server}${api.routePrefix}`,
  timeout: api.timeout,
  withCredentials: true
});

const handleRequestReject = err => {
  if (err.statusCode == 200 && err.data) {
    if (err.data.resCode == -99) {
      toast.warn(err.data.resMsg);
    }
  }
};

const get = async (url, data, config = void 0) => {
  const _config = {
    method: 'get',
    url: url,
    headers: {
      'X-Real-Request': new Date().getTime()
    }
  };

  if (config) {
    Object.assign(_config, config);
  }

  if (stores.authorize.token && (!('X-Real-User' in _config.headers) || !_config.headers['X-Real-User'])) {
    _config.headers['X-Real-User'] = stores.authorize.token;
  }

  if (data) {
    _config.params = data;
  }

  let resp = await service(_config).catch(err => {
    handleRequestReject(err);
    throw err;
  });

  return resp ? resp.data : {};
};

const post = async (url, data, fileUpload = false, config = void 0) => {
  if (!config) {
    config = {};
  }

  if (!config.headers) {
    config.headers = {};
  }

  config.headers['X-Real-Request'] = new Date().getTime();

  if (stores.authorize.token && (!('X-Real-User' in config.headers) || !config.headers['X-Real-User'])) {
    config.headers['X-Real-User'] = stores.authorize.token;
  }

  let resp = void 0;

  if (fileUpload) {
    resp = await service.postForm(url, data, config).catch(err => {
      handleRequestReject(err);
      throw err;
    });
  } else {
    resp = await service.post(url, data, config).catch(err => {
      handleRequestReject(err);
      throw err;
    });
  }

  return resp ? resp.data : {};
};

const reject = (data, statusCode = 200) => {
  return Promise.reject({
    data,
    statusCode
  });
};

const refreshToken = service => {
  if (!stores.authorize.refreshToken) {
    return reject(void 0, -22);
  }

  const config = {
    method: 'post',
    url: '/lycoris/authentication/refresh/token',
    data: {
      refreshToken: stores.authorize.refreshToken
    }
  };

  return service(config);
};

let isRefreshing = false; // 标记是否正在刷新令牌
let refreshSubscribers = []; // 用于存储等待刷新的请求

function handleRefreshSubscribers() {
  const interval = setInterval(() => {
    if (refreshSubscribers.length == 0) {
      setTimeout(() => {
        clearInterval(interval);
      }, 5000);
    } else {
      const item = refreshSubscribers.shift();
      item(stores.authorize.token);
    }
  }, 100);
}

service.interceptors.response.use(responseSuccessInterceptor, responseErrorInterceptor);

function responseSuccessInterceptor(resp) {
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
      resMsg: resp.data.resMsg || '操作失败',
      data: resp.data.data
    });
  }

  if (resp.data.resCode == -21) {
    if (resp.config.url.includes('/authentication/sso/login')) {
      return Promise.resolve(resp);
    }

    const originalRequest = { ...resp.config };

    if (!isRefreshing) {
      isRefreshing = true;
      return refreshToken(service)
        .then(res => {
          if (res.data.resCode != 0) {
            return reject(void 0, -1);
          }

          stores.authorize.setUserLoginState(res.data.data);
          originalRequest.headers['X-Real-User'] = stores.authorize.token;

          if (refreshSubscribers && refreshSubscribers.length) {
            handleRefreshSubscribers();
          }

          return service(originalRequest);
        })
        .catch(err => {
          return reject(err, err?.statusCode);
        })
        .finally(() => {
          setTimeout(() => {
            isRefreshing = false;
          }, 3000);
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
}

function responseErrorInterceptor(err) {
  if (err && err.response) {
    if (err.config.headers['X-Response-Interceptors']) {
      return Promise.reject(err);
    }

    // 刷新令牌返回错误的直接返回登录页
    if (err.config.url.includes('/authentication/refresh/token')) {
      stores.authorize.setUserLogoutState();
      stores.user.setLogoutState();
      return reject({}, err.response.status);
    }

    if (err.response.status == 401 || err.response.status == 403) {
      stores.authorize.setUserLogoutState();
      stores.user.setLogoutState();
      return reject({}, err.response.status);
    }

    switch (err.response.status) {
      case 400:
        err.message = '系统繁忙，请稍候再试';
        break;
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
        response: err.response || {},
        messate: err.message
      },
      err.response.status
    );
  }

  toast.warn('请求超时', {
    max: 1
  });

  // router.push({ name: 'server-error' });

  return reject(
    {
      err: err
    },
    0
  );
}

export default {
  axios,
  get,
  post
};
