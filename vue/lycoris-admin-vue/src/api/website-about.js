import request from '../utils/request';

const controller = '/WebSite/About';

export const getAboutWeb = () => {
  return request.get(`${controller}/aboutweb`);
};

export const saveAboutWeb = value => {
  return request.post(`${controller}/web`, {
    value
  });
};

export const getAboutMe = type => {
  return request.get(`${controller}/me/${type}`);
};

export const saveAboutMe = (type, value) => {
  return request.post(`${controller}/me`, {
    type: type,
    config: JSON.stringify(value)
  });
};
