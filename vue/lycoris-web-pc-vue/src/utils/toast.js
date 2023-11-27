import { ElMessage } from 'element-plus';

const info = (message, config) => {
  if (config && config.type) {
    delete config.type;
  }

  if (config?.max && document.getElementsByClassName('el-message').length >= config?.max) {
    return;
  }

  ElMessage({
    message: message,
    ...config
  });
};

const success = (message, config) => {
  if (config?.max && document.getElementsByClassName('el-message').length >= config?.max) {
    return;
  }

  ElMessage({
    message: message,
    type: 'success',
    ...config
  });
};

const warn = (message, config) => {
  if (config?.max && document.getElementsByClassName('el-message').length >= config?.max) {
    return;
  }

  ElMessage({
    message: message,
    type: 'warning',
    ...config
  });
};

const error = (message, config) => {
  if (config?.max && document.getElementsByClassName('el-message').length >= config?.max) {
    return;
  }

  ElMessage({
    message: message,
    type: 'error',
    ...config
  });
};

export default {
  info,
  success,
  warn,
  error
};
