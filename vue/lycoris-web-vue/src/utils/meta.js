import { web } from '../config.json';

const setTitle = title => {
  let temp = '';
  if (title) {
    temp = `${title}_${web.name}`;
  } else {
    temp = web.name;
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

export default {
  setTitle,
  setKeywords,
  setDescription
};
