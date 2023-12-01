<template>
  <page-layout :loading="loading">
    <div class="">
      <single-markdown-page ref="markdown" @save="save" @fileUpload="fileUpload"></single-markdown-page>
    </div>
  </page-layout>
</template>

<script setup name="about-web">
import { ref, onMounted } from 'vue';
import pageLayout from '../layout/page-layout.vue';
import singleMarkdownPage from '../../components/single-markdown-page/index.vue';
import { getAboutWeb, saveAboutWeb, uploadFile } from '../../api/website';
import toast from '../../utils/toast';

const markdown = ref();
const loading = ref(true);

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

  let res = await uploadFile('about/web', file);
  if (res && res.resCode == 0) {
    callback(res.data);
  }
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
</script>

<style lang="scss" scoped>
.markdown-container {
  height: calc(100vh - 210px) !important;
}
</style>
