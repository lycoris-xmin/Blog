import request from '../utils/request';

const controller = '/lycoris/user';

export const getUserBrief = () => {
  return request.get(`${controller}/brief`);
};

export const updateUserBrief = data => {
  return request.post(`${controller}/brief/update`, data, true);
};
