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
            <el-option :key="false" label="未确认" :value="false" />
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

      <template #pathUrl="{ row }">
        <el-popover placement="bottom" :width="400" trigger="hover">
          <template #reference>
            <span class="file-name" @click="copyFileUrl(row.pathUrl)">{{ row.pathUrl }}</span>
          </template>
          <template #default>
            <div class="staticfile-img-preview">
              <el-image v-if="row.remoteUrl" :src="row.remoteUrl">
                <template #error>
                  <img :src="`${api.server}${row.pathUrl}`" />
                </template>
              </el-image>
              <img v-else :src="`${api.server}${row.pathUrl}`" />
            </div>
          </template>
        </el-popover>
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

      <template #action="{ row }">
        <el-button type="warning" plain @click="checkFileUse(row)" :loading="row.check">状态检测</el-button>
        <el-button v-if="row.uploadChannel != model.configUploadChannel" type="success" :loading="row.syncFileToRemote" @click="syncFile(row)" plain>同步远端</el-button>
        <el-button v-if="!row.localBack" type="primary" plain @click="$viewLog(row)">本地备份</el-button>
      </template>
    </lycoris-table>
  </page-layout>
</template>

<script setup name="file-manage">
import { reactive, ref, onMounted, onBeforeMount, onBeforeUnmount, inject } from 'vue';
import PageLayout from '../layout/page-layout.vue';
import LycorisTable from '../../components/lycoris-table/index.vue';
import { getList, checkFileUseState, syncFileToRemote, downAllFile } from '../../api/staticFile';
import { getUploadChannelEnum, getStaticFileSettings } from '../../api/configuration';
import { api } from '../../config.json';
import { setStaticSource } from '../../utils/staticfile';
import toast from '../../utils/toast';
import useClipboard from 'vue-clipboard3';
import swal from '../../utils/swal';

const signalR = inject('$signalR');
const { toClipboard } = useClipboard();

const model = reactive({
  loading: true,
  uploadChannelEnum: [],
  beginTime: '',
  endTime: '',
  uploadChannel: '',
  localBack: '',
  use: '',
  configUploadChannel: 0,
  downloadAllLoading: false
});

const toolbar = reactive({
  search: true
});

const column = ref([
  {
    column: 'pathUrl',
    name: '文件名称',
    overflow: true
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
    width: '250px',
    fixed: 'right',
    align: 'left'
  }
]);

const table = reactive({
  count: 0,
  list: [],
  pageIndex: 1,
  pageSize: 20,
  loading: false
});

onBeforeMount(() => {
  setStaticSource('local');
});

onMounted(async () => {
  Object.freeze(column);
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
    let res = await getStaticFileSettings();
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
  row.check = true;
  try {
    let res = await checkFileUseState(row.id);
    if (res && res.resCode == 0) {
      //
      toast.success('状态检测任务已提交后台处理');
    }
  } catch {
    row.check = false;
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
};
</script>

<style lang="scss" scoped>
.file-name {
  cursor: pointer;
  transition: all 0.3;

  &:hover {
    color: var(--color-info);
  }
}

$imgMaxWidth: 1400px;

.staticfile-img-preview {
  position: relative;
  max-height: 800px;
  max-width: $imgMaxWidth;
  overflow-x: hidden;
  overflow-y: auto;

  :deep(img) {
    max-width: $imgMaxWidth;
    object-fit: cover;
    border-radius: 5px;
  }
}
</style>

<style>
.el-popper:has(.staticfile-img-preview) {
  height: auto !important;
  width: auto !important;
}
</style>
