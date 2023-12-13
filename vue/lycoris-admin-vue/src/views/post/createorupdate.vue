<template>
  <page-layout>
    <template #tool>
      <el-button type="info" plain @click="toPostPage">
        <el-icon style="margin-right: 8px">
          <component :is="'arrow-left'"></component>
        </el-icon>
        返回
      </el-button>
      <el-button :disabled="model.loading" type="primary" plain v-if="model.id" @click="handleSetting">
        <el-icon style="margin-right: 8px">
          <component :is="'setting'"></component>
        </el-icon>
        设置
      </el-button>
      <el-button :disabled="model.loading" type="warning" plain @click="submitSave(false, false)">
        <el-icon style="margin-right: 8px">
          <component :is="'files'"></component>
        </el-icon>
        保存
      </el-button>

      <el-button :disabled="model.loading" type="success" plain @click="handleSubmit">
        <el-icon style="margin-right: 8px">
          <component :is="'promotion'"></component>
        </el-icon>
        发布
      </el-button>
    </template>

    <div class="container-body">
      <div class="post-title">
        <el-input v-model="model.title" placeholder="文章标题" maxlength="80" show-word-limit type="text" />
      </div>

      <markdown-container class="markdown-container" ref="markdown" :preview-only="false" :events="markdownEvents" @fileUpload="fileUpload" @keydown-save="handleKeyDownSave"> </markdown-container>
    </div>

    <post-setting-modal ref="postSettingModalRef" :id="model.id" @sumit="handleDialogSumit"></post-setting-modal>
  </page-layout>
</template>

<script setup name="post-editor">
import { reactive, ref, onMounted, onBeforeUnmount } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import pageLayout from '../layout/page-layout.vue';
import markdownContainer from '../../components/markdown-editor/index.vue';
import postSettingModal from './components/post-setting-modal.vue';
import { getPostInfo, savePost } from '../../api/post';
import { uploadStaticFile } from '../../api/staticFile';
import { getPostSetting } from '../../api/configuration';
import UploadType from '../../constants/UploadType';
import toast from '../../utils/toast';
import swal from '../../utils/swal';
import { debounce } from '../../utils/tool';

const route = useRoute();
const router = useRouter();

const markdown = ref();
const postSettingModalRef = ref();

const model = reactive({
  loading: true,
  id: '',
  title: '',
  buttonLoading: ''
});

const setting = {
  autoSave: false,
  autoSaveSecond: 0,
  lastSaveTime: 0,
  isModify: false,
  postInfo: {}
};

const form = {
  id: '',
  title: '',
  info: '',
  markdown: '',
  icon: '',
  type: 0,
  category: '',
  comment: 1,
  recommend: 0,
  tags: [],
  updateTime: '',
  isPublish: false
};

const fileUpload = async (file, callback) => {
  if (file === undefined || file === null) {
    return;
  }

  if (file.size / 1024 / 1024 > 5) {
    toast.warn('文件大小不能超过5MB');
    return;
  }

  try {
    let res = await uploadStaticFile(UploadType.POST.FILE, file);
    if (res.resCode == 0) {
      callback(res.data);
    }
  } catch (error) {
    console.log(error);
  }
};

const markdownEvents = ref({
  afterChange: debounce(markdown => {
    if (!setting.isModify && form.markdown != markdown) {
      setting.isModify = true;
    } else if (setting.isModify && form.markdown == markdown) {
      setting.isModify = false;
    }

    if (setting.autoSave) {
      setting.lastSaveTime = new Date().addSeconds(setting.autoSaveSecond).getTime();
      console.log(setting.lastSaveTime);
    }
  }, 500)
});

const toPostPage = () => {
  router.push({
    name: 'post'
  });
};

let autoSaveInterval = void 0;

onMounted(async () => {
  try {
    form.updateTime = new Date().format('yyyy-MM-dd HH:mm:ss');

    if (route.query?.id) {
      model.id = route.query.id;
      try {
        let res = await getPostInfo(model.id);

        form.id = model.id;
        model.title = res.data.title;
        form.title = res.data.title;
        form.markdown = res.data.markdown;
        form.icon = res.data.icon || '';
        form.category = res.data.category || '';
        form.tags = res.data.tags || [];
        form.type = res.data.type;
        form.comment = res.data.comment;
        form.recommend = res.data.recommend;
        form.info = res.data.info;

        if (res.data.updateTime) {
          form.updateTime = res.data.updateTime;
        }

        form.isPublish = res.data.isPublish || false;

        markdown.value.init(res.data.markdown);

        setting.postInfo = { ...form };
      } catch {
        swal
          .error('获取博客文章数据失败')
          .then(() => {
            toPostPage();
          })
          .catch(() => {
            toPostPage();
          });
      }
    } else {
      setting.postInfo = { ...form };
      markdown.value.init();
    }

    await getSettings();
  } catch (err) {
    console.log(err);
  }

  if (setting.autoSave) {
    //
    autoSaveInterval = setInterval(async () => {
      if (setting.lastSaveTime < new Date().getTime()) {
        await autoSavePost();
      }
    }, setting.autoSaveSecond * 1000);
  }

  model.loading = false;
});

onBeforeUnmount(() => {
  if (autoSaveInterval) {
    clearInterval(autoSaveInterval);
  }
});

const getSettings = async () => {
  let res = await getPostSetting();
  if (res && res.resCode == 0) {
    //
    setting.autoSave = res.data.autoSave;
    setting.autoSaveSecond = res.data.second;
    toast.info(`已开启自动存档，每过${setting.autoSaveSecond}秒将会提交自动存档`);
  }
};

const checkPostChange = data => {
  if (Object.keys(setting.postInfo).length == 0) {
    return data;
  }

  for (let key in setting.postInfo) {
    if (setting.postInfo[key] != data[key]) {
      return data;
    }
  }

  return void 0;
};

const submitSave = async (publish, redirect = true) => {
  form.title = model.title;
  form.isPublish = publish;

  let data = { ...form };

  if (data.title == '') {
    toast.warn('文章标题不能为空');
    return;
  }

  if (!data.info) {
    data.info = markdown.value.getInfoText(200);
  }

  if (form.isPublish) {
    //
    if (!data.info) {
      toast.warn('文章简介不能为空');
      return;
    }
  } else {
    if (!data.info) {
      delete data.info;
    }
  }

  //
  data.category = data.category || 0;
  data.markdown = markdown.value.getMarkdown();

  if (data) {
    data = checkPostChange(data);
    if (!data) {
      toast.success(publish ? '发布成功' : '保存成功');
      if (redirect) {
        setTimeout(() => {
          toPostPage();
        }, 500);
      }
      return;
    }

    model.buttonLoading = publish ? 'publish' : 'draft';
    markdown.value.showLoading('正在提交,请稍候...');

    try {
      let res = await savePost(data);
      if (res && res.resCode == 0) {
        toast.success(publish ? '发布成功' : '保存成功');

        if (redirect) {
          setTimeout(() => {
            toPostPage();
          }, 500);
        } else {
          if (form.id) {
            form.id = res.data.id;
          }

          setting.postInfo = { ...form };
        }

        if (setting.autoSave) {
          setting.lastSaveTime = new Date().addSeconds(setting.autoSaveSecond * 1000).getTime();
        }
      }
    } finally {
      markdown.value.hideLoading();
      model.buttonLoading = '';
    }
  }
};

const autoSavePost = async () => {
  try {
    form.title = model.title;

    //
    let data = {
      ...form
    };

    data.markdown = markdown.value.getMarkdown();
    if (!data.markdown) {
      return;
    }

    //
    if (data.title == '') {
      form.title = '未命名';
      data.title = form.title;
    }

    if (!data.info) {
      data.info = markdown.value.getInfoText(200);
    }

    if (!data.info) {
      delete data.info;
    }

    //
    data.category = data.category || 0;

    if (data) {
      data = checkPostChange(data);
      if (!data) {
        toast.success('自动存档成功');
        return;
      }

      let res = await savePost(data);
      if (res && res.resCode == 0) {
        toast.success('自动存档成功');
        if (form.id) {
          form.id = res.data.id;
        }
        setting.postInfo = { ...form };
      }
    }
  } catch (err) {
    toast.error('自动存档失败');
    console.log(err);
  } finally {
    if (setting.autoSave) {
      setting.lastSaveTime = new Date().addSeconds(setting.autoSaveSecond * 1000).getTime();
    }
  }
};

const handleKeyDownSave = async () => {
  await submitSave(form.isPublish, false);
};

const handleDialogSumit = data => {
  Object.assign(form, { ...data });
  if (!form.id) {
    submitSave(true);
  }
};

const handleSetting = () => {
  setting.lastSaveTime = new Date().addSeconds(setting.autoSaveSecond * 1000).getTime();
  //
  postSettingModalRef.value.show({ ...form });
};

const handleSubmit = () => {
  setting.lastSaveTime = new Date().addSeconds(setting.autoSaveSecond * 1000).getTime();
  // 创建的话 需要弹出文章配置页面
  if (!form.id) {
    postSettingModalRef.value.show({ ...form });
    return;
  }

  submitSave(true);
};
</script>

<style lang="scss" scoped>
.container-body {
  height: calc(100vh - 210px);
  width: 100%;
  overflow-x: hidden;

  $top-height: 260px;

  .post-title {
    padding: 0 0 15px 0;
  }

  .markdown-container {
    height: calc(100vh - $top-height) !important;
  }
}
</style>
