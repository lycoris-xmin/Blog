import request from '../utils/request';
import { stores } from '../stores';

const controller = '/user';

export const getUserBrief = (userId = undefined) => {
  if (userId == undefined || userId == '') {
    if (stores.authorize.token) {
      return request.get(`${controller}/brief`);
    } else {
      return new Promise.reject('');
    }
  } else if (userId != '0') {
    return request.get(`${controller}/brief/${userId}`);
  }
};

export const updateUserBrief = ({ nickName, avatar, blog, qq, wechat, github, gitee, email, bilibili, cloudMusic, file }) => {
  let data = {};
  if (nickName) {
    data.nickName = nickName;
  }

  if (avatar) {
    data.avatar = avatar;
  }

  if (blog) {
    data.blog = blog;
  }

  if (qq) {
    data.qq = qq;
  }

  if (wechat) {
    data.wechat = wechat;
  }

  if (github) {
    data.github = github;
  }

  if (gitee) {
    data.gitee = gitee;
  }

  if (email) {
    data.email = email;
  }

  if (bilibili) {
    data.bilibili = bilibili;
  }

  if (cloudMusic) {
    data.cloudMusic = cloudMusic;
  }

  if (file) {
    data.file = file;
  }

  if (Object.keys(data).length == 0) {
    return Promise.resolve({ resCode: 0 });
  }

  return request.post(`${controller}/brief/update`, data, true);
};
