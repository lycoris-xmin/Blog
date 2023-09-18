import { defineStore } from 'pinia';

export default defineStore('user', {
  state: () => {
    return {
      id: '',
      nickName: '',
      avatar: '',
      qq: '',
      wechat: '',
      github: '',
      email: '',
      bilibili: '',
      music: '',
      state: false
    };
  },
  actions: {
    setLoginState({ id, nickName, avatar, qq, wechat, github, email, bilibili, music }) {
      this.id = id || '';
      this.nickName = nickName || '';
      this.avatar = avatar || '';
      this.qq = qq || '';
      this.wechat = wechat || '';
      this.github = github || '';
      this.email = email || '';
      this.bilibili = bilibili || '';
      this.music = music || '';
      this.state = true;
    },
    setLogoutState() {
      this.id = '';
      this.nickName = '';
      this.avatar = '';
      this.qq = '';
      this.wechat = '';
      this.github = '';
      this.email = '';
      this.bilibili = '';
      this.music = '';
      this.state = false;
    }
  }
});
