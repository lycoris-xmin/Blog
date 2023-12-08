import request from '../utils/request';
const controller = '/sitenavigation';

export const getSiteNavigationList = groupId => {
  return request.get(`${controller}/list`, {
    groupId
  });
};

export const getGroups = () => {
  return request.get(`${controller}/enum/group`);
};
