import { stores } from '../stores';

const setTitle = title => {
  let temp = '';
  if (title) {
    temp = `${title}_${stores.webSetting.webName}`;
  } else {
    temp = stores.webSetting.webName;
  }

  if (temp != document.title) {
    document.title = temp;
  }
};

const setKeywords = keywords => {
  document.querySelector('meta[name="keywords"]').setAttribute('content', keywords);
};

const setDescription = description => {
  document.querySelector('meta[name="description"]').setAttribute('content', description);
};

const setFavicon = () => {
  if (stores.webSetting.favicon) {
    var link = document.createElement('link');
    link.type = 'image/x-icon';
    link.rel = 'shortcut icon';
    link.href = stores.webSetting.favicon;
    document.getElementsByTagName('head')[0].appendChild(link);
  }
};

export default {
  setTitle,
  setKeywords,
  setDescription,
  setFavicon
};
