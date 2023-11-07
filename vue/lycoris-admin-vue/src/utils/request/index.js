import axios from 'axios';
import { api } from '../../config.json';
import { stores } from '../../stores';
import response_interceptors from './response_interceptors';
import toast from '../toast';

const service = axios.create({
  baseURL: `${api.server}${api.routePrefix}`,
  timeout: api.timeout,
  withCredentials: true
});

service.interceptors.response.use(
  resp => response_interceptors.success(resp, service),
  err => response_interceptors.error(err, service)
);

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

export default {
  get,
  post
};
