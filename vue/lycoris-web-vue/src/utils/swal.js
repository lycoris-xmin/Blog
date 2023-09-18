import { ElMessageBox } from 'element-plus';

const getArgs = args => {
  let message = args[0],
    title = '',
    config = {};

  for (let i = 1; i < args.length; i++) {
    let type = typeof args[i];
    if (type == 'string') {
      title = args[i];
    } else if (type == 'object') {
      config = args[i];
    }
  }

  return { message, title, config };
};

const info = function () {
  let { message, title, config } = getArgs(arguments);
  title = title || '提示';
  config = config || {};

  let option = {
    type: 'info',
    confirmButtonText: '确定',
    draggable: true
  };

  Object.assign(option, config);

  return ElMessageBox.alert(message, title, option);
};

const success = function () {
  let { message, title, config } = getArgs(arguments);

  title = title || '处理成功';
  config = config || {};

  let option = {
    type: 'success',
    confirmButtonText: '确定',
    draggable: true
  };

  Object.assign(option, config);

  return ElMessageBox.alert(message, title, option);
};

const warn = function () {
  let { message, title, config } = getArgs(arguments);

  title = title || '警告';
  config = config || {};

  let option = {
    type: 'warning',
    confirmButtonText: '确定',
    draggable: true
  };

  Object.assign(option, config);

  return ElMessageBox.alert(message, title, option);
};

const error = function () {
  let { message, title, config } = getArgs(arguments);

  title = title || '错误';
  config = config || {};

  let option = {
    type: 'error',
    confirmButtonText: '确定',
    draggable: true
  };

  Object.assign(option, config);

  return ElMessageBox.alert(message, title, option);
};

const confirm = function () {
  let { message, title, config } = getArgs(arguments);

  title = title || '警告';
  config = config || {};

  let option = {
    type: 'warning',
    cancelButtonText: '取消',
    confirmButtonText: '确定',
    draggable: true
  };

  Object.assign(option, config);

  return ElMessageBox.confirm(message, title, option);
};

export default {
  info,
  success,
  warn,
  error,
  confirm
};
