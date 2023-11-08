import request from '../utils/request';
const controller = '/sitenavigation';

export const getList = ({ name, group, domain, pageIndex, pageSize }) => {
  let data = {
    pageIndex,
    pageSize
  };

  if (name) {
    data.name = name;
  }

  if (group) {
    data.group = group;
  }

  if (domain) {
    data.domain = domain;
  }

  return request.get(`${controller}/query/list`, data);
};

export const createSiteNavigation = ({ name, group, domain }) => {
  return request.post(`${controller}/create`, { name, group, domain });
};

export const updateSiteNavigation = ({ id, name, group, domain }) => {
  return request.post(`${controller}/update`, { id, name, group, domain });
};

export const deleteSiteNavigation = id => {
  return request.post(`${controller}/delete`, { id });
};

export const getGroupOptions = () => {
  return request.get(`${controller}/enum/group`);
};
