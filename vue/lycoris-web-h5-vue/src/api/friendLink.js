import request from '../utils/request';

const controller = '/lycoris/friendLink';

export const getFriendLinkList = ({ pageIndex, pageSize }) => {
  return request.get(`${controller}/list`, { pageIndex, pageSize });
};

export const friendLinkApply = ({ name, icon, link, description }) => {
  return request.post(`${controller}/apply`, { name, icon, link, description });
};
