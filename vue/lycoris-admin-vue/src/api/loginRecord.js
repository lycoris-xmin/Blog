import request from '../utils/request';
const controller = '/loginRecord';

export const getList = ({ pageIndex, pageSize }) => {
  return request.get(`${controller}/list`, {
    pageIndex,
    pageSize
  });
};
