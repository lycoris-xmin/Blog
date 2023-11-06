function isNull(o) {
  return o == null || o === '' || o === 'undefined' || o === 'null';
}

/**
 * 防抖
 * @param {*} callback
 * @param {*} wait
 * @param {*} immediate
 * @returns
 */
export const debounce = (callback, wait, immediate = false) => {
  let timeout;

  return function () {
    const context = this;
    const args = arguments;

    if (timeout) {
      clearTimeout(timeout);
    }

    if (immediate) {
      // 判断是否执行过
      let flag = !timeout;
      timeout = setTimeout(function () {
        callback.apply(context, args);
      }, wait);

      if (flag) {
        callback.apply(context, args);
      }
    } else {
      timeout = setTimeout(function () {
        callback.apply(context, args);
      }, wait);
    }
  };
};

/**
 * 节流
 * @param {*} callback
 * @param {*} delay
 * @param {*} immediate
 * @returns
 */
export const throttle = (callback, delay, immediate = true) => {
  var timer,
    context,
    iNow,
    firstTime = +new Date(),
    args = [];
  return function () {
    clearTimeout(timer);
    context = this;
    iNow = +new Date();
    args = Array.prototype.slice.call(arguments);
    // 判断是否是第一次执行
    if (immediate) {
      immediate = false;
      callback.apply(context, args);
    } else {
      // 第二次执行的时候判断时间差
      if (iNow - firstTime > delay) {
        firstTime = iNow;
        callback.apply(context, args);
      } else {
        // 判断是否是最后一次执行
        timer = setTimeout(function () {
          callback.apply(context, args);
        }, delay);
      }
    }
  };
};

export const countChange = function formatCount(count) {
  if (isNull(count)) {
    return '0';
  }

  if (typeof count !== 'number') {
    count = Number(count);
  }

  if (isNaN(count)) {
    return '';
  }

  if (count == 0) {
    return '0';
  }

  let result = '';
  if (count < 1000) {
    result = count.toString();
  } else if (count < 10000) {
    result = `${parseInt((count / 1000) * 10) / 10}K`;
  } else if (count < 100000000) {
    result = `${parseInt((count / 10000) * 10) / 10}万+`;
  } else {
    result = `${parseInt((count / 100000000) * 10) / 10}亿+`;
  }

  return result;
};

export const isMobile = () => {
  return navigator.userAgent.match(/(phone|pad|pod|iPhone|iPod|ios|iPad|Android|Mobile|BlackBerry|IEMobile|MQQBrowser|JUC|Fennec|wOSBrowser|BrowserNG|WebOS|Symbian|Windows Phone)/i);
};

export const isInViewPortOfOne = el => {
  // viewPortHeight 兼容所有浏览器写法
  const viewPortHeight = window.innerHeight || document.documentElement.clientHeight || document.body.clientHeight;
  const offsetTop = el.offsetTop;
  const scrollTop = document.documentElement.scrollTop;
  const top = offsetTop - scrollTop;
  return top <= viewPortHeight;
};

export const isInViewPort = element => {
  const viewWidth = window.innerWidth || document.documentElement.clientWidth;
  const viewHeight = window.innerHeight || document.documentElement.clientHeight;
  const { top, right, bottom, left } = element.getBoundingClientRect();
  return top >= 0 && left >= 0 && right <= viewWidth && bottom <= viewHeight;
};

export const scrollPageTop = (step = 50, duration = 10) => {
  let start = document.documentElement.scrollTop;
  let scrollInterval = setInterval(() => {
    start -= step;
    window.scrollTo(0, start < 0 ? 0 : start);
    if (start <= 0) {
      clearInterval(scrollInterval);
    }
  }, duration);
};

export const uuid = () => {
  let s = [];
  var hexDigits = '0123456789abcdef';
  for (var i = 0; i < 36; i++) {
    let start = Math.floor(Math.random() * 0x10);
    s[i] = hexDigits.substring(start, start + 1);
  }
  s[14] = '4';
  let index = (s[19] & 0x3) | 0x8;
  s[19] = hexDigits.substring(index, index + 1);
  s[8] = s[13] = s[18] = s[23] = '-';
  var uuid = s.join('');
  return uuid.replaceAll('-', '');
};

export const lockScroll = () => {
  let widthBar = 17,
    root = document.documentElement;
  if (typeof window.innerWidth == 'number') {
    widthBar = window.innerWidth - root.clientWidth;
  }
  root.style.overflow = 'hidden';
  root.style.borderRight = widthBar + 'px solid transparent';
};

export const unlockScroll = () => {
  let root = document.documentElement;
  root.style.overflow = '';
  root.style.borderRight = '';
};
