import request from '../utils/request';

const controller = '/home';

export const getWebSetting = () => {
  return request.get(`${controller}/web/setting`);
};
