import request from '../utils/request';

const controller = '/lycoris/user';

export const getUserBrief = (userId = undefined) => {
  if (userId == undefined || userId == '') {
    return request.get(`${controller}/brief`);
  } else {
    return request.get(`${controller}/brief/${userId}`);
  }
};
