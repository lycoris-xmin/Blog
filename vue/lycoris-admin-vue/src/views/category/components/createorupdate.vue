<template>
  <el-dialog v-model="model.visible" title="博客分类" width="450px" :close-on-click-modal="false" :before-close="beforeClose">
    <el-form label-width="100px" label-position="left" class="form-ref" ref="formRef" :model="form">
      <div class="upload-img flex-center-center">
        <el-upload accept=".jpg,.png,.gif" :on-change="iconChange" :auto-upload="false" :show-file-list="false">
          <div v-show="form.icon" class="upload-img-view flex-center-center">
            <img :src="form.icon" />
          </div>
          <div v-show="!form.icon" class="upload-icon flex-center-center">
            <el-icon :size="30">
              <component :is="'plus'"></component>
            </el-icon>
          </div>
        </el-upload>
        <span style="padding-top: 10px; font-size: 16px; font-weight: 500">展示图</span>
      </div>
      <el-form-item label="分类名称">
        <el-input v-model="form.name" placeholder="请输入分类名称"></el-input>
      </el-form-item>
      <el-form-item label="分类关键词">
        <el-input v-model="form.keyword" placeholder="请输入分类关键词" :autosize="{ minRows: 5 }" type="textarea"> </el-input>
      </el-form-item>
    </el-form>
    <template #footer>
      <span class="dialog-footer">
        <el-button @click="close">取消</el-button>
        <el-button type="primary" @click="submit" :loading="model.btnLoading">保存</el-button>
      </span>
    </template>
  </el-dialog>
</template>

<script setup>
import { reactive, ref } from 'vue';
import { createCategory, updateCategory } from '../../../api/category';
import { uploadStaticFile } from '../../../api/staticFile';
import UploadType from '../../../constants/UploadType';
import toast from '../../../utils/toast';
import { api } from '../../../config.json';

const formRef = ref();

const model = reactive({
  visible: false,
  keywordInput: '',
  index: -1,
  btnLoading: false
});

const emit = defineEmits(['complete']);

const form = reactive({
  id: '',
  name: '',
  keyword: '',
  icon: '',
  file: void 0
});

const iconChange = file => {
  form.file = file.raw;
  form.icon = URL.createObjectURL(file.raw);
};

const beforeClose = done => {
  done();
  setTimeout(() => {
    form.id = '';
    form.name = '';
    form.keyword = '';
    form.icon = '';
    form.file = void 0;

    model.keywordInput = '';
    model.index = -1;
  }, 500);
};

const submit = async () => {
  try {
    const data = {
      id: form.id,
      name: form.name,
      keyword: form.keyword
    };

    if (!form.id) {
      delete data.id;
    }

    if (data.keyword) {
      data.keyword = [...new Set(data.keyword.split(','))].join(',');
    }

    model.btnLoading = true;

    if (form.file) {
      let uploadRes = await uploadStaticFile(UploadType.CATEGORY, form.file);
      if (!uploadRes || uploadRes.resCode != 0) {
        return;
      }

      data.icon = uploadRes.data.url;
    }

    let res = form.id ? await updateCategory(data) : await createCategory(data);
    if (res && res.resCode == 0) {
      toast.success('保存成功');
      emit('complete', res.data, model.index);
      close();
    }
  } finally {
    model.btnLoading = false;
  }
};

const show = (index, row) => {
  model.index = index;
  if (index > -1 && row) {
    let keyword = row.keyword || [];

    form.id = row.id;
    form.name = row.name;
    form.keyword = keyword.length > 0 ? keyword.join(',') : '';
    if (row.icon) {
      form.icon = `${api.server}${row.icon}`;
    }
    form.showHeader = row.showHeader;
  }
  model.visible = true;
};

const close = () => {
  beforeClose(() => {
    model.visible = false;
  });
};

defineExpose({
  show,
  close
});
</script>

<style lang="scss" scoped>
.form-ref {
  .input-btn {
    border: 0;
    background-color: transparent;
    color: var(--color-primary);
    cursor: pointer;
    transition: all 0.3s;
  }

  .input-btn:hover {
    color: var(--color-info);
  }

  .upload-img {
    flex-direction: column;
    margin-bottom: 20px;

    .upload-img-view {
      height: 140px;
      width: 210px;

      img {
        height: 140px;
        width: 210px;
        object-fit: cover;
        border: 1px solid var(--color-secondary);
        border-radius: 8px;
      }
    }

    .upload-icon {
      height: 140px;
      width: 210px;
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

  .el-select {
    width: 100%;
  }
}
</style>
