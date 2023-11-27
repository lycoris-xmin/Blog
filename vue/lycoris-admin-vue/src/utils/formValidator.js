import { passwordRegex, urlRegex } from './regex';

export const passwordValidator = function (rule, value, callback) {
  if (value == '') {
    callback(new Error('密码不能为空'));
    return;
  }

  if (!passwordRegex(value)) {
    callback(new Error('密码必须包含大写字母，小写字母，数字，特殊符号 `@#$%^&*`~()-+=` 中任意3项密码'));
    return;
  }

  callback();
};

export const passwordConfirmValidator = function (password, rule, value, callback) {
  if (value == '') {
    callback(new Error('密码不能为空'));
    return;
  }

  if (value != password) {
    callback(new Error('两次输入的密码不一致'));
    return;
  }

  callback();
};

export const urlValidator = function (rule, value, callback) {
  if (!urlRegex(value)) {
    callback(new Error('请输入正确的链接地址'));
  } else {
    callback();
  }
};
