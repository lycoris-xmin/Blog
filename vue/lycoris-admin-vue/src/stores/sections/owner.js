import { defineStore } from 'pinia';

export default defineStore('web-owner', {
  state: () => {
    return {
      nickName: '',
      avatar: '',
      email: '',
      qq: '',
      wechat: '',
      gitHub: '',
      bilibili: '',
      cloudMusic: ''
    };
  },
  getters: {
    isValid: state => {
      return state.name != '' && state.avatar != '';
    }
  },
  actions: {
    setData({ nickName, avatar, email, qq, wechat, gitHub, bilibili, cloudMusic }) {
      if (nickName != undefined) {
        this.nickName = nickName || 'Lycoris';
      }

      if (avatar != undefined) {
        this.avatar = avatar || '/avatar/default_admin.jpeg';
      }

      if (email != undefined) {
        this.email = email || '';
      }

      if (qq != undefined) {
        this.qq = qq || '';
      }

      if (wechat != undefined) {
        this.wechat = wechat || '';
      }

      if (gitHub != undefined) {
        this.gitHub = gitHub || '';
      }

      if (bilibili != undefined) {
        this.bilibili = bilibili || '';
      }

      if (cloudMusic != undefined) {
        this.cloudMusic = cloudMusic || '';
      }
    }
  }
});
