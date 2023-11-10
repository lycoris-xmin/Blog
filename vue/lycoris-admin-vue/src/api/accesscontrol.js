import request from '../utils/request';

const controller = '/accessControl';

export const getList = ({ pageIndex, pageSize, ip }) => {
  return request.get(`${controller}/list`, {
    pageIndex,
    pageSize,
    ip
  });
};
