import request from '../utils/request';
const controller = '/lycoris/post';

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
  if (!data.id || data.id == '0') {
    delete data.id;
  }
  return request.post(`${controller}/save`, data, true);
};

export const uploadMarkdownPicture = file => {
  let data = {
    file: file
  };

  return request.post(`${controller}/markdown/upload`, data, true);
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
