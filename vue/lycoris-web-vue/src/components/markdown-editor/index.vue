<template>
  <div :id="container" class="cherry-markdown-container" :class="{ 'cherry-markdown-container-border': !props.previewOnly }" @keydown="handleKeyDown"></div>
</template>

<script setup>
import { ref } from 'vue';
import 'cherry-markdown/dist/cherry-markdown.min.css';
import CherryMarkdown from 'cherry-markdown';
import $loading from '../../utils/loading';

const container = ref('cherry-markdown-container');

const loading = new $loading({
  target: `#${container.value}`,
  text: 'markdown 插件初始化...'
});

const props = defineProps({
  toolbars: {
    type: Object,
    default: void 0
  },
  events: {
    type: Object,
    default: void 0
  },
  options: {
    type: Object,
    default: () => {
      return {};
    }
  },
  previewOnly: {
    type: Boolean,
    default: true
  },
  customMenu: {
    type: Array,
    default: void 0
  }
});

const emit = defineEmits(['fileUpload', 'keydown-save']);

const cherry = {
  instance: '',
  options: {
    id: container.value,
    value: '',
    editor: {
      codemirror: {
        theme: 'default'
      },
      defaultModel: props.previewOnly ? 'previewOnly' : 'edit&preview',
      convertWhenPaste: true
    },
    engine: {
      global: {
        htmlWhiteList: 'iframe|script|style'
      },
      syntax: {
        list: {
          listNested: true
        },
        inlineCode: {
          theme: 'red'
        },
        codeBlock: {
          theme: 'dark',
          wrap: true,
          lineNumber: true
        }
      }
    },
    toolbars: {
      theme: 'light ',
      showToolbar: true,
      toolbar: [
        'bold',
        'size',
        'color',
        'header',
        '|',
        'hr',
        'strikethrough',
        '|',
        'code',
        'link',
        'table',
        '|',
        'list',
        {
          insert: ['image', 'audio', 'video', 'pdf', 'word', 'formula']
        },
        'graph',
        'togglePreview',
        'export'
      ],
      sidebar: [],
      bubble: false,
      float: false
    },
    fileUpload: (file, callback) => {
      emit('fileUpload', file, url => {
        callback(url);
      });
    },
    callback: {}
  }
};

const init = (value = '') => {
  if (!props.previewOnly) {
    loading.show();
  }

  try {
    Object.assign(cherry, props.options);

    cherry.options.value = value;

    if (props.toolbars) {
      cherry.options.toolbars = props.toolbars;
    }

    if (props.events) {
      cherry.options.callback = props.events || {};
    }

    if (props.customMenu) {
      let tools = generateCustomeMenu();
      if (tools) {
        cherry.options.toolbars.customMenu = tools;
        for (let tool in tools) {
          cherry.options.toolbars.toolbar.push(tool);
        }
      }
    }

    cherry.instance = new CherryMarkdown(cherry.options);
  } catch (error) {
    console.log(error);
  } finally {
    if (!props.previewOnly) {
      loading.hide();
    }
  }
};

const generateCustomeMenu = () => {
  let tools = {};
  for (let item of props.customMenu) {
    let tmp = CherryMarkdown.createMenuHook(item.label, {
      iconName: item.icon || '',
      onClick(selection) {
        if (item.callback) {
          item.callback(selection);
        }
      }
    });

    tools[item.name] = tmp;
  }

  return tools;
};

const setMarkdown = value => {
  cherry.options.value = value;
  cherry.instance = new CherryMarkdown(cherry.options);
};

const getMarkdown = () => {
  return cherry.instance.getMarkdown();
};

const getHtml = () => {
  return cherry.instance.getHtml();
};

const exportPage = (type = 'pdf') => {
  type = type != 'pdf' && type != 'img' ? 'pdf' : type;
  return cherry.instance.export(type);
};

//{'edit&preview'|'editOnly'|'previewOnly'}
const changModel = model => {
  if (cherry.options.editor.defaultModel != model) {
    cherry.instance.switchModel(model);
    cherry.options.editor.defaultModel = model;
  }
};

const getInfoText = length => {
  let re = new RegExp('<[^<>]+>', 'g');
  let html = cherry.instance.getHtml();
  let text = html.replace(re, '');
  text = text.replace(/\s*/g, '');

  if (text.length > length) {
    text = text.substring(0, length);
  }

  return text;
};

const getTopic = () => {
  const topics = [];

  const allTopic = document.querySelector('div.cherry-previewer').querySelectorAll('h1,h2');

  let data = {};
  for (let topic of allTopic) {
    data = {
      level: parseInt(topic.tagName.replace('H', '')),
      id: topic.getAttribute('id'),
      text: topic.innerText
    };

    if (topics.length > 0) {
      if (topics[topics.length - 1].level < data.level) {
        if (!topics[topics.length - 1].child) {
          topics[topics.length - 1].child = [];
        }

        topics[topics.length - 1].child.push(data);
        continue;
      }
    }

    topics.push(data);
  }

  return topics.map(x => {
    let child = x.child?.map(y => {
      return {
        id: y.id,
        text: y.text
      };
    });

    return {
      id: x.id,
      text: x.text,
      child: child || []
    };
  });
};

const handleKeyDown = event => {
  if (event.ctrlKey && event.key === 's') {
    event.preventDefault();
    emit('keydown-save');
  }
};

const setImgPreview = callback => {
  const imgs = document.querySelector('div.cherry-previewer').querySelectorAll('img');
  if (imgs.length > 0) {
    imgs.forEach(el => {
      el.parentNode.style.display = 'flex';
      el.parentNode.style['justify-content'] = 'center';
      el.parentNode.style['align-items'] = 'center';

      el.addEventListener('click', function () {
        callback.call(this);
      });

      el.addEventListener('error', function () {
        this.src = '/images/404.png';
        this.style.width = '300px';
      });
    });
  }
};

const showLoading = () => {
  loading.show();
};

const hideLoading = () => {
  loading.hide();
};

defineExpose({
  init,
  setMarkdown,
  getMarkdown,
  getHtml,
  getInfoText,
  getTopic,
  exportPage,
  changModel,
  setImgPreview,
  showLoading,
  hideLoading
});
</script>

<style lang="scss">
.cherry-markdown-container {
  height: 100%;

  .cherry {
    background: #fff !important;
    box-shadow: 0 0 0 #f6f8fb;

    .cherry-toolbar {
      border-bottom: 2px solid var(--color-secondary);
    }

    .cherry-editor {
      .CodeMirror-lines {
        padding: 15px;
      }
    }

    .cherry-previewer {
      background-color: #fff;
      border-left: 0;

      img {
        cursor: pointer;
        border-radius: 5px;
      }

      ul.cherry-list__default {
        li {
          padding-left: 10px;
        }

        li::marker {
          content: '>';
          color: var(--color-danger);
        }
      }
    }

    .cherry-previewer.cherry-preview--full {
      overflow-x: hidden;
    }

    .cherry-markdown {
      h1,
      h2,
      h3,
      h4,
      h5,
      h6 {
        transition: color 0.35s;
        cursor: default;
      }

      p,
      pre,
      blockquote,
      table {
        margin: 15px auto;
      }

      div[data-type='codeBlock'] {
        > pre code[class*='language-'] .code-line {
          padding-left: 2em !important;
        }

        .cherry-copy-code-block {
          position: relative;
          width: 25px;
          text-align: center;
          height: 25px;
          border: 1px solid #ddd;
          cursor: pointer;
          float: right;
          right: 10px;
          top: 22px;
          color: #f6f8fb;
          border-radius: 5px;
          margin-left: -27px;
          transition: all 0.3s;
          z-index: 1;
        }

        .cherry-copy-code-block .ch-icon {
          vertical-align: middle;
          cursor: pointer;
        }
      }

      div[data-type='codeBlock']:hover {
        .cherry-copy-code-block {
          display: block !important;
          cursor: pointer;
        }

        .cherry-copy-code-block .ch-icon-ok {
          color: #3582fb;
        }
      }

      .menu-click-color {
        color: #926dde;
      }

      h1 > a.anchor,
      h2 > a.anchor,
      h3 > a.anchor,
      h4 > a.anchor,
      h5 > a.anchor,
      h6 > a.anchor {
        display: none;
      }

      a {
        color: var(--color-purple);
      }

      a:active,
      a:hover {
        color: var(--color-purple-light);
      }

      h1 {
        padding: 25px 0;
        margin: 0;
        border-radius: 5px;
      }

      h1:before {
        content: '[';
        margin-right: 8px;
        color: var(--color-danger) !important;
      }

      h1:after {
        content: ']';
        margin-left: 8px;
        color: var(--color-danger) !important;
      }

      h2 {
        padding: 20px 0;
        margin: 0;
      }

      h2:before {
        content: '#';
        margin-right: 8px;
        color: var(--color-danger) !important;
      }

      h3 {
        color: #6d6c6c;
        background: #d1edc4;
        padding: 15px 20px 15px 10px;
        font-weight: normal;
        font-size: 16px;
        border-radius: 5px;
        word-wrap: break-word;
        word-break: break-all;
        overflow: hidden;
      }

      h3:before {
        content: '$';
        margin-right: 8px;
        color: var(--color-danger) !important;
      }

      h4 {
        background-color: #c8e6fb !important;
        padding: 10px;
        border-radius: 5px;
        font-weight: 600;
      }

      h5 {
        background-color: #fff6b5 !important;
        padding: 10px;
        border-radius: 5px;
        font-weight: 600;
      }

      h6 {
        background-color: var(--color-danger) !important;
        padding: 10px;
        border-radius: 5px;
        font-weight: 600;
      }

      h4 > strong,
      h5 > strong,
      h6 > strong {
        font-size: 16px;
        font-weight: 600;
        line-height: 24px;
      }
    }

    .cherry-table {
      width: 100%;

      th {
        padding: 5px 10px !important;
      }

      td {
        padding: 5px 10px !important;
      }
    }

    .cherry-panel {
      .cherry-panel--title {
        font-size: 18px;
        font-weight: 500;
        padding: 10px 2px 10px 10px;
      }

      .cherry-panel--title.cherry-panel--title__not-empty::before {
        margin: 0 5px;
      }

      .cherry-panel--body {
        color: var(--color-dark) !important;
        background-color: #fff !important;
      }
    }

    .cherry-panel__primary {
      .cherry-panel--title {
        background-color: var(--color-info);
      }
    }

    .cherry-panel__success {
      .cherry-panel--title {
        background-color: #15c377;
      }
    }

    p {
      del {
        color: var(--color-danger);
        padding: 0 10px;
      }
    }
  }

  pre {
    * {
      font-family: Consolas, Monaco, Andale Mono, Ubuntu Mono, monospace !important;
      font-weight: 500;
    }
  }
}

.cherry-markdown-container-border {
  border: 2px solid var(--color-secondary) !important;
}
</style>
