<template>
  <el-dialog v-model="model.visible" title="文件详情" width="700">
    <div class="file-body">
      <div class="flex-center-center">
        <el-image
          v-if="model.fileType == 0"
          class="view-image"
          :src="`${api.server}${model.pathUrl}`"
          lazy
          :zoom-rate="1.2"
          :max-scale="7"
          :min-scale="0.2"
          :preview-src-list="[`${api.server}${model.pathUrl}`]"
          :initial-index="0"
          fit="cover"
        >
        </el-image>

        <div v-else-if="model.fileType == 1" class="view flex-center-center">
          <audio :src="`${api.server}${model.pathUrl}`" controls="controls"></audio>
        </div>

        <div v-else-if="model.fileType == 2" class="view flex-center-center">
          <video :src="`${api.server}${model.pathUrl}`" controls="controls"></video>
        </div>

        <div v-else class="view flex-center-center">
          <el-icon :size="70">
            <component :is="'files'"></component>
          </el-icon>
        </div>
      </div>

      <p class="file-property">
        <span class="title">文件名称</span>
        <span class="value">{{ model.fileName }}</span>
      </p>
      <p class="file-property">
        <span class="title">相对路径</span>
        <span class="value">{{ model.path }}</span>
      </p>
      <p class="file-property">
        <span class="title">本地路径</span>
        <span class="value copy" @click="copyUrl(`${api.server}${model.pathUrl}`)">{{ `${api.server}${model.pathUrl}` }}</span>
      </p>
      <p class="file-property">
        <span class="title">远端路径</span>
        <span class="value copy" @click="copyUrl(model.remoteUrl)">{{ model.remoteUrl }}</span>
      </p>
      <p class="file-property">
        <span class="title">存储位置</span>
        <span class="value">{{ uploadChannel }}</span>
      </p>
      <p class="file-property">
        <span class="title">文件大小</span>
        <span class="value">{{ model.fileSize }}</span>
      </p>
      <p class="file-property">
        <span class="title">文件Sha</span>
        <span class="value">{{ model.fileSha }}</span>
      </p>
      <p class="file-property">
        <span class="title">上传时间</span>
        <span class="value">{{ model.createTime }}</span>
      </p>
    </div>
    <template #footer>
      <div class="footer-icon" v-show="!(model.index == 0 && props.pageSize == 1)">
        <el-button @click="changeFile(-1)">
          <el-icon>
            <component :is="'arrow-left-bold'"></component>
          </el-icon>
        </el-button>
        <span class="file-index">{{ model.index + 1 }}</span>
        <el-button @click="changeFile(1)">
          <el-icon>
            <component :is="'arrow-right-bold'"></component>
          </el-icon>
        </el-button>
      </div>
    </template>
  </el-dialog>
</template>

<script setup>
import { computed, reactive } from 'vue';
import { api } from '../../../config.json';
import useClipboard from 'vue-clipboard3';
import toast from '../../../utils/toast';

const { toClipboard } = useClipboard();

const props = defineProps({
  pageSize: {
    type: Number,
    require: true
  },
  uploadChannelEnum: {
    type: Array,
    require: true
  }
});

const model = reactive({
  visible: false,
  index: 0,
  fileName: '',
  fileType: 0,
  path: '',
  pathUrl: '',
  remoteUrl: '',
  uploadChannel: 0,
  fileSize: 0,
  fileSha: '',
  createTime: ''
});

const uploadChannel = computed(() => {
  if (props.uploadChannelEnum.filter(x => x.value == model.uploadChannel).length > 0) {
    return props.uploadChannelEnum.filter(x => x.value == model.uploadChannel)[0].name;
  }

  return '';
});

const emit = defineEmits(['changeFile']);

const show = (row, index) => {
  model.index = index;
  setFileProperty(row);
  model.visible = true;
};

const close = () => {
  model.visible = false;
};

const changeFile = value => {
  if (value < 0 && model.index == 0) {
    toast.warn('这已经是第一张了', { max: 3, grouping: true });
    return;
  } else if (value > 0 && model.index + 1 == props.pageSize) {
    toast.warn('这已经是最后一张了', { max: 3, grouping: true });
    return;
  }

  model.index = model.index + value;
  emit('changeFile', model.index, row => setFileProperty(row));
};

const setFileProperty = row => {
  model.fileName = row.fileName;
  model.fileType = row.fileType || 99;
  model.path = row.path;
  model.pathUrl = row.pathUrl;
  model.remoteUrl = row.remoteUrl;
  model.uploadChannel = row.uploadChannel;
  model.fileSize = row.fileSize;
  model.fileSha = row.fileSha;
  model.createTime = row.createTime;
};

const copyUrl = async url => {
  await toClipboard(url);
  toast.success('复制成功');
};

defineExpose({
  show,
  close
});
</script>

<style lang="scss" scoped>
.file-body {
  .view-image {
    width: 250px;
    height: 160px;
    border-radius: 5px;
    border: 1px solid var(--color-secondary);
  }

  .view {
    height: 160px;

    video {
      height: 160px;
      width: 350px;
    }
  }

  .file-property {
    padding: 10px 5px;
    font-size: 16px;
    display: grid;
    grid-template-columns: 100px 1fr;

    &:nth-child(2) {
      padding-top: 30px;
    }

    .title {
      &::after {
        content: '：';
      }
    }

    .value {
      overflow: hidden;
      text-overflow: ellipsis;
      word-break: break-all;

      &.copy {
        cursor: pointer;
        transition: color 0.3s;

        &:hover {
          color: var(--color-primary);
        }
      }
    }
  }
}
.footer-icon {
  display: flex;
  justify-content: center;
  align-items: center;
}

.file-index {
  padding: 0 20px;
}
</style>
