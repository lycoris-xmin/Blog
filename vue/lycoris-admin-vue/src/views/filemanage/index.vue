<template>
  <page-layout :loading="model.loading">
    <div class="search-panel">
      <el-form :label-position="'top'" ref="formRef">
        <el-form-item class="form-group" label="开始时间">
          <el-date-picker v-model="model.beginTime" type="datetime" format="YYYY-MM-DD HH:mm:ss" value-format="YYYY-MM-DD HH:mm:ss" />
        </el-form-item>

        <el-form-item class="form-group" label="结束时间">
          <el-date-picker v-model="model.endTime" type="datetime" format="YYYY-MM-DD HH:mm:ss" value-format="YYYY-MM-DD HH:mm:ss" />
        </el-form-item>
        <el-form-item class="form-group" label="保存位置">
          <el-select v-model="model.uploadChannel" placeholder="- 全部 -" clearable>
            <el-option v-for="item in model.uploadChannelEnum" :key="item.value" :label="item.name" :value="item.value" />
          </el-select>
        </el-form-item>
        <el-form-item class="form-group" label="本地备份">
          <el-select v-model="model.localBack" placeholder="- 全部 -" clearable>
            <el-option :key="false" label="未备份" :value="false" />
            <el-option :key="true" label="已备份" :value="true" />
          </el-select>
        </el-form-item>
        <el-form-item class="form-group" label="文件状态">
          <el-select v-model="model.use" placeholder="- 全部 -" clearable>
            <el-option :key="false" label="未使用" :value="false" />
            <el-option :key="true" label="使用中" :value="true" />
          </el-select>
        </el-form-item>
      </el-form>
    </div>

    <lycoris-table
      ref="tableRef"
      :show-selection="true"
      :column="column"
      :page-index="table.pageIndex"
      :table-height="'calc(100vh - 385px)'"
      :page-size="table.pageSize"
      :hide-on-single-page="true"
      :list="table.list"
      :count="table.count"
      :loading="table.loading"
      :toolbar="toolbar"
      @page-change="handleCurrentChange"
      @toolbar-search="$search"
    >
      <template #toolbar>
        <el-button type="info" @click="clearSearchForm" plain>清空条件</el-button>
        <el-button type="primary" @click="syncAllFile" plain>同步远端</el-button>
        <el-button type="danger" style="width: 120px" @click="downloadAll" plain :loading="model.downloadAllLoading">下载全部文件</el-button>
      </template>

      <template #fileName="{ row }">
        <div class="file">
          <p class="file-name" @click="copyFileUrl(row.pathUrl)">{{ row.fileName }}</p>
          <p class="file-info flex-start-center">
            <span>{{ row.path }}</span>
            <span>{{ row.fileSize }}</span>
            <span class="img" v-if="row.fileType == 0">图片</span>
            <span class="audio" v-else-if="row.fileType == 1">音频</span>
            <span class="video" v-else-if="row.fileType == 2">视频</span>
            <span v-else>文件</span>
          </p>
        </div>
      </template>

      <template #uploadChannel="{ value }">
        <el-tag>{{ model.uploadChannelEnum.filter(x => x.value == value)[0].name }}</el-tag>
      </template>

      <template #localBack="{ row }">
        <el-tag v-if="row.uploadChannel == 0" type="info">本地仓库</el-tag>
        <el-tag v-else-if="row.localBack">已备份</el-tag>
        <el-tag type="danger" v-else>未备份</el-tag>
      </template>

      <template #use="{ value }">
        <el-tag v-if="value">使用中</el-tag>
        <el-tag type="warning" v-else>未使用</el-tag>
      </template>

      <template #action="{ row, index }">
        <el-button type="info" plain @click="showFileDetail(row, index)">详情</el-button>
        <el-button type="warning" plain @click="checkFileUse(row)" :loading="row.check">检测</el-button>
        <el-button type="success" v-if="row.localBack && row.uploadChannel != 0 && row.uploadChannel != model.configUploadChannel" :loading="row.syncFileToRemote" @click="syncFile(row)" plain>同步远端</el-button>
        <el-button v-if="!row.localBack" type="primary" plain @click="$viewLog(row)">本地备份</el-button>
      </template>
    </lycoris-table>

    <file-detail ref="fileDetailRef" :page-size="table.list.length == table.pageSize ? table.pageSize : table.list.length" :upload-channel-enum="model.uploadChannelEnum" @change-file="changeFile"></file-detail>
  </page-layout>
</template>

<script setup name="filemanage">
import { reactive, ref, onMounted, onBeforeMount, onBeforeUnmount, inject } from 'vue';
import pageLayout from '../layout/page-layout.vue';
import LycorisTable from '../../components/lycoris-table/index.vue';
import fileDetail from './components/file-detail.vue';
import { getList, checkFileUseState, syncFileToRemote, downAllFile } from '../../api/staticFile';
import { getUploadChannelEnum, getUploadSetting } from '../../api/configuration';
import { api } from '../../config.json';
import { setStaticSource } from '../../utils/staticfile';
import toast from '../../utils/toast';
import useClipboard from 'vue-clipboard3';
import swal from '../../utils/swal';

const signalR = inject('$signalR');
const { toClipboard } = useClipboard();

const fileDetailRef = ref();

const model = reactive({
  loading: true,
  uploadChannelEnum: [],
  beginTime: '',
  endTime: '',
  uploadChannel: '',
  localBack: '',
  use: '',
  configUploadChannel: 0,
  downloadAllLoading: false,
  checkState: []
});

const toolbar = reactive({
  search: true
});

const column = [
  {
    column: 'fileName',
    name: '文件名称'
  },
  {
    column: 'uploadChannel',
    name: '保存位置',
    width: '200px'
  },
  {
    column: 'localBack',
    name: '本地备份',
    width: '150px'
  },
  {
    column: 'use',
    name: '文件状态',
    width: '150px'
  },
  {
    column: 'createTime',
    name: '上传时间',
    width: '220px'
  },
  {
    column: 'action',
    name: '操作',
    width: '280px',
    fixed: 'right',
    align: 'left'
  }
];

const table = reactive({
  count: 0,
  list: [],
  pageIndex: 1,
  pageSize: 20,
  loading: false
});

onBeforeMount(() => {
  // setStaticSource('local');
});

onMounted(async () => {
  await getEnum();
  await getConfigUploadChannel();
  await getTableList();
  model.loading = false;

  signalR.subscribe('checkkFileUseState', subscribeCheckkFileUseState);
  signalR.subscribe('downloadAll', subscribeDownloadAll);
});

onBeforeUnmount(() => {
  setStaticSource('cdn');

  signalR.unsubscribe('checkkFileUseState');
  signalR.unsubscribe('downloadAll');
});

const getEnum = async () => {
  try {
    let res = await getUploadChannelEnum();
    if (res != null && res.resCode == 0) {
      model.uploadChannelEnum = res.data.list;
    }
  } catch (error) {}
};

const getConfigUploadChannel = async () => {
  try {
    let res = await getUploadSetting();
    if (res && res.resCode == 0) {
      model.configUploadChannel = res.data.uploadChannel;
    }
  } catch {}
};

const getTableList = async () => {
  table.loading = true;
  try {
    let res = await getList({
      ...model,
      pageIndex: table.pageIndex,
      pageSize: table.pageSize
    });

    if (res && res.resCode == 0) {
      table.count = res.data.count;
      table.list = res.data.list;
    }
  } finally {
    table.loading = false;
  }
};

const $search = () => {
  table.pageIndex = 1;
  getTableList();
};

const handleCurrentChange = pageIndex => {
  table.pageIndex = pageIndex;
  getTableList();
};

const checkFileUse = async row => {
  //
  if (model.checkState.length >= 3) {
    toast.warn('只能同时验证三个文件的使用状态');
    return;
  }
  let index = -1;

  row.check = true;
  try {
    model.checkState.push(row.id);
    let res = await checkFileUseState(row.id);
    if (res && res.resCode == 0) {
      toast.success('状态检测任务已提交后台处理', {
        grouping: true
      });
    } else {
      index = model.checkState.findIndex(x => x == row.id);
    }
  } catch {
    index = model.checkState.findIndex(x => x == row.id);
    row.check = false;
  } finally {
    if (index > -1) {
      model.checkState.splice(index, 1);
    }
  }
};

const syncFile = async row => {
  //
  row.syncFileToRemote = true;
  try {
    let res = await syncFileToRemote(row.id);
    if (res) {
      if (res.resCode == 0) {
        toast.success('同步成功');
        row.uploadChannel = model.configUploadChannel;
      } else if (res.resCode == 110) {
        toast.warn(res.resMsg);
      }
    }
  } finally {
    row.syncFileToRemote = false;
  }
};

const clearSearchForm = () => {
  model.beginTime = '';
  model.endTime = '';
  model.uploadChannel = '';
  model.localBack = '';
  model.use = '';
};

const copyFileUrl = async pathUrl => {
  //
  await toClipboard(`${api.server}${pathUrl}`);
  toast.success('复制图片链接成功');
};

const subscribeCheckkFileUseState = data => {
  if (data && data.id) {
    let index = table.list.findIndex(x => x.id == data.id);
    if (index > -1) {
      setTimeout(() => {
        if (table.list[index].use != data.use) {
          table.list[index].use = data.use;
        }

        table.list[index].check = false;
        if (data.use) {
          toast.success(`${table.list[index].fileName}：${data.message}`);
        } else {
          toast.warn(`${table.list[index].fileName}：未检测到使用状态`);
        }

        index = model.checkState.findIndex(x => x == data.id);
        if (index > -1) {
          model.checkState.splice(index, 1);
        }
      }, 1000);
    }
  }
};

let downloadAllZipFile = '';
const downloadAll = async () => {
  let result = await swal.confirm('<p>下载的文件为本地备份的文件</p><p>远端仓库的文件请自行前往远端仓库下载</p>', '下载提醒', {
    dangerouslyUseHTMLString: true
  });

  if (!result) {
    return;
  }

  //
  model.downloadAllLoading = true;
  try {
    let res = await downAllFile();
    if (res && res.resCode == 0) {
      downloadAllZipFile = res.data;
    }
  } finally {
    model.downloadAllLoading = false;
  }
};

const subscribeDownloadAll = data => {
  // 触发下载
  if (downloadAllZipFile == data) {
    //
    window.open(`${api.server}${api.routePrefix}/download/staticfile/all/${data}`, '_self');
  }
};

const syncAllFile = async () => {
  //
  let result = await swal.confirm('确定要同步所有文件到远端仓库吗？', '同步提醒');
  if (!result) {
    return;
  }

  // 请求接口
  toast.warn('暂未开发');
};

const showFileDetail = (row, index) => {
  fileDetailRef.value.show(row, index);
};

const changeFile = (index, callback) => {
  callback(table.list[index]);
};
</script>

<style lang="scss" scoped>
.file {
  .file-name {
    overflow: hidden;
    text-overflow: ellipsis;
    word-break: break-all;
    white-space: nowrap;
    cursor: pointer;
    transition: all 0.3;

    &:hover {
      color: var(--color-info);
    }
  }

  .file-info {
    font-size: 14px;
    padding-top: 5px;

    > span {
      padding-right: 15px;
      padding-bottom: 3px;
      color: var(--color-dark-light);

      &:first-child {
        width: 170px;
      }

      &:nth-child(2) {
        width: 120px;
      }

      &:last-child {
        border-radius: 5px;
        padding: 2px 6px;
        background-color: var(--color-secondary);
        font-size: 12px;
      }

      &.img {
        background-color: var(--color-info);
        color: #fff;
      }

      &.audio {
        background-color: var(--color-purple);
        color: #fff;
      }

      &.video {
        background-color: var(--color-danger);
        color: #fff;
      }
    }
  }
}
</style>
