import request from '../utils/request';

const controller = '/lycoris/website';

export const getAboutWeb = () => {
  return request.get(`${controller}/about/web`);
};

export const saveAboutWeb = value => {
  return request.post(`${controller}/about/web`, {
    value
  });
};

export const getAboutMe = type => {
  return request.get(`${controller}/about/me/${type}`);
};

export const saveAboutMe = (type, value) => {
  return request.post(`${controller}/about/me`, {
    type: type,
    config: JSON.stringify(value)
  });
};

export const uploadFile = (path, file) => {
  let data = {
    path: path,
    file: file
  };

  return request.post(`${controller}/about/upload`, data, true);
};
