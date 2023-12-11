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
    setData({ webName, webPath, adminPath, logo, icp }) {
      if (webName) {
        this.webName = webName;
      }

      if (webPath) {
        this.webPath = webPath.trimEnd('/');
      }

      if (adminPath) {
        this.adminPath = adminPath;
      }

      if (logo) {
        this.logo = logo;
      }

      if (icp) {
        this.icp = icp;
      }
    }
  }
});
