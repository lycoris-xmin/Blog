import request from '../utils/request';

const controller = '/lycoris/configuration';

export const getWebSettings = () => {
  return request.get(`${controller}/web`);
};

export const saveWebSettings = ({ webName, webPath, adminPath, buildTime }) => {
  return request.post(`${controller}/web`, { webName, webPath, adminPath, buildTime });
};

export const getPostSettings = () => {
  return request.get(`${controller}/post`);
};

export const savePostSettings = ({ autoSave, second, images }) => {
  return request.post(`${controller}/post`, {
    autoSave,
    second,
    images
  });
};

export const getEmailSettings = () => {
  return request.get(`${controller}/email`);
};

export const saveEmailSettings = ({ emailAddress, emailUser, stmpServer, stmpPort, emailPassword, emailSignature, useSSL }) => {
  return request.post(`${controller}/email`, { emailAddress, emailUser, stmpServer, stmpPort, emailPassword, emailSignature, useSSL });
};

export const getfileUploadSettings = () => {
  return request.get(`${controller}/fileUpload`);
};

export const saveFileUploadSettings = ({ saveChannel, minio, oss, cos, obs, kodo }) => {
  let data = {
    saveChannel: saveChannel
  };

  if (data.saveChannel == 10) {
    data.minio = minio;
  }

  // 阿里云存储
  if (data.saveChannel == 20) {
    data.oss = oss;
  }

  // 腾讯云存储
  if (data.saveChannel == 30) {
    data.cos = cos;
  }

  // 华为云存储
  if (data.saveChannel == 40) {
    data.obs = obs;
  }

  // 七牛云存储
  if (data.saveChannel == 50) {
    data.kodo = kodo;
  }

  return request.post(`${controller}/fileUpload`, data);
};

export const getFileSaveChannelEnum = () => {
  return request.get(`${controller}/fileUpload/channel`);
};

export const getSeoSettings = () => {
  return request.get(`${controller}/seo`);
};

export const saveSeoSettings = data => {
  return request.post(`${controller}/seo`, data);
};

export const uploadFile = (configName, file) => {
  let data = {
    configName: configName,
    file: file
  };

  return request.post(`${controller}/upload`, data, true);
};
