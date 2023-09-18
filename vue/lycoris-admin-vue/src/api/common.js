import request from '../utils/request';

export const getWebOwner = () => {
  return request.get('/lycoris/common/webowner');
};
