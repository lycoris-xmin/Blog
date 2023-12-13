<template>
  <div>
    <el-dialog v-model="model.visible" title="文章设置" width="550px">
      <div class="setting-from">
        <div class="form-group">
          <el-image class="post-icon" :src="form.icon">
            <template #error>
              <div class="image-slot flex-center-center">
                <el-icon>
                  <component :is="'picture'"></component>
                </el-icon>
              </div>
            </template>
          </el-image>
        </div>

        <div class="form-group">
          <span>封面图</span>
          <div>
            <el-input v-model="form.icon" placeholder="不填则使用随机图">
              <template #append>
                <el-button type="info" @click="showImageSelect">
                  <el-icon class="folder">
                    <component :is="'folder-opened'"></component>
                  </el-icon>
                </el-button>
              </template>
            </el-input>
          </div>
        </div>

        <div class="form-group">
          <span>文章类型</span>
          <el-radio-group v-model="form.type">
            <el-radio :label="0" size="large">原创</el-radio>
            <el-radio :label="1" size="large">转载</el-radio>
          </el-radio-group>
        </div>

        <div class="form-group">
          <span>文章摘要</span>
          <el-radio-group v-model="model.infoType">
            <el-radio :label="0" size="large">自动生成</el-radio>
            <el-radio :label="1" size="large">手动输入</el-radio>
          </el-radio-group>
        </div>

        <div class="form-group form-info" :class="{ 'show-info': model.infoType == 1, 'hide-info': model.infoType == 0 }">
          <el-input v-model="form.info" type="textarea" :autosize="{ minRows: 8, maxRows: 8 }" maxlength="200" show-word-limit :resize="'none'"></el-input>
        </div>

        <div class="form-group">
          <span>文章分类</span>
          <div class="form-contorl flex-start-center">
            <el-select v-model="form.category" placeholder="- 未分类 -" clearable @change="categoryChange">
              <el-option v-for="item in stores.enums.category" :key="item.value" :label="item.name" :value="item.value" />
            </el-select>
            <el-tooltip content="刷新分类列表" placement="top">
              <div class="refresh-icon flex-center-center">
                <el-icon @click="refreshCategoryEnum">
                  <component :is="'refresh'" />
                </el-icon>
              </div>
            </el-tooltip>
            <el-tooltip content="使用分类图片作为封面图" placement="top">
              <div class="refresh-icon flex-center-center">
                <el-icon @click="selectCategoryIcon">
                  <component :is="'check'" />
                </el-icon>
              </div>
            </el-tooltip>
          </div>
        </div>

        <div class="form-group">
          <span>文章标签</span>
          <el-select class="form-contorl" v-model="model.tagValue" multiple filterable allow-create default-first-option :reserve-keyword="false" placeholder="">
            <el-option v-for="item in form.tags" :key="item" :label="item" :value="item" />
          </el-select>
        </div>

        <div class="form-group">
          <span>文章评论</span>
          <el-radio-group v-model="form.comment">
            <el-radio :label="1" size="large">允许评论</el-radio>
            <el-radio :label="0" size="large">禁止评论</el-radio>
          </el-radio-group>
        </div>

        <div class="form-group">
          <span>文章推荐</span>
          <el-radio-group v-model="form.recommend">
            <el-radio :label="1" size="large">推荐</el-radio>
            <el-radio :label="0" size="large">不推荐</el-radio>
          </el-radio-group>
        </div>

        <div class="form-group">
          <span>发布时间</span>
          <el-date-picker v-model="form.updateTime" type="datetime" format="YYYY-MM-DD hh:mm:ss" value-format="YYYY-MM-DD hh:mm:ss" />
        </div>
      </div>
      <template #footer>
        <span class="dialog-footer">
          <el-button @click="model.visible = false">取消</el-button>
          <el-button type="primary" @click="handleSumit"> {{ !props.id ? '发布' : '确定' }} </el-button>
        </span>
      </template>
    </el-dialog>

    <static-file-modal ref="imageModalRef" :showFilter="false" :defaultFileType="0" :upload-file-type="UploadType.POST.ICON" @selected="handleImageSelect"></static-file-modal>
  </div>
</template>

<script setup>
import { onMounted, reactive, ref } from 'vue';
import staticFileModal from '@/components/static-file-modal/index.vue';
import UploadType from '../../../constants/UploadType';
import { getCategoryEnums } from '@/api/category';
import { stores } from '@/stores';

const imageModalRef = ref();

const props = defineProps({
  id: {
    type: String,
    default: ''
  }
});

const model = reactive({
  visible: false,
  infoType: 0,
  tagValue: []
});

const form = reactive({
  icon: '',
  type: 0,
  category: '',
  info: '',
  comment: 1,
  recommend: 0,
  tags: [],
  updateTime: ''
});

const emit = defineEmits(['sumit']);

onMounted(() => {
  categoryEnums();
});

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

const show = ({ icon, type, category, info, comment, recommend, tags, updateTime }) => {
  if (icon) {
    form.icon = icon;
  }

  if (type) {
    form.type = type;
  }

  if (category) {
    form.category = category;
  }

  if (info) {
    form.info = info;
    model.infoType = 1;
  }

  if (comment != undefined && typeof comment == 'number') {
    form.comment = comment;
  }

  if (recommend != undefined && typeof recommend == 'boolean') {
    form.recommend = recommend;
  }

  if (tags) {
    form.tags = tags;
  }

  if (updateTime) {
    form.updateTime = updateTime;
  } else {
    form.updateTime = new Date().format('yyyy-MM-dd HH:mm:ss');
  }

  model.visible = true;
};

const close = () => {
  model.visible = false;

  setTimeout(() => {
    model.tagValue = [];
    form.icon = '';
    form.type = 0;
    form.category = '';
    form.info = '';
    form.comment = 1;
    form.recommend = false;
    form.tags = [];
    form.updateTime = '';
  }, 200);
};

const showImageSelect = () => {
  imageModalRef.value.show();
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

const refreshCategoryEnum = async () => {
  let res = await getCategoryEnums();
  if (res && res.resCode == 0) {
    stores.enums.setCategory(res.data.list);
  }
};

const selectCategoryIcon = () => {
  if (form.category) {
    form.icon = stores.enums.category.filter(x => x.value == form.category)[0].icon;
  }
};

const handleImageSelect = url => {
  form.icon = url;
};

const handleSumit = () => {
  if (model.infoType == 0) {
    form.info = '';
  }

  if (model.tagValue && model.tagValue.length) {
    form.tags = [...model.tagValue];
  }

  if (!form.updateTime) {
    form.updateTime = new Date().format('yyyy-MM-dd HH:mm:ss');
  }

  emit('sumit', { ...form });
  close();
};

defineExpose({
  show,
  close
});
</script>

<style lang="scss" scoped>
.setting-from {
  .collapse-title {
    color: var(--color-dark);
    font-size: 16px;
  }

  .form-group {
    display: flex;
    justify-content: flex-start;
    align-items: center;
    margin-bottom: 10px;

    $span-width: 80px;
    > span {
      &:first-child {
        flex-shrink: 1;
        width: $span-width;
        padding-right: 10px;

        &::after {
          content: ':';
        }
      }
    }

    > div:last-child {
      width: calc(100% - $span-width);

      .el-select {
        width: 100%;
      }
    }

    .post-icon {
      margin-bottom: 15px;
      height: 140px;
      width: 240px !important;
      border-radius: 5px;
      border: 1px solid var(--color-secondary);

      .image-slot {
        height: 100%;

        .el-icon {
          font-size: 45px;
        }
      }
    }

    .refresh-icon {
      margin-left: 10px;
      cursor: pointer;
      border-radius: 3px;
      padding: 5px 8px;
      border: 1px solid var(--color-secondary);
    }

    .form-contorl {
      width: calc(100% - 100px);
    }

    &:has(.post-icon) {
      justify-content: center;
    }

    &:first-child {
      margin-top: 10px;
    }

    &.form-info {
      overflow: hidden;
      transition: all 0.4s;

      .el-textarea {
        width: 100%;
      }
    }

    &.show-info {
      max-height: 180px;
    }

    &.hide-info {
      max-height: 0;
    }
  }

  .folder {
    cursor: pointer;
  }
}
</style>
