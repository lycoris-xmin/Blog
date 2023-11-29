import request from '../utils/request';

const controller = '/WebStatistics';

export const getWorldMapList = () => {
  return request.get(`${controller}/worldmap/list`);
};
