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
    return {
      data: {
        token: '',
        refreshToken: ''
      }
    };
  },
  getters: {
    token: state => {
      if (state.data.token) {
        return state.data.token;
      }

      let token = decryptString(localStorage.getItem(tokenKey));
      if (!token) {
        return '';
      }

      state.data.token = token;
      return state.data.token;
    },
    refreshToken: state => {
      if (state.data.refreshToken) {
        return state.data.refreshToken;
      }

      let refreshToken = decryptString(localStorage.getItem(refreshTokenKey));
      state.data.refreshToken = refreshToken;
      return state.data.refreshToken;
    }
  },
  actions: {
    /**
     * @function 设置用户登录状态
     * @param {Object} authorize
     */
    setUserLoginState: function (authorize) {
      this.data.token = authorize.token || '';
      localStorage.setItem(tokenKey, encryptString(this.data.token));

      if (authorize.refreshToken) {
        this.data.refreshToken = authorize.refreshToken;
        localStorage.setItem(refreshTokenKey, encryptString(this.data.refreshToken));
      }
    },
    /**
     * @function 设置用户登出
     */
    setUserLogoutState: function () {
      this.data.token = '';
      this.data.refreshToken = '';
      localStorage.removeItem(tokenKey);
      localStorage.removeItem(refreshTokenKey);
    }
  }
});
