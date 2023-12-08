import { defineStore } from 'pinia';

const datakey = 'l-setting';
export default defineStore('web-setting', {
  state: () => {
    let emptyValue = {
      webName: '',
      webPath: '',
      adminPath: '',
      logo: '',
      icp: ''
    };
    let value = localStorage.getItem(datakey);

    try {
      value = JSON.parse(value);
      return value ? value : emptyValue;
    } catch {
      return emptyValue;
    }
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

      localStorage.setItem(datakey, {
        webName: this.webName,
        webPath: this.webPath,
        adminPath: this.adminPath,
        logo: this.logo,
        icp: this.icp
      });
    }
  }
});
