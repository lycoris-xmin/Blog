import request from '../utils/request';

const controller = '/lycoris/category';

export const getCategoryHeaders = () => {
  return request.get(`${controller}/header`);
};

export const getCategoryEnums = () => {
  return request.get(`${controller}/enum`);
};
