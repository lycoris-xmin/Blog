import { defineStore } from 'pinia';

export default defineStore('web-setting', {
  state: () => {
    return {
      webName: '',
      webPath: '',
      adminPath: '',
      logo: '',
      icp: ''
    };
  },
  actions: {
    setData(data) {
      if (data.webName) {
        this.webName = data.webName;
      }

      if (data.webPath) {
        this.webPath = data.webPath.trimEnd('/');
      }

      if (data.adminPath) {
        this.adminPath = data.adminPath;
      }

      if (data.logo) {
        this.logo = data.logo;
      }

      if (data.icp) {
        this.icp = data.icp;
      }
    }
  }
});
