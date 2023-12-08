import request from '../utils/request';
const controller = '/message';

export const getConfiguration = () => {
  return request.get(`${controller}/configuration`);
};

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

export const publishReplyMessage = (messageId, content, repliedUserId) => {
  let data = {
    messageId,
    content
  };
  if (repliedUserId) {
    data.repliedUserId = repliedUserId;
  }
  return request.post(`${controller}/reply/publish`, data);
};

export const deleteSlefMessage = id => {
  return request.post(`${controller}/self/delete`, {
    id
  });
};
