import { defineStore } from 'pinia';
import secret from '../../utils/secret';

const dataKey = 'v-user';

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
      tokenExpireTime: 0,
      refreshToken: '',
      refreshTokenExpireTime: 0
    };

    try {
      const value = decryptString(localStorage.getItem(dataKey));
      if (value && Object.keys(value).length == 4) {
        const now = new Date().getTime();
        if (now < value.refreshTokenExpireTime) {
          data.refreshToken = value.refreshToken;
          data.refreshTokenExpireTime = value.refreshTokenExpireTime;

          data.token = value.token;
          data.tokenExpireTime = value.tokenExpireTime;
        }
      }
    } catch {}

    return data;
  },
  actions: {
    /**
     * @function 设置用户登录状态
     * @param {Object} authorize
     */
    setUserLoginState: function (authorize) {
      if (authorize.token) {
        this.token = authorize.token;
      }

      if (authorize.tokenExpireTime) {
        this.tokenExpireTime = new Date(authorize.tokenExpireTime).getTime();
      }

      if (authorize.refreshToken) {
        this.refreshToken = authorize.refreshToken;
      }

      if (authorize.refreshTokenExpireTime) {
        this.refreshTokenExpireTime = new Date(authorize.refreshTokenExpireTime).getTime();
      }

      localStorage.setItem(
        dataKey,
        encryptString({
          token: this.token,
          tokenExpireTime: this.tokenExpireTime,
          refreshToken: this.refreshToken,
          refreshTokenExpireTime: this.refreshTokenExpireTime
        })
      );
    },
    /**
     * @function 设置用户登出
     */
    setUserLogoutState: function () {
      this.token = '';
      this.tokenExpireTime = 0;
      this.refreshToken = '';
      this.refreshTokenExpireTime = 0;

      //localStorage.removeItem(dataKey);
    }
  }
});
