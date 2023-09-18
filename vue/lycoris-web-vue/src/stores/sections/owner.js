import { defineStore } from 'pinia';

export default defineStore('web-owner', {
  state: () => {
    return {
      nickName: '',
      avatar: '',
      email: '',
      qq: '',
      wechat: '',
      github: '',
      bilibili: '',
      music: ''
    };
  },
  getters: {
    isValid: state => {
      return state.name != '' && state.avatar != '';
    }
  },
  actions: {
    setData({ nickName, avatar, email, qq, wechat, github, bilibili, music }) {
      this.nickName = nickName || 'Lycoris';
      this.avatar = avatar || '/avatar/default_admin.jpeg';
      this.email = email || '';
      this.qq = qq || '';
      this.wechat = wechat || '';
      this.github = github || '';
      this.bilibili = bilibili;
      this.music = music;
    }
  }
});
