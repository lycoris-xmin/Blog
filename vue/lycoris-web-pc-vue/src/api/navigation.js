import request from '../utils/request';
const controller = '/sitenavigation';

export const getSiteNavigationList = () => {
  return request.get(`${controller}/list`);
};
