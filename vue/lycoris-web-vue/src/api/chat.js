import request from '../utils/request';

const controller = '/lycoris/chat';

export const getChatRoomList = ({ pageIndex, pageSize }) => {
  return request.get(`${controller}/room/list`, {
    pageIndex,
    pageSize
  });
};

export const getChatMessageList = ({ roomId, pageIndex, pageSize }) => {
  return request.get(`${controller}/message/list`, {
    roomId,
    pageIndex,
    pageSize
  });
};
