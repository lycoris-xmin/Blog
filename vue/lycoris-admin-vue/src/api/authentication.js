import request from '../utils/request';

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

export const refreshToken = async refreshToken => {
  return request.post(`${controller}/dashboard/refresh/token`, {
    refreshToken: refreshToken
  });
};

export const changePassword = ({ oldPassword, password }) => {
  return request.post(`${controller}/dashboard/changepassword`, { oldPassword, password });
};

export const screenUnLock = password => {
  return request.post(`${controller}/dashboard/screen/unLock`, {
    password
  });
};
