import request from '../utils/request';

const controller = '/accessControl';

export const getList = ({ pageIndex, pageSize, ip }) => {
  return request.get(`${controller}/list`, {
    pageIndex,
    pageSize,
    ip
  });
};

export const createAccessControl = ip => {
  return request.post(`${controller}/create`, { ip });
};

export const deleteAccessControl = id => {
  return request.post(`${controller}/delete`, { id });
};

export const getAccessControlLogList = ({ pageIndex, pageSize, id }) => {
  return request.get(`${controller}/log/list`, { pageIndex, pageSize, id });
};
