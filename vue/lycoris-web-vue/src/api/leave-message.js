import request from '../utils/request';
const controller = '/lycoris/leavemessage';

export const getMessageList = ({ pageIndex, pageSize }) => {
  return request.get(`${controller}/list`, {
    pageIndex,
    pageSize
  });
};

export const getReplyMessageLsit = ({ messageId, pageIndex, pageSize }) => {
  return request.get(`${controller}/reply/list`, {
    messageId,
    pageIndex,
    pageSize
  });
};

export const publishMessage = content => {
  return request.post(`${controller}/publish`, {
    content
  });
};

export const publishReplyMessage = (messageId, content) => {
  return request.post(`${controller}/reply/publish`, {
    messageId,
    content
  });
};

export const deleteSlefMessage = id => {
  return request.post(`${controller}/self/delete`, {
    id
  });
};
