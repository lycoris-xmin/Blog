Date.prototype.format = function (fmt) {
  //author: meizz
  var o = {
    'M+': this.getMonth() + 1, //月份
    'd+': this.getDate(), //日
    'H+': this.getHours(), //小时
    'm+': this.getMinutes(), //分
    's+': this.getSeconds(), //秒
    'q+': Math.floor((this.getMonth() + 3) / 3), //季度
    S: this.getMilliseconds() //毫秒
  };
  if (/(y+)/.test(fmt)) fmt = fmt.replace(RegExp.$1, (this.getFullYear() + '').substr(4 - RegExp.$1.length));
  for (var k in o) if (new RegExp('(' + k + ')').test(fmt)) fmt = fmt.replace(RegExp.$1, RegExp.$1.length == 1 ? o[k] : ('00' + o[k]).substr(('' + o[k]).length));
  return fmt;
};

Date.prototype.addDays = function (days) {
  let time = this.getTime();
  time += parseInt(days) * (24 * 60 * 60 * 1000);
  return new Date(time);
};

Date.prototype.addHours = function (hours) {
  let time = this.getTime();
  time += parseInt(hours) * (60 * 60 * 1000);
  return new Date(time);
};

Date.prototype.addMinutes = function (minutes) {
  let time = this.getTime();
  time += parseInt(minutes) * (60 * 1000);
  return new Date(time);
};

Date.prototype.addSeconds = function (seconds) {
  let time = this.getTime();
  time += parseInt(seconds) * 1000;
  return new Date(time);
};

Array.prototype._sortBy = function (property, asc) {
  if (asc == undefined) {
    asc = -1;
  } else {
    asc = asc ? -1 : 1;
  }
  return function (value1, value2) {
    let a = value1[property];
    let b = value2[property];
    return a < b ? asc : a > b ? asc * -1 : 0;
  };
};

/**
 *
 * @param {*} property
 * @param {String} orderType
 */
Array.prototype.orderBy = function (property) {
  return this.sort(this._sortBy(property, true));
};

/**
 *
 * @param {*} property
 * @param {String} orderType
 */
Array.prototype.orderDescBy = function (property) {
  return this.sort(this._sortBy(property, false));
};
