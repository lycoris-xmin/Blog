import request from '../utils/request';
import { stores } from '../stores';
import { throttle } from '../utils/tool';

const controller = '/authentication';

export const ssoLogin = token => {
  return request.post(`${controller}/sso/login`, {}, false, {
    headers: {
      'X-Real-User': token
    }
  });
};

export const loginValidate = ({ email, password }) => {
  return request.post(`${controller}/login/validate`, {
    email,
    password
  });
};

export const login = ({ email, oathCode, remember }) => {
  return request.post(`${controller}/dashboard/login`, {
    email,
    oathCode,
    remember,
    adminLogin: true
  });
};

export const logout = () => {
  return request.post(`${controller}/dashboard/logout`);
};

export const refreshToken = throttle(async () => {
  try {
    const res = await request.post(`${controller}/dashboard/refresh/token`, {
      refreshToken: stores.authorize.refreshToken
    });

    if (res && res.resCode == 0) {
      stores.authorize.setUserLoginState(res.data);
    } else {
      stores.authorize.setUserLogoutState();
    }
  } catch (error) {
    stores.authorize.setUserLogoutState();
  }
}, 1000);

export const changePassword = ({ oldPassword, password }) => {
  return request.post(`${controller}/dashboard/changepassword`, { oldPassword, password });
};

export const screenUnLock = password => {
  return request.post(`${controller}/dashboard/screen/unLock`, {
    password
  });
};
