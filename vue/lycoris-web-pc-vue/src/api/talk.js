import request from '../utils/request';

const controller = '/talk';

export const getTalkList = ({ pageIndex, pageSize }) => {
  return request.get(`${controller}/list`, { pageIndex, pageSize });
};
