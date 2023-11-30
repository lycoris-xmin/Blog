import request from '../utils/request';

const controller = '/WebStatistics';

export const getBrowseStatisticsList = ({ pageIndex, pageSize, sum }) => {
  let data = {
    pageIndex,
    pageSize,
    sum
  };

  if (sum == undefined || (typeof data.sum == 'boolean' && !data.sum)) {
    delete data.sum;
  }

  return request.get(`${controller}/browse/list`, data);
};

export const getRefererStatisticsList = ({ pageIndex, pageSize, sum }) => {
  let data = {
    pageIndex,
    pageSize,
    sum
  };

  if (sum == undefined || (typeof data.sum == 'boolean' && !data.sum)) {
    delete data.sum;
  }

  return request.get(`${controller}/referer/list`, data);
};

export const getWorldMapList = () => {
  return request.get(`${controller}/worldmap/list`);
};
