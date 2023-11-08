import request from '../utils/request';

const controller = '/friendLink';

export const getList = ({ pageIndex, pageSize, status, name }) => {
  if (status != 0 && status != 1 && status != 2) {
    status = void 0;
  }

  if (!name) {
    name = void 0;
  }
  return request.get(`${controller}/query/list`, { pageIndex, pageSize, status, name });
};

export const createFriendLink = ({ name, icon, link, description }) => {
  return request.post(`${controller}/create`, { name, icon, link, description });
};

export const auditFriendLink = ({ id, status, description, remark }) => {
  let data = {
    id,
    status
  };

  if (description) {
    data.description = description;
  }

  if (remark) {
    data.remark = remark;
  }

  return request.post(`${controller}/audit`, data);
};

export const deleteFriendLink = id => {
  return request.post(`${controller}/delete`, { id });
};
