import { defineStore } from 'pinia';
import secret from '../../utils/secret';

const key = 'l-c-val';
const timeOutValue = 30 * 60 * 1000;

const encryptString = data => {
  return secret.encrypt(JSON.stringify(data));
};

const decryptString = data => {
  if (!data) {
    return {
      lastActiveTime: 0
    };
  }
  let val = secret.decrypt(data);
  return val
    ? JSON.parse(val)
    : {
        lastActiveTime: -1
      };
};

export default defineStore('screen-lock', {
  state: () => {
    let val = localStorage.getItem(key);
    return decryptString(val);
  },
  actions: {
    setActive() {
      this.lastActiveTime = new Date().getTime();
      localStorage.setItem(key, encryptString({ lastActiveTime: this.lastActiveTime }));
    },
    setLossOfActivity() {
      this.lastActiveTime = new Date().getTime() - timeOutValue;
      localStorage.setItem(key, encryptString({ lastActiveTime: this.lastActiveTime }));
    },
    checkLossOfActivity() {
      var val = this.lastActiveTime + timeOutValue <= new Date().getTime();
      if (this.lastActiveTime == 0) {
        this.setActive();
      }
      return val;
    }
  }
});
