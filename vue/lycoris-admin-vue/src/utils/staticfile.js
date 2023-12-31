const storageKey = 'static-local';

export const setStaticSource = value => {
  setStaticSourceCookie(value);
  localStorage.setItem(storageKey, value);
};

const setStaticSourceCookie = value => {
  if (value == 'cdn') {
    let exp = new Date();
    exp.setTime(exp.getTime() - 1);
    document.cookie = `x-local=0;path=/;expires=${exp.toGMTString()}`;
  } else {
    document.cookie = 'x-local=1;path=/';
  }

  localStorage.setItem(storageKey, value);
};
