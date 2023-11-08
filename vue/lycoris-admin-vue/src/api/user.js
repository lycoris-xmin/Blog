import request from '../utils/request';

const controller = '/user';

export const getUserBrief = () => {
  return request.get(`${controller}/dashboard/brief`);
};

export const updateUserBrief = data => {
  return request.post(`${controller}/dashboard/brief/update`, data, true);
};
