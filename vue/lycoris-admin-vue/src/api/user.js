import request from '../utils/request';

const controller = '/user';

export const getUserBrief = () => {
  return request.get(`${controller}/dashboard/brief`);
};

export const updateUserBrief = ({ nickName, avatar, blog, github, gitee, qq, wechat, cloudMusic, bilibili }) => {
  return request.post(`${controller}/dashboard/brief/update`, { nickName, avatar, blog, github, gitee, qq, wechat, cloudMusic, bilibili });
};

export const getList = ({ pageIndex, pageSize, nickName, email }) => {
  let data = {
    pageIndex,
    pageSize
  };
  if (nickName) {
    data.nickName = nickName;
  }

  if (email) {
    data.email = email;
  }

  return request.get(`${controller}/list`, data);
};

export const getUserLink = id => {
  return request.get(`${controller}/link`, { id });
};

export const getUserStatusEnum = () => {
  return request.get(`${controller}/status/enum`);
};

export const createUser = ({ email, nickName, password }) => {
  let data = {
    email,
    nickName
  };

  if (password) {
    data.password = password;
  }

  return request.post(`${controller}/create`, data);
};

export const auditUser = ({ id, status, remark }) => {
  let data = {
    id,
    status
  };

  if (remark) {
    data.remark = remark;
  }

  return request.post(`${controller}/audit`, data);
};
