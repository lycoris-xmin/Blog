import { defineStore } from 'pinia';
import secret from '../../utils/secret';

const tokenKey = 'v-t-ek',
  refreshTokenKey = 'v-k-rt';

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
      data.token = decryptString(localStorage.getItem(tokenKey));
    } catch {
      data.token = '';
    }

    try {
      data.refreshToken = decryptString(localStorage.getItem(refreshTokenKey));
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
