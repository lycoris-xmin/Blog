import request from '../utils/request';
const controller = '/browselog';

export const getList = ({ pageIndex, pageSize, beginTime, endTime, path, ip, referer }) => {
  let data = {
    pageIndex,
    pageSize,
    beginTime,
    endTime,
    path,
    ip,
    referer
  };

  if (!data.beginTime) {
    delete data.beginTime;
  }

  if (!data.endTime) {
    delete data.endTime;
  }

  if (!data.path) {
    delete data.path;
  }

  if (!data.ip) {
    delete data.ip;
  }

  if (!data.referer) {
    delete data.referer;
  }

  return request.get(`${controller}/query/list`, data);
};

export const deleteLog = ids => {
  return request.post(`${controller}/delete`, {
    ids
  });
};

export const getRefererList = ({ pageIndex, pageSize }) => {
  return request.get(`${controller}/referer/query/list`, {
    pageIndex,
    pageSize
  });
};

export const deleteReferer = ids => {
  return request.post(`${controller}/referer/delete`, {
    ids
  });
};
