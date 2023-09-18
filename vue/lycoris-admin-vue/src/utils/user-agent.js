const uaRegex = [
  {
    client: '猎豹浏览器',
    clientIcon: '/icon/ua/liebao.png',
    value: /LBBROWSER/,
    enum: 6
  },
  {
    client: 'QQ浏览器',
    clientIcon: '/icon/ua/qq.png',
    value: / QQBrowser /,
    enum: 5
  },
  {
    client: '百度浏览器',
    clientIcon: '/icon/ua/baidu.png',
    value: /BIDUBrowser|baidubrowser|BaiduHD/,
    enum: 4
  },
  {
    client: 'UC浏览器',
    clientIcon: '/icon/ua/uc.png',
    value: /UBrowser|UCBrowser|UCWEB/,
    enum: 3
  },
  {
    client: '小米浏览器',
    clientIcon: '/icon/ua/android.png',
    value: /MiuiBrowser/,
    enum: 11
  },
  {
    client: '微信',
    clientIcon: '/icon/ua/android.png',
    value: /MicroMessenger/,
    enum: 12
  },
  {
    client: '手机QQ',
    clientIcon: '/icon/ua/qq.png',
    value: /Mobile\/\w{5,}\sQQ\/(\d+[.\d]+)/,
    enum: 13
  },
  {
    client: '手机百度',
    clientIcon: '/icon/ua/baidu.png',
    value: /baiduboxapp/,
    enum: 14
  },
  {
    client: '火狐浏览器',
    clientIcon: '/icon/ua/firefox.png',
    value: /Firefox/,
    enum: 7
  },
  {
    client: '360安全浏览器',
    clientIcon: '/icon/ua/360.png',
    value: /360SE/,
    enum: 8
  },
  {
    client: '360极速浏览器',
    clientIcon: '/icon/ua/360.png',
    value: /360EE/,
    enum: 9
  },
  {
    client: 'Opera',
    clientIcon: '/icon/ua/opera.png',
    value: /Opera|OPR\/(\d+[.\d]+)/,
    enum: 10
  },
  {
    client: 'Edge',
    clientIcon: '/icon/ua/edge.png',
    value: /Edg/,
    enum: 0
  },
  {
    client: '安卓浏览器',
    clientIcon: '/icon/ua/android.png',
    value: /Android.*Mobile\sSafari|Android\/(\d[.\d]+)\sRelease\/(\d[.\d]+)\sBrowser\/AppleWebKit(\d[.\d]+)/i,
    enum: 16
  },
  {
    client: 'IE浏览器',
    clientIcon: '/icon/ua/ie.png',
    value: /Trident|MSIE/,
    enum: 15
  },
  {
    client: '谷歌浏览器',
    clientIcon: '/icon/ua/chrome.png',
    value: /Chrome|CriOS/,
    enum: 1
  },
  {
    client: 'Safari',
    clientIcon: '/icon/ua/safari.png',
    value: /Version[|/]([\w.]+)(\s\w.+)?\s?Safari|like\sGecko\)\sMobile\/\w{3,}$/,
    enum: 2
  }
];

export const getUserAgent = ua => {
  let client = void 0;
  for (let item of uaRegex) {
    if (item.value.exec(ua)) {
      client = item;
      break;
    }
  }

  return client;
};

export const getUserAgentIcon = ua => {
  let item = getUserAgent(ua);
  return item?.clientIcon || '/icon/ua/edge.png';
};

export const getUserAgentEnum = ua => {
  let item = getUserAgent(ua);
  return item.enum;
};

export const getUserAgentIconByEnum = $enum => {
  let filter = uaRegex.filter(x => x.enum == $enum);
  if (filter && filter.length > 0) {
    return filter[0].clientIcon;
  } else {
    return '';
  }
};
