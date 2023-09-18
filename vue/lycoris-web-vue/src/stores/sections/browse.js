import { defineStore } from 'pinia';

const key = 'l-z-history';

export default defineStore('browse', {
  state: () => {
    let pageState = {};
    let str = localStorage.getItem(key);
    if (str) {
      pageState = JSON.parse(str);
    }

    return {
      pageState
    };
  },
  actions: {
    setSync(path) {
      this.pageState[path] = new Date().getTime();
      localStorage.setItem(key, JSON.stringify(this.pageState));
    },
    checkCanSync(path) {
      let time = this.pageState[path] || 0;
      return time + 30 * 60 * 60 < new Date().getTime();
    }
  }
});
