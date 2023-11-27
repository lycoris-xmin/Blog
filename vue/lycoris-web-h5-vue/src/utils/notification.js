import { ElNotification } from 'element-plus';

class notice {
  constructor(config) {
    this.config = config || {};
    this.config.onClick = () => {
      if (config.callback) {
        config.callback(this.config.notification);
      }
    };
  }

  show = (message, title) => {
    if (title) {
      this.config.title = title;
    }
    this.config.message = message;
    this.config.notification = ElNotification(this.config);
  };

  onClick = callback => {
    if (!this.config.onClick) {
      this.config.onClick = callback;
    }
  };
}

const success = (title, message, config) => {
  let option = {
    title: title,
    message: message,
    type: 'success'
  };

  Object.assign(option, config);

  ElNotification(option);
};

const warn = (title, message, config) => {
  let option = {
    title: title,
    message: message,
    type: 'warning'
  };

  Object.assign(option, config);

  ElNotification(option);
};

const info = (title, message, config) => {
  let option = {
    title: title,
    message: message,
    type: 'info'
  };

  Object.assign(option, config);

  ElNotification(option);
};

const error = (title, message, config) => {
  let option = {
    title: title,
    message: message,
    type: 'error'
  };

  Object.assign(option, config);

  ElNotification(option);
};

export default {
  instance: notice,
  success,
  warn,
  info,
  error
};
