const success = config => {
  return config;
};

const error = err => {
  return Promise.reject(err);
};

export default {
  success,
  error
};
