import request from '../utils/request';
const controller = '/requestlog';

export const getList = ({ pageIndex, pageSize, beginTime, endTime, route, ip, status, elapsed }) => {
  let data = {
    pageIndex,
    pageSize,
    beginTime,
    endTime,
    route,
    ip,
    elapsed,
    status
  };

  if (!data.beginTime) {
    delete data.beginTime;
  }

  if (!data.endTime) {
    delete data.endTime;
  }

  if (!data.ip) {
    delete data.ip;
  }

  if (!data.route) {
    delete data.route;
  }

  if (data.elapsed == '') {
    delete data.elapsed;
  }

  if (data.status == '') {
    delete data.status;
  }

  return request.get(`${controller}/list`, data);
};

export const getInfo = id => {
  return request.get(`${controller}/info`, {
    id
  });
};

export const deleteLog = ids => {
  return request.post(`${controller}/delete`, {
    ids
  });
};

export const setAccessControl = ip => {
  return request.post(`${controller}/accesscontrol`, {
    ip
  });
};
