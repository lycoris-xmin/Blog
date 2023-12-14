import { defineStore } from 'pinia';
import { api } from '../../config.json';

export default defineStore('user', {
  state: () => {
    return {
      id: '',
      nickName: '',
      avatar: '',
      cancellationTime: 0,
      blog: '',
      qq: '',
      wechat: '',
      github: '',
      gitee: '',
      email: '',
      bilibili: '',
      cloudMusic: '',
      isAdmin: false,
      state: false
    };
  },
  actions: {
    setLoginState({ id, nickName, avatar, cancellationTime, blog, qq, wechat, github, gitee, email, bilibili, cloudMusic, isAdmin }) {
      this.id = id || '';
      this.nickName = nickName || '';

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

      if (cancellationTime) {
        this.cancellationTime = new Date(cancellationTime).getTime();
      }

      this.blog = blog || '';
      this.qq = qq || '';
      this.wechat = wechat || '';
      this.github = github || '';
      this.gitee = gitee || '';
      this.email = email || '';
      this.bilibili = bilibili || '';
      this.cloudMusic = cloudMusic || '';
      this.isAdmin = isAdmin || false;
      this.state = true;
    },
    updateUser({ nickName, avatar, blog, qq, wechat, github, gitee, bilibili, cloudMusic }) {
      if (nickName) {
        this.nickName = nickName;
      }

      if (avatar) {
        if (avatar.startsWith('/avatar')) {
          this.avatar = `${api.server}${avatar}`;
        } else {
          this.avatar = avatar;
        }
      } else {
        this.avatar = '/avatar/default_admin.jpeg';
      }

      if (blog) {
        this.blog = blog;
      }

      if (qq) {
        this.qq = qq;
      }

      if (wechat) {
        this.wechat = wechat;
      }

      if (github) {
        this.github = github;
      }

      if (gitee) {
        this.gitee = gitee;
      }

      if (bilibili) {
        this.bilibili = bilibili;
      }

      if (cloudMusic) {
        this.cloudMusic = cloudMusic;
      }
    },
    setCancell(cancellationTime) {
      if (cancellationTime) {
        this.cancellationTime = new Date(cancellationTime).getTime();
      }
    },
    clearCancell() {
      this.cancellationTime = 0;
    },
    setLogoutState() {
      this.id = '';
      this.nickName = '';
      this.avatar = '';
      this.cancellationTime = 0;
      this.blog = '';
      this.qq = '';
      this.wechat = '';
      this.github = '';
      this.gitee = '';
      this.email = '';
      this.bilibili = '';
      this.cloudMusic = '';
      this.isAdmin = false;
      this.state = false;
    }
  }
});
