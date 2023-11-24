import request from '../utils/request';

const controller = '/configuration';

export const getWebSettings = () => {
  return request.get(`${controller}/web`);
};

export const saveWebSettings = ({ webName, webPath, adminPath, logo, logoDisplay, favicon, icp, buildTime, avatar, description }) => {
  return request.post(`${controller}/web`, { webName, webPath, logo, logoDisplay, adminPath, favicon, icp, description, buildTime, avatar }, true);
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

export const sendTestEmail = ({ emailAddress, emailUser, stmpServer, stmpPort, emailPassword, emailSignature, useSSL, testEmail }) => {
  return request.post(`${controller}/email/test`, { emailAddress, emailUser, stmpServer, stmpPort, emailPassword, emailSignature, useSSL, testEmail });
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

export const getSystemSettings = () => {
  return request.get(`${controller}/systemsettings`);
};

export const saveShowdocSettings = host => {
  return request.post(`${controller}/systemsettings/showdoc`, {
    host
  });
};

export const saveSystemFileClear = ({ staticFile, tempFile, logFile }) => {
  return request.post(`${controller}/systemsettings/fileclear`, { staticFile, tempFile, logFile });
};

export const saveSystemDbClear = ({ requestLog, browseLog, postComment, leaveMessage }) => {
  return request.post(`${controller}/systemsettings/dbclear`, { requestLog, browseLog, postComment, leaveMessage });
};

export const uploadFile = (configName, file) => {
  let data = {
    configName: configName,
    file: file
  };

  return request.post(`${controller}/upload`, data, true);
};

export const showdocPushTest = data => {
  return request.post(`${controller}/showdoc/pushtest`, data);
};
