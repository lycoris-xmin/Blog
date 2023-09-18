<template>
  <div class="chat-bar">
    <div class="chat-tool flex-start-center" @click="handleToolbarClick">
      <toolbar :editor="editorRef" :defaultConfig="model.toolbar" mode="simple" />
    </div>
    <div class="chat-editor">
      <editor v-model="model.html" :defaultConfig="model.config" :defaultContent="model.default" mode="simple" @onCreated="handleCreated" @keyup.enter="handleEnterHotKey" @customPaste="model.customPaste" />
    </div>
    <div class="submit">
      <el-dropdown split-button type="primary" @click="handleSubmitClick" trigger="click">
        发送消息 ({{ stores.chat.submitType == 0 ? 'Ctrl + Enter' : 'Enter' }})
        <template #dropdown>
          <el-dropdown-menu>
            <el-dropdown-item :icon="stores.chat.submitType == 0 ? 'Check' : ''" @click="() => stores.chat.setSubmitType(0)" :style="{ paddingLeft: stores.chat.submitType == 0 ? '16px' : '35px' }">
              按键 Ctrl + Enter 发送消息
            </el-dropdown-item>
            <el-dropdown-item :icon="stores.chat.submitType == 1 ? 'Check' : ''" @click="() => stores.chat.setSubmitType(1)" :style="{ paddingLeft: stores.chat.submitType == 1 ? '16px' : '35px' }">
              按键 Enter 发送消息
            </el-dropdown-item>
          </el-dropdown-menu>
        </template>
      </el-dropdown>
    </div>
    <loading-line :loading="!editorRef" :show-text="true" text="编辑器加载中" v-if="!editorRef"></loading-line>
  </div>
</template>

<script setup>
import { nextTick, onBeforeUnmount, reactive, shallowRef, watch } from 'vue';
import '@wangeditor/editor/dist/css/style.css';
import { Boot } from '@wangeditor/editor';
import { Editor, Toolbar } from '@wangeditor/editor-for-vue';
import loadingLine from '../../../../components/loadings/loading-line.vue';
import { stores } from '../../../../stores';
import toast from '../../../../utils/toast';

Boot.registerPlugin(editor => {
  const newEditor = editor;
  //
  newEditor.insertBreak = () => {
    return;
  };

  return newEditor;
});

const editorRef = shallowRef();

const model = reactive({
  html: '',
  toolbar: { toolbarKeys: ['emotion', 'color'] },
  config: {
    MENU_CONF: {
      fontSize: {
        fontSizeList: [
          { name: '16', value: '16px' },
          { name: '18', value: '18px' },
          { name: '20', value: '20px' }
        ]
      },
      fontFamily: {
        fontFamilyList: [
          { name: '黑体', value: '黑体' },
          { name: '仿宋', value: '仿宋' },
          { name: '数黑体', value: 'AlimamaShuHeiTi-Bold' },
          { name: 'Arial', value: 'Arial' },
          { name: 'Tahoma', value: 'Tahoma' },
          { name: 'Verdana', value: 'Verdana' }
        ]
      }
    },
    hoverbarKeys: {
      link: {
        // 重写 link 元素的 hoverbar
        menuKeys: []
      },
      image: {
        // 清空 image 元素的 hoverbar
        menuKeys: []
      }
    }
  },
  default: [
    {
      type: 'paragraph',
      children: [
        {
          text: '',
          fontFamily: 'AlimamaShuHeiTi-Bold',
          fontSize: '16px'
        }
      ]
    }
  ],
  customPaste: (editor, clipboard) => {
    console.log(clipboard.clipboardData.items[0].type);

    if (clipboard.clipboardData.items[0].type.indexOf('image') > -1) {
      hanldeClipboardImage(editor, clipboard);
    } else {
      const text = clipboard.clipboardData.getData('text/plain');
      // 异步
      setTimeout(() => {
        editor.insertText(text);
      }, 0);
    }

    clipboard.preventDefault();

    return false;
  }
});

const props = defineProps({
  roomId: {
    type: String,
    required: true,
    default: ''
  }
});

const editorInput = {};

watch(
  () => props.roomId,
  (value, oldvalue) => {
    if (value == oldvalue) {
      return;
    }

    if (editorRef.value) {
      if (model.html) {
        editorInput[oldvalue] = model.html;
      }

      if (editorInput[value]) {
        model.html = editorInput[value];
      } else {
        model.html = '';
      }

      nextTick(() => {
        editorRef.value.focus(true);
      });
    }
  }
);

const emit = defineEmits(['submit-message']);

onBeforeUnmount(() => {
  editorRef.value.destroy();
});

const handleCreated = editor => {
  editorRef.value = editor;
};

const handleEnterHotKey = e => {
  if (stores.chat.submitType == 0) {
    if (e.ctrlKey) {
      handleSubmitClick();
    } else {
      editorRef.value.insertText('\n');
      newLineScroll();
    }
  } else {
    if (e.ctrlKey) {
      editorRef.value.insertText('\n');
      newLineScroll();
    } else {
      handleSubmitClick();
    }
  }
};

const handleSubmitClick = e => {
  editorRef.value.focus();

  if (model.html && model.html.trim()) {
    if (editorRef.value.isEmpty()) {
      if (e) {
        editorRef.value.focus();
      }
      return;
    }

    emit('submit-message', model.html);

    // 清空数据
    editorRef.value.clear();
  }
};

const handleToolbarClick = () => {
  if (!editorRef.value.isFocused()) {
    if (!stores.chat.firstShowWangEditor) {
      toast.success('wangEditor 组件设置了编辑器没有聚焦，工具禁用，我也不知道为什么要做这么一个设定，且没找到可配置的地方，我尝试绕过它，但是没有效果，我又懒不想翻源码，请恕我无能！', {
        duration: 8000
      });
      stores.chat.setFirstShowWangEditor(true);
    }
  }
};

let editorScroll = null;
const newLineScroll = () => {
  if (!editorScroll) {
    const editor = document.querySelector('.chat-editor');
    editorScroll = editor.querySelector('div.w-e-scroll');
  }
  nextTick(() => {
    editorScroll.scrollTo({
      top: editorScroll.scrollHeight,
      behavior: 'smooth'
    });
  });
};

const imageMaxSize = 550;
const hanldeClipboardImage = (editor, clipboard) => {
  // 获取文件对象
  const file = clipboard.clipboardData.items[0].getAsFile();

  // 获取file图片的真实宽高像素
  const img = new Image();
  img.src = URL.createObjectURL(file);
  img.onload = () => {
    const { height, width } = img;
    // 上传文件
    // 根据宽高判断是否大于最大框高
    editor.dangerouslyInsertHtml(
      `<img id='img-upload' style='height: ${height > imageMaxSize ? imageMaxSize : height}px;width: ${width > imageMaxSize ? imageMaxSize : width}px' src='${img.src}' onerror="javascript:this.src="/images/404.png"/>`
    );
  };
};

defineExpose({
  focus: () => {
    editorRef.value.focus();
  },
  clear: () => {
    editorRef.value.clear();
  }
});
</script>

<style lang="scss" scoped>
.chat-bar {
  position: relative;
  height: var(--chat-bar-height);
  --editor-height: calc(var(--chat-bar-height) - 85px);

  .chat-tool {
    height: 40px;
    border-top: 2px solid var(--color-secondary);
    border-bottom: 2px solid var(--color-secondary);

    :deep(.w-e-bar) {
      background-color: transparent;
      border: 0;

      .w-e-bar-item {
        .w-e-drop-panel {
          left: 0px !important;
          right: auto !important;
        }

        ul {
          max-height: 250px;
          overflow-y: auto;

          li[title='默认字号'] {
            display: none;
          }

          li[title='默认字体'] {
            display: none;
          }
        }
      }

      .w-e-bar-item:nth-child(3) {
        width: 50px;
      }
    }
  }

  .chat-editor {
    height: var(--editor-height) !important;
    overflow: hidden;

    :deep(.w-e-text-container) {
      min-height: var(--editor-height) !important;
      background-color: transparent;

      [data-slate-editor] {
        font-size: 16px;

        p {
          margin: 2px 0;
        }

        .w-e-image-container {
          max-width: 550px;
          object-fit: cover;
          border-radius: 5px;

          @media (max-width: 1920px) {
            max-width: 500px;
          }

          @media (max-width: 1440px) {
            max-width: 400px;
          }
        }
      }
    }
  }

  .submit {
    text-align: right;
    height: 42px;
    padding: 5px 10px;
    --el-color-primary: var(--color-primary);

    :deep(.el-button-group) {
      .el-button {
        padding: 4px 10px;
      }

      .el-button:first-child {
        span {
          font-size: 12px;
        }
      }
    }
  }
}
</style>
