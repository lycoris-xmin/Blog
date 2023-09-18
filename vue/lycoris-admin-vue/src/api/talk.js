import request from '../utils/request';

const controller = '/lycoris/talk';

export const getList = ({ pageIndex, pageSize }) => {
  return request.get(`${controller}/query/list`, {
    pageIndex,
    pageSize
  });
};

export const createorupdate = ({ id, content }) => {
  let data = {
    id,
    content
  };

  if (!data.id) {
    delete data.id;
  }

  return request.post(`${controller}/createorupdate`, data);
};

export const deleteTalk = id => {
  return request.post(`${controller}/delete?id=${id}`);
};
