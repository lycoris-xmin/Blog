import request from '../utils/request';

const controller = '/lycoris/authentication';

export const loginValidate = ({ email, password }) => {
  return request.post(`${controller}/login/validate`, {
    email,
    password
  });
};

export const login = ({ email, oathCode, remember }) => {
  return request.post(`${controller}/login`, {
    email,
    oathCode,
    remember
  });
};

export const logout = () => {
  return request.post(`${controller}/logout`);
};

export const refreshToken = async refreshToken => {
  let res = await request.post('/lycoris/authentication/refresh/token', {
    refreshToken: refreshToken
  });

  if (res && res.resCode == 0) {
    return res.data;
  } else {
    return null;
  }
};

export const getAdmin = () => {
  return request.post(`${controller}/admin`);
};

export const registerCaptcha = emial => {
  return request.post(`${controller}/captcha/register`, emial);
};

export const register = ({ email, captcha, password }) => {
  return request.post(`${controller}/register`, {
    email,
    captcha,
    password
  });
};
