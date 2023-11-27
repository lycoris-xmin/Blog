import request from '../utils/request';
const controller = '/lycoris/sitenavigation';

export const getSiteNavigationList = () => {
  return request.get(`${controller}/list`);
};
