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
    let token = decryptString(localStorage.getItem(tokenKey));
    let refreshToken = decryptString(localStorage.getItem(refreshTokenKey));

    return {
      token: token || '',
      refreshToken: refreshToken || ''
    };
  },
  actions: {
    /**
     * @function 设置用户登录状态
     * @param {Object} authorize
     */
    setUserLoginState: function (authorize) {
      this.token = authorize.token || '';
      this.refreshToken = authorize.refreshToken || '';

      if (this.token) {
        localStorage.setItem(tokenKey, encryptString(this.token));
      }

      if (this.refreshToken) {
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
