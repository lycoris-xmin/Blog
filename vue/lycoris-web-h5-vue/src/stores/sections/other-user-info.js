import { defineStore } from 'pinia';

export default defineStore('other-user-info', {
  state: () => {
    return [];
  },
  actions: {
    getUserInfo(id) {
      const data = this.$state.filter(x => x.id == id);
      if (data && data.length) {
        return data[0];
      } else {
        return void 0;
      }
    },
    addUserInfo({ id, nickName, avatar, email, qq, wechat, github, bilibili, music }) {
      this.$state.push({
        id,
        nickName,
        avatar,
        email,
        qq,
        wechat,
        github,
        bilibili,
        music
      });
    }
  }
});
