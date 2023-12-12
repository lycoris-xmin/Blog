import request from '../utils/request';
const controller = '/post';

export const getList = data => {
  if (data.title == '') {
    delete data.title;
  }
  return request.get(`${controller}/query/list`, data);
};

export const getPostInfo = id => {
  return request.get(`${controller}/info`, {
    id
  });
};

export const savePost = data => {
  if (data == void 0) {
    return;
  }
  if (!data.id || data.id == '0') {
    delete data.id;
  }

  return request.post(`${controller}/save`, data);
};

export const uploadMarkdownPicture = file => {
  return request.post(
    `${controller}/markdown/upload`,
    {
      file
    },
    true
  );
};

export const deletePost = id => {
  return request.post(`${controller}/delete`, { id });
};

export const publishPost = id => {
  return request.post(`${controller}/publish?id=${id}`);
};

export const setPostComment = data => {
  return request.post(`${controller}/comment`, data);
};

export const setPostRecommend = data => {
  return request.post(`${controller}/recommend`, data);
};

export const uploadPostIcon = file => {
  return request.post(`${controller}/icon/upload`, { file }, true);
};
