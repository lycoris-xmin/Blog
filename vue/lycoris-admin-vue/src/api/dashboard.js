import request from '../utils/request';

const controller = '/lycoris/dashboard';

export const getWebStatistics = () => {
  return request.get(`${controller}/web/statistics`);
};

export const getNearlyDaysWebStatistics = () => {
  return request.get(`${controller}/nearlydays/web/statistics`);
};
