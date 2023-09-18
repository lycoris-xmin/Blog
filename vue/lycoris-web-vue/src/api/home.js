import request from '../utils/request';
import { uuid } from '../utils/tool';

const controller = '/lycoris/home';
const orignFlagKey = 'l-z-orign';

export const getWebOwner = () => {
  return request.get(`${controller}/web/owner`);
};

export const getAboutWeb = () => {
  return request.get(`${controller}/about/web`);
};

export const getAboutMe = () => {
  return request.get(`${controller}/about/me`);
};

export const pageBrowse = (path, data) => {
  if (!path || !data) {
    return;
  }

  let orignFlag = localStorage.getItem(orignFlagKey);
  if (!orignFlag) {
    orignFlag = uuid();
    localStorage.setItem(orignFlagKey, orignFlag);
  }

  return request.post(
    `${controller}/web/browse/record`,
    {
      path: path,
      ...data,
      clientOrign: orignFlag
    },
    false,
    {
      headers: {
        'X-Response-Interceptors': new Date().getTime()
      }
    }
  );
};

export const getWebStatistics = () => {
  return request.get(`${controller}/web/statistics`);
};

export const getPostStatistics = () => {
  return request.get(`${controller}/post/statistics`);
};
