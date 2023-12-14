<template>
  <div>
    <el-dialog v-model="model.visible" title="附件库" width="800px">
      <div class="header">
        <el-upload class="upload" :accept="model.accept" :show-file-list="false" :http-request="handleUpload" v-if="props.showUpload">
          <template #trigger>
            <el-button type="primary" plain :loading="model.uploadLoading">上传文件</el-button>
          </template>
        </el-upload>

        <div v-if="props.showFilter">
          <el-select v-model="model.fileType" clearable @change="handleFileTypeChange">
            <el-option label="图片" :value="0" />
            <el-option label="音频" :value="1" />
            <el-option label="视频" :value="2" />
            <el-option label="文件" :value="3" />
          </el-select>
        </div>
      </div>

      <div class="file-body">
        <div class="file-card" v-for="(item, index) in model.list" :key="item" @click="model.selectd = item" :class="{ active: model.selectd.url == item.url }">
          <div class="card-body">
            <el-image v-if="item.fileType == 0" :src="item.url" alt="" lazy> </el-image>
            <div class="view flex-center-center" v-else-if="item.fileType == 1">
              <el-icon :size="20">
                <component :is="'headset'"></component>
              </el-icon>
              <span>音频文件</span>
            </div>
            <div class="view flex-center-center" v-else-if="item.fileType == 2">
              <el-icon :size="20">
                <component :is="'video-play'"></component>
              </el-icon>
              <span>视频文件</span>
            </div>
            <div class="view flex-center-center" v-else>
              <el-icon :size="20">
                <component :is="'files'"></component>
              </el-icon>
              <span>文件</span>
            </div>

            <div class="file-overlay show" v-if="model.uploadLoading && model.preview == item">
              <el-icon class="is-loading">
                <component :is="'loading'"></component>
              </el-icon>
            </div>

            <div class="file-overlay" v-else>
              <el-icon @click.stop="handleViewDetail(index)">
                <component :is="'view'"></component>
              </el-icon>
              <el-icon class="check">
                <component :is="'success-filled'"></component>
              </el-icon>
            </div>
          </div>
          <div class="fileName">
            <p>{{ item.fileName }}</p>
          </div>
        </div>
        <div v-if="model.list != void 0 && model.list.length == 0" class="flex-center-center empty">
          <p>找不到该类型的附件</p>
        </div>
        <loading-line :loading="model.loading"></loading-line>
      </div>

      <div class="flex-center-center pagination">
        <el-pagination background :hide-on-single-page="true" v-model:current-page="model.pageIndex" :total="model.count" :page-size="model.pageSize" :layout="'prev, pager, next'" />
      </div>

      <template #footer>
        <span class="dialog-footer">
          <el-button @click="close">取消</el-button>
          <el-button type="primary" :disabled="!model.selectd" @click="handleSelected"> 确定 </el-button>
        </span>
      </template>
    </el-dialog>

    <el-dialog v-model="model.infoVisible" title="附件详情" width="650px">
      <div class="flex-center-center info-view-header">
        <el-image v-if="model.info.fileType == 0" :src="model.info.url" alt="" :preview-src-list="[model.info.url]" lazy> </el-image>
        <audio v-else-if="model.info.fileType == 1" :src="model.info.url" controls="controls"></audio>
        <video v-else-if="model.info.fileType == 2" :src="model.info.url" controls="controls"></video>
        <div v-else>文件</div>
      </div>
      <div class="file-info">
        <span>文件名称:</span>
        <p>{{ model.info.fileName }}</p>

        <span>文件大小:</span>
        <p>{{ model.info.fileSize }}</p>
      </div>

      <template #footer>
        <div class="flex-center-center">
          <el-button @click="changeFile(-1)">
            <el-icon>
              <component :is="'arrow-left-bold'"></component>
            </el-icon>
          </el-button>

          <el-button type="primary" plain @click="handleSelectCurrent">
            <el-icon>
              <component :is="'check'"></component>
            </el-icon>
          </el-button>

          <el-button @click="changeFile(1)">
            <el-icon>
              <component :is="'arrow-right-bold'"></component>
            </el-icon>
          </el-button>
        </div>
      </template>
    </el-dialog>
  </div>
</template>

<script setup>
import { nextTick, onMounted, reactive, watch } from 'vue';
import loadingLine from '@/components/loadings/loading-line.vue';
import { getStaticFileRepository, uploadStaticFile } from '@/api/staticFile';
import toast from '@/utils/toast';
import { api } from '@/config.json';

const props = defineProps({
  showUpload: {
    type: Boolean,
    default: true
  },
  showFilter: {
    type: Boolean,
    default: true
  },
  defaultFileType: {
    type: Number,
    default: -1
  },
  uploadFileType: {
    type: Number,
    default: 99
  }
});

const model = reactive({
  visible: false,
  infoVisible: false,
  infoIndex: 0,
  info: {},
  accept: '*',
  selectd: {
    url: '',
    fileType: -1
  },
  count: 0,
  list: void 0,
  pageIndex: 1,
  pageSize: 12,
  uploadLoading: false,
  preview: '',
  fileType: -1,
  loading: false
});

const emit = defineEmits(['selected']);

watch(
  () => model.pageIndex,
  () => {
    getList();
  }
);

onMounted(() => {
  if (!props.showFilter && props.defaultFileType > -1) {
    if (props.defaultFileType == 0) {
      model.accept = 'image/*';
    } else if (props.defaultFileType == 1) {
      model.accept = 'audio/*';
    } else if (props.defaultFileType == 2) {
      model.accept = 'video/*';
    }
  }
});

const getList = async () => {
  model.loading = true;
  try {
    let res = await getStaticFileRepository({ ...model });
    if (res && res.resCode == 0) {
      //
      model.count = res.data.count;
      model.list = res.data.list.map(x => {
        return {
          fileType: x.fileType,
          url: `${api.server}${x.url}`,
          pathUrl: x.url,
          fileName: x.fileName,
          fileSize: x.fileSize
        };
      });
    }
  } finally {
    model.loading = false;
  }
};

const show = () => {
  if (model.list == void 0) {
    if (model.fileType == -1) {
      if (props.defaultFileType != void 0 && props.defaultFileType > -1) {
        model.fileType = props.defaultFileType;
      } else {
        model.fileType = '';
      }
    }

    // 查询数据
    getList();
  }

  model.visible = true;
};

const close = () => {
  model.visible = false;
  nextTick(() => {
    model.selectd = '';
  });
};

const handleSelected = () => {
  emit('selected', model.selectd.pathUrl, model.selectd.fileType);
  close();
};

const handleUpload = async options => {
  model.uploadLoading = true;

  model.count++;
  let last = '';
  if (model.list.length == model.pageSize) {
    last = model.list.pop();
  }

  model.preview = URL.createObjectURL(options.file);
  model.list.unshift(model.preview);
  //
  try {
    let res = await uploadStaticFile(props.uploadFileType, options.file);
    if (res && res.resCode == 0) {
      model.list[0] = {
        fileType: res.data.fileType,
        url: `${api.server}${res.data.url}`,
        pathUrl: res.data.url,
        fileName: res.data.fileName,
        fileSize: res.data.fileSize
      };
      toast.success('上传成功');
    } else {
      model.count--;
      if (last) {
        model.list.push(last);
      }
    }
  } catch {
    model.count--;
    if (last) {
      model.list.push(last);
    }
  } finally {
    model.uploadLoading = false;
    model.preview = '';
  }
};

const handleFileTypeChange = () => {
  getList();
};

const handleViewDetail = index => {
  //
  model.infoIndex = index;
  model.info = model.list[index];
  model.infoVisible = true;
};

const changeFile = value => {
  if (value < 0 && model.infoIndex == 0) {
    toast.warn('这已经是第一张了', { max: 3, grouping: true });
    return;
  } else if (value > 0 && model.infoIndex + 1 == model.list.length) {
    toast.warn('这已经是最后一张了', { max: 3, grouping: true });
    return;
  }

  model.infoIndex += value;
  model.info = model.list[model.infoIndex];
};

const handleSelectCurrent = () => {
  model.selectd = model.list[model.infoIndex];
  model.infoVisible = false;
};

defineExpose({
  show,
  close
});
</script>

<style lang="scss" scoped>
.header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding-bottom: 20px;

  .upload {
    width: 100%;
    :deep(.el-upload-list) {
      width: 100%;
    }
  }
}

.file-body {
  position: relative;
  display: grid;
  grid-template-columns: repeat(6, 118px);
  grid-gap: 10px;
  margin: 10px 0;
  min-height: 110px;

  .file-card {
    cursor: pointer;
    border: 1px solid var(--color-secondary);
    border-radius: 5px;

    .card-body {
      position: relative;
      height: 110px;
      display: flex;
      justify-content: center;
      align-items: center;
      overflow: hidden;
    }

    .fileName {
      border-top: 1px solid var(--color-secondary);
      p {
        overflow: hidden;
        padding: 0 2px;
        text-overflow: ellipsis;
        word-break: break-all;
        white-space: nowrap;
      }
    }

    img {
      width: 100%;
      object-fit: cover;
    }

    .file-overlay {
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

    .view {
      flex-direction: column;

      .el-icon {
        padding-bottom: 10px;
      }
    }

    &:hover {
      .file-overlay {
        background-color: #00000036;

        .el-icon {
          display: block;
        }
      }
    }

    &.active {
      border-color: var(--color-success);

      .file-overlay {
        .check {
          display: block;
          color: var(--color-success);
        }
      }
    }
  }

  .empty {
    grid-column-start: 1;
    grid-column-end: 7;

    p {
      font-size: 22px;
      letter-spacing: 2.5px;
    }
  }
}

.pagination {
  padding-top: 25px;
}

.info-view-header {
  height: 200px;

  video {
    height: 200px;
    width: 60%;
  }

  .el-image {
    border-radius: 5px;
    border: 1px solid var(--color-secondary);
    height: 200px;
    width: 60%;
  }
}

.file-info {
  padding: 25px 15px;
  display: grid;
  grid-template-columns: 80px 1fr;
  grid-gap: 10px;
}
</style>
