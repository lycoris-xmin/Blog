import { defineStore } from 'pinia';
import secret from '../../utils/secret';

const key = 'v-u-state';

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

const getStoreState = ({ id, nickName, avatar, blog, qq, wechat, github, gitee, email, bilibili, cloudMusic, isAdmin, state }) => {
  return { id, nickName, avatar, blog, qq, wechat, github, gitee, email, bilibili, cloudMusic, isAdmin, state };
};

export default defineStore('user', {
  state: () => {
    let empthValue = {
      id: '',
      nickName: '',
      avatar: '',
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

    try {
      let value = localStorage.getItem(key);
      return value ? decryptString(value) : empthValue;
    } catch (error) {
      return empthValue;
    }
  },
  actions: {
    setLoginState({ id, nickName, avatar, blog, qq, wechat, github, gitee, email, bilibili, cloudMusic, isAdmin }) {
      this.id = id || '';
      this.nickName = nickName || '';
      this.avatar = avatar || '';
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

      localStorage.setItem(key, encryptString(getStoreState(this)));
    },
    updateUser({ nickName, avatar, blog, qq, wechat, github, gitee, bilibili, cloudMusic }) {
      if (nickName) {
        this.nickName = nickName;
      }

      if (avatar) {
        this.avatar = avatar;
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

      localStorage.setItem(key, encryptString(getStoreState(this)));
    },
    setLogoutState() {
      try {
        this.id = '';
        this.nickName = '';
        this.avatar = '';
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

        localStorage.setItem(key, '');
      } catch (error) {
        debugger;
      }
    }
  }
});
