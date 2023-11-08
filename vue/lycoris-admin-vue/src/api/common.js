import request from '../utils/request';

export const getWebOwner = () => {
  return request.get('/common/webowner');
};
