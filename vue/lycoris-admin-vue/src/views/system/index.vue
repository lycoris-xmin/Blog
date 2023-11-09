<template>
  <page-layout :loading="laoding">
    <el-tabs tab-position="top">
      <el-tab-pane v-for="item in model.tabs" :label="item.title" :key="item.value">
        <web-settings v-if="item.value == 'webSettings'" :value="0" @tab-complete="complete"></web-settings>
        <post-settings v-if="item.value == 'postSettings'" :value="1" @tab-complete="complete"></post-settings>
        <email-settings v-if="item.value == 'emailSettings'" :value="2" @tab-complete="complete"></email-settings>
        <staticfile-settings v-if="item.value == 'fileUploadSettings'" :value="3" @tab-complete="complete"></staticfile-settings>
        <seo-settings v-if="item.value == 'seoSettings'" :value="4" @tab-complete="complete"></seo-settings>
        <system-settings v-if="item.value == 'systemSettings'" :value="5" @tab-complete="complete"></system-settings>
        <systemInfo v-if="item.value == 'systemInfo'" :value="6" @tab-complete="complete"></systemInfo>
      </el-tab-pane>
    </el-tabs>
  </page-layout>
</template>

<script setup name="settings">
import { reactive, computed } from 'vue';
import PageLayout from '../layout/page-layout.vue';
import webSettings from './components/web-settings.vue';
import postSettings from './components/post-settings.vue';
import emailSettings from './components/email-settings.vue';
import staticfileSettings from './components/staticfile-settings.vue';
import seoSettings from './components/seo-settings.vue';
import systemSettings from './components/system-settings.vue';
import systemInfo from './components/system-info.vue';

const model = reactive({
  active: 'webSettings',
  tabs: [
    {
      title: '网站设置',
      value: 'webSettings',
      loading: true
    },
    {
      title: '博客设置',
      value: 'postSettings',
      loading: true
    },
    {
      title: '邮件服务',
      value: 'emailSettings',
      loading: true
    },
    {
      title: '静态文件设置',
      value: 'fileUploadSettings',
      loading: true
    },
    {
      title: 'SEO设置',
      value: 'seoSettings',
      loading: true
    },
    {
      title: '系统设置',
      value: 'systemSettings',
      loading: true
    },
    {
      title: '系统信息',
      value: 'systemInfo',
      loading: true
    }
  ],
  loading: []
});

const laoding = computed(() => {
  return model.loading.filter(x => x == true).length > 0;
});

for (let i = 0; i < model.tabs.length; i++) {
  model.loading.push(true);
}

const complete = index => {
  if (model.loading.length > index) {
    model.loading[index] = false;
  }
};
</script>

<style lang="scss" scoped>
:deep(.el-tabs__nav) {
  $width: 140px;

  .el-tabs__active-bar {
    width: $width !important;
  }

  .el-tabs__item {
    width: $width;
    padding: 0;
    align-items: center;
    font-size: 18px;
  }
}

.el-tab-pane {
  padding: 0 10px;
}
</style>
