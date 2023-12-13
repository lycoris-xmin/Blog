<template>
  <page-layout :loading="loading">
    <markdown-editor class="markdown-container" ref="markdown" :preview-only="false" :custom-menu="model.tool" :events="model.markdownEvents" @fileUpload="fileUpload" @keydown-save="handleKeyDownSave"></markdown-editor>
  </page-layout>
</template>

<script setup name="about-web">
import { ref, onMounted, reactive } from 'vue';
import pageLayout from '../layout/page-layout.vue';
import markdownEditor from '../../components/markdown-editor/index.vue';
import { getAboutWeb, saveAboutWeb } from '../../api/website';
import { uploadStaticFile } from '../../api/staticFile';
import UploadType from '../../constants/UploadType';
import toast from '../../utils/toast';

const markdown = ref();
const loading = ref(true);

const model = reactive({
  tool: [
    {
      name: 'save',
      label: '保存',
      callback: async () => {
        if (model.isModify) {
          let value = markdown.value.getMarkdown();
          save(value);
        }
      }
    }
  ],
  markdownEvents: {
    afterChange: markdown => {
      if (!model.isModify && model.markdown != markdown) {
        model.isModify = true;
      } else if (model.isModify && model.markdown == markdown) {
        model.isModify = false;
      }
    }
  }
});

onMounted(async () => {
  try {
    let res = await getAboutWeb();
    if (res && res.resCode == 0) {
      markdown.value.init(res.data);
    }
  } finally {
    loading.value = false;
  }
});

const fileUpload = async (file, callback) => {
  if (file === undefined || file === null) {
    return;
  }

  if (file.size / 1024 / 1024 > 5) {
    toast.warn('文件大小不能超过5MB');
    return;
  }

  try {
    let res = await uploadStaticFile(UploadType.ABOUT_WEB, file);
    if (res && res.resCode == 0) {
      callback(res.data);
    }
  } catch (error) {}
};

const save = async value => {
  markdown.value.showLoading('数据保存中,请稍候...');
  try {
    let res = await saveAboutWeb(value);
    if (res && res.resCode == 0) {
      toast.success('保存成功');
    }
  } finally {
    markdown.value.hideLoading();
  }
};

const handleKeyDownSave = () => {
  if (model.isModify) {
    let value = markdown.value.getMarkdown();
    save(value);
  }
};
</script>

<style lang="scss" scoped>
.markdown-container {
  height: calc(100vh - 210px) !important;
}
</style>
