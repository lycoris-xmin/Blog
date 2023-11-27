import vClickOutSide from './vClickOutSide';

const directives = {
  clickOutSide: vClickOutSide
};

export default {
  install(app) {
    Object.keys(directives).forEach(key => {
      app.directive(key, directives[key]);
    });
  }
};
