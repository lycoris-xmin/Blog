<template>
  <page-layout :loading="laoding">
    <el-tabs tab-position="top" class="system-setting">
      <el-tab-pane v-for="(item, index) in model.tabs" :label="item.title" :key="item.value">
        <web-setting v-if="item.value == 'webSetting'" :value="index" @tab-complete="complete"></web-setting>
        <post-setting v-else-if="item.value == 'PostSetting'" :value="index" @tab-complete="complete"></post-setting>
        <message-setting v-else-if="item.value == 'messageSetting'" :value="index" @tab-complete="complete"></message-setting>
        <email-setting v-else-if="item.value == 'emailSetting'" :value="index" @tab-complete="complete"></email-setting>
        <upload-setting v-else-if="item.value == 'fileUploadSetting'" :value="index" @tab-complete="complete"></upload-setting>
        <seo-setting v-else-if="item.value == 'seoSetting'" :value="index" @tab-complete="complete"></seo-setting>
        <system-setting v-else :value="index" @tab-complete="complete"></system-setting>
      </el-tab-pane>
    </el-tabs>
  </page-layout>
</template>

<script setup name="settings">
import { reactive, computed } from 'vue';
import PageLayout from '../layout/page-layout.vue';
import webSetting from './components/web-setting.vue';
import postSetting from './components/post-setting.vue';
import messageSetting from './components/message-setting.vue';
import emailSetting from './components/email-setting.vue';
import uploadSetting from './components/upload-setting.vue';
import seoSetting from './components/seo-setting.vue';
import systemSetting from './components/system-setting.vue';

const model = reactive({
  active: 'webSettings',
  tabs: [
    {
      title: '网站设置',
      value: 'webSetting',
      loading: true
    },
    {
      title: '文章设置',
      value: 'PostSetting',
      loading: true
    },
    {
      title: '留言设置',
      value: 'messageSetting',
      loading: true
    },
    {
      title: '邮件设置',
      value: 'emailSetting',
      loading: true
    },
    {
      title: '上传设置',
      value: 'fileUploadSetting',
      loading: true
    },
    {
      title: 'SEO设置',
      value: 'seoSetting',
      loading: true
    },
    {
      title: '系统设置',
      value: 'systemSetting',
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

<style lang="scss">
.tab-panel-container {
  padding-top: 18px;

  .harf-body {
    width: 35%;

    .form-panel {
      margin-bottom: 35px;
      grid-gap: 20px;

      .header {
        margin-bottom: 15px;
        font-size: 18px;
        padding-bottom: 8px;
        border-bottom: 1px solid var(--color-secondary);

        .title-info-text {
          color: var(--color-danger);
          padding-left: 15px;

          &::before {
            content: '*';
            color: var(--color-danger);
          }
        }
      }

      &.in-line {
        display: grid;
        grid-template-columns: repeat(2, 1fr);
        justify-content: start;
        align-items: center;

        .header:first-child {
          grid-column-start: 1;
          grid-column-end: 3;
        }
      }

      .el-select {
        width: 100%;
      }
    }

    .submit {
      margin-top: 20px;
      text-align: left;

      .el-button {
        width: 120px;
      }
    }
  }
}
</style>
