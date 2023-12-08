import request from '../utils/request';
const controller = '/sitenavigation';

export const getList = ({ name, groupId, domain, pageIndex, pageSize }) => {
  let data = {
    pageIndex,
    pageSize
  };

  if (name) {
    data.name = name;
  }

  if (groupId != undefined && groupId > 0) {
    data.groupId = groupId;
  }

  if (domain) {
    data.domain = domain;
  }

  return request.get(`${controller}/query/list`, data);
};

export const createSiteNavigation = ({ name, url, groupId, groupName }) => {
  return request.post(`${controller}/create`, { name, url, groupId, groupName });
};

export const updateSiteNavigation = ({ id, name, url, groupId, groupName }) => {
  let data = { id };

  if (name) {
    data.name = name;
  }

  if (url) {
    data.url = url;
  }

  if (groupId != undefined && typeof groupId == 'number') {
    data.groupId = groupId;
  }

  if (groupName) {
    data.groupName = groupName;
  }

  return request.post(`${controller}/update`, data);
};

export const deleteSiteNavigation = id => {
  return request.post(`${controller}/delete`, { id });
};

export const getGroups = () => {
  return request.get(`${controller}/enum/group`);
};

export const setGroupOrder = ids => {
  return request.post(`${controller}/grouporder`, { ids });
};

export const deleteGroup = id => {
  return request.post(`${controller}/group/delete`, { id });
};
