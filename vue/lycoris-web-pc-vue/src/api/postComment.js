import request from '../utils/request';
const controller = '/postcomment';

export const getCommentList = ({ pageIndex, pageSize, postId }) => {
  if (!postId) {
    return Promise.resolve({
      resCode: 0,
      resMsg: '',
      data: {
        count: 0,
        list: []
      }
    });
  }

  return request.get(`${controller}/list`, {
    pageIndex,
    pageSize,
    postId
  });
};

export const publishComment = ({ postId, repliedUserId, content }) => {
  let data = {
    postId,
    repliedUserId,
    content
  };

  if (!data.repliedUserId) {
    delete data.repliedUserId;
  }

  return request.post(`${controller}/publish`, data);
};
