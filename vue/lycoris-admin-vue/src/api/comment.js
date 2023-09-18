import request from '../utils/request';
const controller = '/lycoris/comment';

export const getCommentList = ({ pageIndex, pageSize, title, content, userId }) => {
  let data = {
    pageIndex,
    pageSize,
    title,
    content,
    userId
  };

  if (!data.title) {
    delete data.title;
  }

  if (!data.content) {
    delete data.content;
  }

  if (!data.userId) {
    delete data.userId;
  }

  return request.get(`${controller}/query/list`, data);
};

export const deleteComment = ids => {
  let data = {
    ids: []
  };

  if (ids.constructor === Array) {
    data.ids = ids;
  } else if (typeof ids == 'string') {
    data.ids.push(ids);
  } else {
    return;
  }

  return request.post(`${controller}/delete`, data);
};
