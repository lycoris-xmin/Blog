import request from '../utils/request';

const controller = '/category';

export const getList = data => {
  return request.get(`${controller}/list`, data);
};

export const createCategory = data => {
  return request.post(`${controller}/create`, data, true);
};

export const updateCategory = data => {
  return request.post(`${controller}/update`, data, true);
};

export const deleteCategory = id => {
  return request.post(`${controller}/delete?id=${id}`);
};

export const getCategoryEnums = () => {
  return request.get(`${controller}/enum`);
};
