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
  var timeout;
  return function () {
    var context = this;
    var args = arguments;

    if (timeout) clearTimeout(timeout);

    if (immediate) {
      // 判断是否执行过
      var flag = !timeout;
      timeout = setTimeout(function () {
        callback.apply(context, args);
      }, wait);

      if (flag) callback.apply(context, args);
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
    let tmp = (parseInt((count / 1000) * 100) / 100).toString();
    result = tmp.includes('.') ? `${tmp}K` : `${tmp}.0K`;
  } else if (count < 100000000) {
    let tmp = (parseInt((count / 10000) * 100) / 100).toString();
    result = tmp.includes('.') ? `${tmp}万+` : `${tmp}.0万+`;
  } else {
    let tmp = (parseInt((count / 100000000) * 100) / 100).toString();
    result = tmp.includes('.') ? `${tmp}亿+` : `${tmp}.0亿+`;
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
