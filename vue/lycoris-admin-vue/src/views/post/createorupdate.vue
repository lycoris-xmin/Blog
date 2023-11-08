<template>
  <page-layout>
    <div class="container-body">
      <div class="left">
        <div class="post-title">
          <el-input v-model="form.title" placeholder="文章标题" maxlength="80" show-word-limit type="text" />
        </div>
        <markdown-container class="markdown-container" ref="markdown" :preview-only="false" :events="markdownEvents" @fileUpload="fileUpload" @keydown-save="handleKeyDownSave"> </markdown-container>
      </div>
      <div class="right">
        <div class="right-container">
          <div class="card">
            <el-form :width="'100%'" label-width="80px">
              <el-form-item>
                <div class="upload flex-center-center">
                  <el-upload accept=".jpg,.png,.gif" :on-change="iconChange" :auto-upload="false" :show-file-list="false">
                    <div v-show="form.icon" class="upload-icon-view flex-center-center">
                      <el-image :src="form.icon" />
                      <div class="icon-wrapper">
                        <el-icon :size="30" style="color: var(--color-danger)" @click.stop="iconDelete">
                          <component :is="'delete'"></component>
                        </el-icon>
                      </div>
                    </div>
                    <div v-show="!form.icon" class="upload-icon flex-center-center">
                      <el-icon :size="30">
                        <component :is="'plus'"></component>
                      </el-icon>
                    </div>
                  </el-upload>
                </div>
              </el-form-item>
              <div class="flex-start-center form-item">
                <span>文章类型</span>
                <el-select v-model="form.type" class="form-contorl">
                  <el-option key="0" label="原创" :value="0" />
                  <el-option key="1" label="转载" :value="1" />
                </el-select>
              </div>
              <div class="flex-start-center form-item form-item-top">
                <span>文章简介</span>
                <el-input
                  class="form-contorl"
                  v-model="form.info"
                  :autosize="{ minRows: 3, maxRows: 8 }"
                  type="textarea"
                  placeholder="请输入文章简介,若没有输入则默认获取html文本内的前200个字符"
                  maxlength="200"
                  show-word-limit
                ></el-input>
              </div>
              <div class="flex-start-center form-item">
                <span>文章分类</span>
                <el-select class="form-contorl" v-model="form.category" placeholder="- 未分类 -" clearable @change="categoryChange">
                  <el-option v-for="item in stores.enums.category" :key="item.value" :label="item.name" :value="item.value" />
                </el-select>
                <div class="refresh-icon">
                  <el-tooltip content="点击刷新" placement="top">
                    <el-icon @click="refreshCategoryEnum">
                      <component :is="'refresh'" />
                    </el-icon>
                  </el-tooltip>
                </div>
              </div>
              <div class="flex-start-center form-item">
                <span>文章评论</span>
                <el-select class="form-control" v-model="form.comment">
                  <el-option key="1" label="允许" :value="1" />
                  <el-option key="0" label="禁止" :value="0" />
                </el-select>
              </div>
              <div class="flex-start-center form-item form-item-top">
                <span>文章标签</span>
                <div class="tags flex-start-center">
                  <el-tag v-for="tag in form.tags" :key="tag" closable :disable-transitions="false" @close="handleTagClose(tag)">
                    {{ tag }}
                  </el-tag>
                  <el-input v-if="model.tagInputVisible" ref="InputRef" v-model="model.tagValue" class="input-w-80" @keyup.enter="handleTagInputConfirm" @blur="handleTagInputConfirm" />
                  <el-button v-else @click="showInput"> 添加标签 </el-button>
                </div>
              </div>
            </el-form>
            <div class="btn-group flex-center-center">
              <el-button type="primary" @click="submitSave(false)" plain>保存为草稿</el-button>
              <el-button type="success" @click="submitSave(true)" plain>文章发布</el-button>
              <el-button type="info" @click="toPostPage" plain>返回</el-button>
            </div>
          </div>
        </div>
      </div>
    </div>
  </page-layout>
</template>

<script setup name="post-markdown">
import { reactive, ref, onMounted, nextTick } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import pageLayout from '../layout/page-layout.vue';
import markdownContainer from '../../components/markdown-editor/index.vue';
import { getPostInfo, uploadMarkdownPicture, savePost } from '../../api/post';
import { getPostSettings } from '../../api/configuration';
import { getCategoryEnums } from '../../api/category';
import toast from '../../utils/toast';
import swal from '../../utils/swal';
import { stores } from '../../stores';
import { debounce } from '../../utils/tool';

const route = useRoute();
const router = useRouter();

const markdown = ref();
const InputRef = ref();

var postInfo = {};

const model = reactive({
  autoSave: false,
  autoSaveSecond: 0,
  tagValue: '',
  tagInputVisible: false,
  isModify: false,
  lastSaveTime: 0
});

const form = reactive({
  id: '0',
  title: '',
  info: '',
  markdown: '',
  icon: '',
  type: 0,
  category: '',
  comment: 1,
  tags: [],
  isPublish: false,
  file: void 0
});

const fileUpload = async (file, callback) => {
  if (file === undefined || file === null) {
    return;
  }

  if (file.size / 1024 / 1024 > 5) {
    toast.warn('文件大小不能超过5MB');
    return;
  }

  let res = await uploadMarkdownPicture(file);
  if (res.resCode == 0) {
    callback(res.data);
  }
};

const markdownEvents = ref({
  afterChange: debounce(markdown => {
    if (!model.isModify && form.markdown != markdown) {
      model.isModify = true;
    } else if (model.isModify && form.markdown == markdown) {
      model.isModify = false;
    }

    if (model.autoSave) {
      model.lastSaveTime = new Date().addSeconds(model.autoSaveSecond).getTime();
      console.log(model.lastSaveTime);
    }
  }, 500)
});

const toPostPage = () => {
  router.push({
    name: 'post'
  });
};

const categoryEnums = async () => {
  try {
    if (!stores.enums.category.length) {
      let res = await getCategoryEnums();
      if (res && res.resCode == 0) {
        stores.enums.setCategory(res.data.list);
      }
    }
  } catch (error) {}
};

onMounted(async () => {
  try {
    await categoryEnums();
    if (route.query?.id) {
      try {
        let res = await getPostInfo(route.query.id);

        form.id = res.data.id;
        form.title = res.data.title;
        form.type = res.data.type;
        form.category = res.data.category || '';
        form.comment = res.data.comment;
        form.tags = res.data.tags || [];
        form.icon = res.data.icon || '';
        form.markdown = '';
        form.isPublish = res.data.isPublish || false;

        markdown.value.init(res.data.markdown);

        postInfo = { ...form };
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
      postInfo = { ...form };
      markdown.value.init();
    }

    await getSettings();
  } catch (err) {
    console.log(err);
  }

  if (model.autoSave) {
    //
    setInterval(async () => {
      if (model.lastSaveTime < new Date().getTime()) {
        await autoSavePost();
      }
    }, model.autoSaveSecond * 1000);
  }
});

const getSettings = async () => {
  let res = await getPostSettings();
  if (res && res.resCode == 0) {
    //
    model.autoSave = res.data.autoSave;
    model.autoSaveSecond = res.data.second;
    toast.info(`配置已开启自动存档，每${model.autoSaveSecond}秒将会提交自动存档`);
  }
};

const categoryChange = val => {
  try {
    if (form.icon == '') {
      form.icon = stores.enums.category.filter(x => x.value == val)[0].icon;
    } else {
      let _categoryIcon = stores.enums.category.filter(x => x.icon == form.icon);
      if (_categoryIcon.length) {
        form.icon = stores.enums.category.filter(x => x.value == val)[0].icon;
      }
    }
  } catch (error) {
    console.log(error);
    form.icon = '';
  }
};

const iconChange = file => {
  form.icon = URL.createObjectURL(file.raw);
  form.file = file.raw;
};

const iconDelete = () => {
  form.icon = '';
  form.file = void 0;
};

const refreshCategoryEnum = async () => {
  let res = await getCategoryEnums();
  if (res && res.resCode == 0) {
    stores.enums.setCategory(res.data.list);
  }
};

const showInput = () => {
  model.tagInputVisible = true;
  nextTick(() => {
    InputRef.value.input.focus();
  });
};

const handleTagInputConfirm = () => {
  try {
    if (model.tagValue) {
      if (form.tags.length >= 8) {
        console.log(form.tags);
        toast.warn('文章标签最多只能添加8个');
        return;
      }

      if (form.tags.indexOf(model.tagValue) == -1) {
        form.tags.push(model.tagValue);
      } else {
        toast.info(`标签：${model.tagValue} 已存在`);
      }
    }
  } finally {
    model.tagValue = '';
    model.tagInputVisible = false;
  }
};

const handleTagClose = tag => {
  form.tags.splice(form.tags.indexOf(tag), 1);
};

const checkPostChange = data => {
  if (Object.keys(postInfo).length == 0) {
    return data;
  }

  for (let old in postInfo) {
    for (let item in data) {
      if (postInfo[old] != data[item]) {
        return data;
      }
    }
  }

  return void 0;
};

const submitSave = async (publish, redirect = true) => {
  form.isPublish = publish;

  let data = {
    ...form
  };

  //
  if (data.title == '') {
    toast.warn('文章标题不能为空');
    return;
  }

  if (!data.info) {
    data.info = markdown.value.getInfoText(200);
  }

  if (!data.info) {
    toast.warn('文章简介不能为空');
    return;
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
    }

    let res = await savePost(data);
    if (res && res.resCode == 0) {
      toast.success(publish ? '发布成功' : '保存成功');
      if (redirect) {
        setTimeout(() => {
          toPostPage();
        }, 500);
      } else {
        postInfo = data;
      }

      if (model.autoSave) {
        model.lastSaveTime = new Date().addSeconds(model.autoSaveSecond * 1000).getTime();
      }
    }
  }
};

const autoSavePost = async () => {
  try {
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
      form.info = '未描述';
      data.info = form.info;
    }

    //
    data.category = data.category || 0;

    if (data) {
      data = checkPostChange(data);
      if (!data) {
        toast.success('自动存档成功');
      }

      let res = await savePost(data);
      if (res && res.resCode == 0) {
        toast.success('自动存档成功');
        postInfo = data;
      }
    }
  } catch (err) {
    toast.error('自动存档失败');
    console.log(err);
  } finally {
    if (model.autoSave) {
      model.lastSaveTime = new Date().addSeconds(model.autoSaveSecond * 1000).getTime();
    }
  }
};

const handleKeyDownSave = async () => {
  await submitSave(form.isPublish, false);
};
</script>

<style lang="scss" scoped>
.container-body {
  height: calc(100vh - 210px);
  width: 100%;
  overflow-x: hidden;
  display: flex;
  justify-content: flex-start;
  align-items: flex-start;

  $top-height: 260px;

  .left {
    flex: 8;

    .post-title {
      padding: 0 0 15px 0;
    }

    .markdown-container {
      height: calc(100vh - $top-height) !important;
    }
  }

  .right {
    flex: 2;
    .right-container {
      margin: 0px 20px;
      max-height: calc(100vh - $top-height + 50px) !important;
      border: 1px solid var(--color-secondary);
      border-radius: 8px;
      overflow-y: auto;

      .card {
        padding: 10px 20px;
      }

      .form-item {
        margin-bottom: 10px;

        span {
          flex-shrink: 0;
          padding-right: 10px;
        }

        .form-contorl {
          flex-grow: 1;
          width: auto !important;
        }

        .el-select {
          width: 100%;
        }
      }

      .form-item-top {
        align-items: flex-start !important;
      }

      .refresh-icon {
        width: 30px;
        background-color: transparent;
        color: var(--color-primary);
        border: 0;
        padding: 0 10px;
        cursor: pointer;
        transform: scale(1);
        transition: all 0.3s;
      }

      .refresh-icon:hover {
        transform: scale(1.2);
      }

      .tags {
        flex-flow: row wrap;
        gap: 5px;
        margin-bottom: 18px;
        min-height: 34px;

        .el-tag {
          margin: 5px;
          cursor: pointer;
          font-size: 15px;
          letter-spacing: 1px;
        }

        .input-w-80 {
          width: 80px;
        }
      }

      .upload {
        margin-top: 15px;
        width: 100%;

        .upload-icon-view {
          position: relative;
          padding: 2px;
          max-height: 144px;
          width: 100%;

          :deep(.el-image) {
            height: 140px;
            width: 200px;
            border-radius: 8px;
            border: 1px solid var(--color-secondary);

            .el-image__wrapper {
              height: 140px;
              width: 200px;
            }
          }

          .icon-wrapper {
            position: absolute;
            left: 1;
            top: 1;
            height: 140px;
            width: 200px;
            border-radius: 8px;
            background-color: rgba(0, 0, 0, 0.637);
            display: none;
            justify-content: center;
            align-items: center;
            transition: all 0.5s;
            cursor: default;

            .el-icon {
              cursor: pointer;
            }
          }
        }

        .upload-icon-view:hover {
          .icon-wrapper {
            display: flex;
          }
        }

        .upload-icon {
          height: 140px;
          width: 200px;
          border: 2px dashed var(--color-secondary);
          border-radius: 3px;
          transition: all 0.5s;

          :deep(.el-icon) {
            transition: all 0.5s;
          }
        }

        .upload-icon:hover {
          border-color: var(--color-info);

          :deep(.el-icon) {
            color: var(--color-info);
          }
        }
      }

      .btn-group {
        flex-direction: column;

        a {
          text-decoration: none;
        }

        .el-button {
          margin: 0 0 18px 0;
          width: 140px;
        }
      }
    }
  }

  @media (max-width: 1920px) {
    .right {
      flex: 3;
    }
  }
}
</style>
