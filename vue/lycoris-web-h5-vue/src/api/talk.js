import request from '../utils/request';

const controller = '/lycoris/talk';

export const getTalkList = ({ pageIndex, pageSize }) => {
  return request.get(`${controller}/list`, { pageIndex, pageSize });
};
