<template>
  <div class="">
    <markdown-container ref="markdown" class="markdown-container" :preview-only="false" :events="markdownEvents" :custom-menu="model.tool" @fileUpload="fileUpload" @keydown-save="handleKeyDownSave"></markdown-container>
  </div>
</template>

<script setup name="about-web">
import { reactive, ref } from 'vue';
import markdownContainer from '../../components/markdown-editor/index.vue';

const markdown = ref();
const model = reactive({
  loading: true,
  markdown: '',
  isModify: false,
  tool: [
    {
      name: 'save',
      label: '保存',
      callback: async () => {
        let value = markdown.value.getMarkdown();
        emit('save', value, model.isModify);
      }
    }
  ]
});

const emit = defineEmits(['save', 'fileUpload']);

const fileUpload = (file, callback) => {
  emit('fileUpload', file, callback);
};

const markdownEvents = ref({
  afterChange: markdown => {
    if (!model.isModify && model.markdown != markdown) {
      model.isModify = true;
    } else if (model.isModify && model.markdown == markdown) {
      model.isModify = false;
    }
  }
});

const init = value => {
  model.markdown = value;
  markdown.value.init(value);
};

const handleKeyDownSave = () => {
  let value = markdown.value.getMarkdown();
  emit('save', value, model.isModify);
};

defineExpose({
  init,
  showLoading: () => {
    markdown.value.showLoading();
  },
  hideLoading: () => {
    markdown.value.hideLoading();
  }
});
</script>

<style lang="scss" scoped>
.markdown-container {
  height: calc(100vh - 210px) !important;
}
</style>
