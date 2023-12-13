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
        return callback.apply(context, args);
      }, wait);

      if (flag) callback.apply(context, args);
    } else {
      timeout = setTimeout(function () {
        return callback.apply(context, args);
      }, wait);
    }
  };
};

/**
 * 节流
 * @param {*} callback
 * @param {*} delay
 * @returns
 */
export const throttle = (callback, delay) => {
  let lastCallTime = 0;

  return function (...args) {
    const now = +new Date();

    if (now - lastCallTime >= delay) {
      lastCallTime = now;
      return callback.apply(this, args);
    } else {
      return Promise.reject();
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

export const scrollPageTop = (step = 100, duration = 10) => {
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

export const getTimeLeft = (sourceDate, targetDate) => {
  if (typeof sourceDate == 'string') {
    sourceDate = new Date(sourceDate).getTime();
  }

  if (typeof targetDate == 'string') {
    targetDate = new Date(targetDate).getTime();
  }

  const timeDifference = 0 + targetDate - (0 + sourceDate);

  const days = Math.floor(timeDifference / (1000 * 60 * 60 * 24));
  const hours = Math.floor((timeDifference % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60));
  const minutes = Math.floor((timeDifference % (1000 * 60 * 60)) / (1000 * 60));
  const seconds = Math.floor((timeDifference % (1000 * 60)) / 1000);

  return {
    days,
    hours,
    minutes,
    seconds
  };
};

export const animateNumber = el => {
  const that = {
    el: el,
    currentNumber: 0,
    _increment: 0,
    _targetNumber: 0,
    minStep: void 0,
    complete: void 0
  };

  that.paly = (duration = 2000, targetNumber, sourceNumber) => {
    if (sourceNumber == undefined && that.sourceNumberHandle && typeof that.sourceNumberHandle == 'function') {
      that.currentNumber = that.sourceNumberHandle(that.el);
      that.currentNumber = parseFloat(that.currentNumber);
      that.currentNumber = isNaN(that.currentNumber) ? 0 : that.currentNumber;
    } else {
      that.currentNumber = sourceNumber == undefined || 0;
    }

    if (that.targetNumberHandle && typeof that.targetNumberHandle == 'function') {
      that._targetNumber = that.targetNumberHandle(that.el);
      that._targetNumber = parseFloat(that._targetNumber);
      that._targetNumber = isNaN(that._targetNumber) ? 0 : that._targetNumber;
    } else {
      that.currentNumber = targetNumber;
    }

    if (that.currentNumber < that._targetNumber) {
      // 计算每一帧的增量
      that._increment = (that._targetNumber - that.currentNumber) / (duration / 16);

      if (that.minStep && that.minStep > that._increment) {
        that._increment = that.minStep;
      }

      // 启动动画
      animateNumber();
    }
  };

  that.sourceNumberHandle = function () {
    return parseFloat(that.el.innerText);
  };

  that.targetNumberHandle = void 0;

  // 创建动画函数
  function animateNumber() {
    // 更新当前数字
    that.currentNumber += that._increment;

    // 更新显示的数字
    that.el.textContent = Math.floor(that.currentNumber);

    // 如果当前数字小于目标数字，则继续动画
    if (that.currentNumber < that._targetNumber) {
      requestAnimationFrame(animateNumber);
    } else if (that.complete && typeof that.complete == 'function') {
      that.complete();
    }
  }

  return that;
};

export const typewriter = el => {
  let str = el.innerHTML.toString();
  let i = 0;
  el.innerHTML = '';

  setTimeout(function () {
    var se = setInterval(function () {
      i++;
      el.innerHTML = str.slice(0, i) + '|';
      if (i == str.length) {
        clearInterval(se);
        el.innerHTML = str;
      }
    }, 10);
  }, 0);
};
