import { defineStore } from 'pinia';

export default defineStore('webSetting', {
  state: () => {
    return {
      webName: '',
      logo: '',
      favicon: '',
      icp: '',
      description: '',
      buildTime: '',
      defaultAvatar: '/avatar/default_admin.jpeg'
    };
  },
  actions: {
    setData(data) {
      this.webName = data.webName;
      this.logo = data.logo || '/logo/logo-lycoirs.png';
      this.favicon = data.favicon;
      this.icp = data.icp;
      this.buildTime = data.buildTime;
      this.description = data.description || '';
      if (data.defaultAvatar) {
        this.defaultAvatar = data.defaultAvatar;
      }
    }
  }
});
