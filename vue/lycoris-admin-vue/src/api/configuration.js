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

export const getStaticFileSettings = () => {
  return request.get(`${controller}/staticFile`);
};

export const saveStaticFileSettings = data => {
  return request.post(`${controller}/staticFile`, data);
};

export const getUploadChannelEnum = () => {
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
