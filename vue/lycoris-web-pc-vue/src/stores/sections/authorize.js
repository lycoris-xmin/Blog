import { defineStore } from 'pinia';
import secret from '../../utils/secret';

const tokenKey = 'l-z-uid',
  refreshTokenKey = 'l-z-rk';

const encryptString = data => {
  return secret.encrypt(JSON.stringify(data));
};

const decryptString = data => {
  if (!data) {
    return '';
  }
  let val = secret.decrypt(data);
  return val ? JSON.parse(val) : '';
};

export default defineStore('authorize', {
  state: () => {
    let data = {
      token: '',
      refreshToken: ''
    };

    try {
      let value = localStorage.getItem(tokenKey);
      data.token = value ? decryptString(value) : '';
    } catch {
      data.token = '';
    }

    try {
      let value = localStorage.getItem(refreshTokenKey);
      data.refreshToken = value ? decryptString(value) : '';
    } catch {
      data.refreshToken = '';
    }

    return data;
  },
  actions: {
    /**
     * @function 设置用户登录状态
     * @param {Object} authorize
     */
    setUserLoginState: function (authorize) {
      this.token = authorize.token || '';
      localStorage.setItem(tokenKey, encryptString(this.token));

      if (authorize.refreshToken) {
        this.refreshToken = authorize.refreshToken;
        localStorage.setItem(refreshTokenKey, encryptString(this.refreshToken));
      }
    },
    /**
     * @function 设置用户登出
     */
    setUserLogoutState: function () {
      this.token = '';
      this.refreshToken = '';
      localStorage.removeItem(tokenKey);
      localStorage.removeItem(refreshTokenKey);
    }
  }
});
