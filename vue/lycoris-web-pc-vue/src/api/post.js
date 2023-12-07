import request from '../utils/request';
const controller = '/post';

export const getRecommendPostList = () => {
  return request.get(`${controller}/recommend/list`);
};

export const getPostList = data => {
  if (!data.title) {
    delete data.title;
  }

  if (!data.category) {
    delete data.category;
  }

  return request.get(`${controller}/list`, data);
};

export const getPostDetail = id => {
  return request.get(`${controller}/detail`, {
    id
  });
};

export const getPostPreviousAndNext = id => {
  return request.get(`${controller}/previousandnext`, {
    id
  });
};

export const searchPost = keyword => {
  return request.get(`${controller}/search`, {
    keyword
  });
};
