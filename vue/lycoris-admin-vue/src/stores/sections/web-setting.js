import { defineStore } from 'pinia';
import { api } from '../../config.json';

const dataKey = 'l-web';

export default defineStore('web-setting', {
  state: () => {
    let value = localStorage.getItem(dataKey);
    if (value) {
      value = JSON.parse(value);
    } else {
      value = {
        webName: '',
        webPath: '',
        logo: '',
        icp: ''
      };
    }

    return value;
  },
  actions: {
    setData({ webName, webPath, logo, icp }) {
      if (webName) {
        this.webName = webName;
      }

      if (webPath) {
        this.webPath = webPath.trimEnd('/');
      }

      if (logo) {
        this.logo = `${api.server}${logo}`;
      }

      if (icp) {
        this.icp = icp;
      }

      localStorage.setItem(
        dataKey,
        JSON.stringify({
          webName: this.webName,
          webPath: this.webPath,
          icp: this.icp,
          logo: this.logo
        })
      );
    }
  }
});
