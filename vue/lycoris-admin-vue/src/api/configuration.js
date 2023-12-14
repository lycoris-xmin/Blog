import request from '../utils/request';

const controller = '/configuration';

export const getWebSetting = () => {
  return request.get(`${controller}/web`);
};

export const saveWebSetting = ({ webName, webPath, adminPath, logo, logoDisplay, favicon, icp, buildTime, avatar, description }) => {
  return request.post(`${controller}/web`, { webName, webPath, logo, logoDisplay, adminPath, favicon, icp, description, buildTime, avatar }, true);
};

export const getPostSetting = () => {
  return request.get(`${controller}/post`);
};

export const savePostSetting = ({ autoSave, second, images }) => {
  return request.post(`${controller}/post`, {
    autoSave,
    second,
    images
  });
};

export const getMessageSetting = () => {
  return request.get(`${controller}/message`);
};

export const saveMessageSetting = ({ messageRemind, frequencySecond }) => {
  return request.post(`${controller}/message`, {
    messageRemind,
    frequencySecond
  });
};

export const getEmailSetting = () => {
  return request.get(`${controller}/email`);
};

export const saveEmailSetting = ({ emailAddress, emailUser, stmpServer, stmpPort, emailPassword, emailSignature, useSSL }) => {
  return request.post(`${controller}/email`, { emailAddress, emailUser, stmpServer, stmpPort, emailPassword, emailSignature, useSSL });
};

export const sendTestEmail = ({ emailAddress, emailUser, stmpServer, stmpPort, emailPassword, emailSignature, useSSL, testEmail }) => {
  return request.post(`${controller}/email/test`, { emailAddress, emailUser, stmpServer, stmpPort, emailPassword, emailSignature, useSSL, testEmail });
};

export const getUploadSetting = () => {
  return request.get(`${controller}/upload`);
};

export const saveUploadSetting = data => {
  return request.post(`${controller}/upload`, data);
};

export const getUploadChannelEnum = () => {
  return request.get(`${controller}/fileUpload/channel`);
};

export const getSeoSetting = () => {
  return request.get(`${controller}/seo`);
};

export const saveSeoSetting = data => {
  return request.post(`${controller}/seo`, data);
};

export const getOthersetting = () => {
  return request.get(`${controller}/othersetting`);
};

export const saveShowdocSetting = ({ host, monitoringPush, cpuRate, ramRate, messagePush, commentPush }) => {
  return request.post(`${controller}/showdoc`, {
    host,
    monitoringPush,
    cpuRate,
    ramRate,
    messagePush,
    commentPush
  });
};

export const saveDataClear = ({ staticFile, tempFile, logFile, requestLog, browseLog, postComment, leaveMessage }) => {
  return request.post(`${controller}/dataClear`, { staticFile, tempFile, logFile, requestLog, browseLog, postComment, leaveMessage });
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
