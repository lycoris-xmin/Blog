import { defineStore } from 'pinia';
import { api } from '../../config.json';

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
      music: '',
      statistics: {
        post: 0,
        talk: 0,
        category: 0
      }
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

      if (avatar != undefined) {
        if (avatar) {
          if (avatar.startsWith('/avatar')) {
            this.avatar = `${api.server}${avatar}`;
          } else {
            this.avatar = avatar;
          }
        } else {
          this.avatar = '/avatar/default_admin.jpeg';
        }
      }

      this.email = email || '';
      this.qq = qq || '';
      this.wechat = wechat || '';
      this.github = github || '';
      this.bilibili = bilibili;
      this.music = music;
    },
    setStatistics(statistics) {
      this.statistics = statistics || {
        post: 0,
        talk: 0,
        category: 0
      };
    }
  }
});
