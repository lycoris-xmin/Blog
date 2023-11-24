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

  title = title || '提示';

  return { message, title, config };
};

const info = async function () {
  try {
    let { message, title, config } = getArgs(arguments);
    title = title || '提示';
    config = config || {};

    let option = {
      type: 'info',
      confirmButtonText: '确定',
      draggable: true
    };

    Object.assign(option, config);

    return await ElMessageBox.alert(message, title, option);
  } catch (error) {}
};

const success = async function () {
  try {
    let { message, title, config } = getArgs(arguments);

    title = title || '处理成功';
    config = config || {};

    let option = {
      type: 'success',
      confirmButtonText: '确定',
      draggable: true
    };

    Object.assign(option, config);

    return await ElMessageBox.alert(message, title, option);
  } catch (error) {}
};

const warn = async function () {
  try {
    let { message, title, config } = getArgs(arguments);

    title = title || '警告';
    config = config || {};

    let option = {
      type: 'warning',
      confirmButtonText: '确定',
      draggable: true
    };

    Object.assign(option, config);

    return await ElMessageBox.alert(message, title, option);
  } catch (error) {}
};

const error = async function () {
  try {
    let { message, title, config } = getArgs(arguments);

    title = title || '错误';
    config = config || {};

    let option = {
      type: 'error',
      confirmButtonText: '确定',
      draggable: true
    };

    Object.assign(option, config);

    return await ElMessageBox.alert(message, title, option);
  } catch (error) {}
};

const confirm = async function () {
  try {
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

    return await ElMessageBox.confirm(message, title, option);
  } catch (error) {
    return false;
  }
};

export default {
  info,
  success,
  warn,
  error,
  confirm
};
