function getCookie(name) {
  let cookieArr = document.cookie.split(';');
  for (let i = 0; i < cookieArr.length; i++) {
    let cookiePair = cookieArr[i].split('=');
    if (name == cookiePair[0].trim()) {
      return decodeURIComponent(cookiePair[1]);
    }
  }
  return '';
}

function setCookie(name, value, days = 7, path = '/') {
  let date = new Date();
  date.setTime(date.getTime() + days * 24 * 60 * 60 * 1000); // 设置cookie的过期时间
  let expires = '; expires=' + date.toUTCString(); // 过期时间的字符串表示
  document.cookie = name + '=' + encodeURIComponent(value) + expires + '; path=' + path; // 设置cookie
}

function setStaticFileResource(value) {
  setCookie('x-local', value);
}

export default {
  getCookie,
  setCookie,
  setStaticFileResource
};
