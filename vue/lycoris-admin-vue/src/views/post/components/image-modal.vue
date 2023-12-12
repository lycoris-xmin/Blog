<template>
  <el-dialog v-model="model.visible" title="附件图库" width="800px">
    <el-upload class="upload" :show-file-list="false" :http-request="handleUpload">
      <template #trigger>
        <el-button type="primary" plain :loading="model.uploadLoading">上传文件</el-button>
      </template>
    </el-upload>

    <div class="img-body">
      <div class="img-card" v-show="model.uploadLoading && model.bmp">
        <img :src="model.bmp" alt="" />
        <div class="img-overlay show">
          <el-icon class="is-loading">
            <component :is="'loading'"></component>
          </el-icon>
        </div>
      </div>
      <div class="img-card" v-for="item in model.images" :key="item" @click="model.selectd = item" :class="{ active: model.selectd == item }">
        <img :src="item" alt="" />
        <div class="img-overlay">
          <el-icon>
            <component :is="'view'"></component>
          </el-icon>
          <el-icon class="check">
            <component :is="'success-filled'"></component>
          </el-icon>
        </div>
      </div>
    </div>

    <template #footer>
      <span class="dialog-footer">
        <el-button @click="close">取消</el-button>
        <el-button type="primary" :disabled="!model.selectd" @click="handleSelected"> 确定 </el-button>
      </span>
    </template>
  </el-dialog>
</template>

<script setup>
import { nextTick, reactive } from 'vue';
import { uploadPostIcon } from '@/api/post';
import toast from '../../../utils/toast';

const model = reactive({
  visible: false,
  images: ['/images/404.png', '/images/bg.jpg', '/images/technology-is-not-good.jpeg'],
  selectd: '',
  pageIndex: 1,
  pageSize: 12,
  uploadLoading: false,
  bmp: ''
});

const emit = defineEmits(['selected']);

const show = () => {
  model.visible = true;
};

const close = () => {
  model.visible = false;
  nextTick(() => {
    model.selectd = '';
  });
};

const handleSelected = () => {
  emit('selected', model.selectd);
  close();
};

const handleUpload = async options => {
  model.uploadLoading = true;
  model.bmp = URL.createObjectURL(options.file);
  //
  try {
    let res = await uploadPostIcon(options.file);
    if (res && res.resCode == 0) {
      if (model.images.length == model.pageSize) {
        model.images.pop();
      }

      model.images.unshift(res.data);
      toast.success('上传成功');
    }
  } finally {
    model.uploadLoading = false;
    model.bmp = '';
  }
};

defineExpose({
  show,
  close
});
</script>

<style lang="scss" scoped>
.upload {
  margin-bottom: 20px;
  width: 100%;
  :deep(.el-upload-list) {
    width: 100%;
  }
}

.img-body {
  display: grid;
  grid-template-columns: repeat(6, 1fr);
  grid-gap: 10px;
  margin: 10px 0;

  .img-card {
    position: relative;
    display: flex;
    justify-content: center;
    align-items: center;
    overflow: hidden;
    height: 110px;
    border: 1px solid var(--color-secondary);
    border-radius: 5px;
    cursor: pointer;

    img {
      width: 100%;
      object-fit: cover;
    }

    .img-overlay {
      z-index: 999;
      position: absolute;
      left: 0;
      top: 0;
      right: 0;
      bottom: 0;
      background-color: transparent;
      display: flex;
      justify-content: flex-end;
      align-items: flex-start;

      .el-icon {
        margin-top: 5px;
        margin-right: 10px;
        display: none;
        font-size: 20px;

        &:hover {
          color: var(--color-danger);
        }
      }

      &.show {
        justify-content: center;
        align-items: center;
        background-color: #00000036;

        .el-icon {
          margin: 0;
          display: block;
          font-size: 36px;
          color: var(--color-danger);
        }
      }
    }

    &:hover {
      .img-overlay {
        background-color: #00000036;

        .el-icon {
          display: block;
        }
      }
    }

    &.active {
      border-color: var(--color-success);

      .img-overlay {
        .check {
          display: block;
          color: var(--color-success);
        }
      }
    }
  }
}
</style>
