import request from '../utils/request';

const controller = '/staticfile';

export const getList = ({ beginTime, endTime, uploadChannel, localBack, use, pageIndex, pageSize }) => {
  let data = {
    pageIndex,
    pageSize
  };

  if (beginTime) {
    data.beginTime = beginTime;
  }

  if (endTime) {
    data.endTime = endTime;
  }

  if (uploadChannel) {
    data.uploadChannel = uploadChannel;
  }

  if (typeof localBack == 'boolean') {
    data.localBack = localBack;
  }

  if (typeof use == 'boolean') {
    data.use = use;
  }

  return request.get(`${controller}/list`, data);
};

export const checkFileUseState = id => {
  return request.post(`${controller}/check/usestate`, {
    id
  });
};

export const syncFileToRemote = id => {
  return request.post(`${controller}/syncfile/remote`, {
    id
  });
};

export const downAllFile = () => {
  return request.post(`${controller}/download/file/all`);
};
