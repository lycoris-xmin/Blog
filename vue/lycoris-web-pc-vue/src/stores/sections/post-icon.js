import { defineStore } from 'pinia';

const key = 'post-icon';

export default defineStore('post-icon', {
  state: () => {
    let icon = [];
    let str = localStorage.getItem(key);
    if (str) {
      icon = JSON.parse(str);
    }

    return {
      icon
    };
  },
  actions: {
    setPostIcon(data) {
      if (data.length > 0) {
        this.icon = data;
        localStorage.setItem(key, JSON.stringify(this.icon));
      } else {
        this._initIcon();
      }
    },
    getRandomPostIcon() {
      if (this.icon.length > 1) {
        let index = getRndInteger(0, this.icon.length);
        return this.icon[index];
      } else if (this.icon.length == 0) {
        this._initIcon();
      }

      return this.icon[0];
    }
  },
  _initIcon() {
    this.icon = [''];
  }
});

function getRndInteger(min, max) {
  return Math.floor(Math.random() * (max - min)) + min;
}
