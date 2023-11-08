import request from '../utils/request';
const controller = '/leavemessage';

export const getList = ({ pageIndex, pageSize, beginTime, endTime, content, ip, status }) => {
  let data = {
    pageIndex,
    pageSize,
    beginTime,
    endTime,
    content,
    ip,
    status
  };
  if (!data.content) {
    delete data.content;
  }

  if (!data.ip) {
    data.ip;
  }

  if (!data.status) {
    data.status;
  }

  return request.get(`${controller}/query/list`, data);
};

export const deleteMessage = ids => {
  ids = Array.isArray(ids) ? ids : [ids];

  return request.post(`${controller}/delete`, {
    ids
  });
};

export const setViolation = id => {
  return request.post(`${controller}/violation`, {
    id
  });
};
