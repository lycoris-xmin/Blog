<template>
  <page-layout :loading="model.loading">
    <div class="search-panel">
      <el-form :label-position="'top'" ref="formRef">
        <el-form-item class="form-group" label="开始时间">
          <el-date-picker v-model="model.beginTime" type="datetime" placeholder="" format="YYYY-MM-DD HH:mm:ss" value-format="YYYY-MM-DD HH:mm:ss" />
        </el-form-item>

        <el-form-item class="form-group" label="结束时间">
          <el-date-picker v-model="model.endTime" type="datetime" placeholder="" format="YYYY-MM-DD HH:mm:ss" value-format="YYYY-MM-DD HH:mm:ss" />
        </el-form-item>

        <el-form-item class="form-group" label="受访地址">
          <el-input v-model="model.path" placeholder="精准查询"></el-input>
        </el-form-item>

        <el-form-item class="form-group" label="来源IP">
          <el-input v-model="model.ip" placeholder="精准查询"></el-input>
        </el-form-item>

        <el-form-item class="form-group" label="跳转来源">
          <el-input v-model="model.referer" placeholder="精准查询"></el-input>
        </el-form-item>
      </el-form>
    </div>

    <lycoris-table
      ref="tableRef"
      :table-height="'calc(100vh - 385px)'"
      :show-selection="true"
      :column="column"
      :page-index="table.pageIndex"
      :page-size="table.pageSize"
      :count="table.count"
      :list="table.list"
      :loading="table.loading"
      :toolbar="toolbar"
      @page-change="handleCurrentChange"
      @toolbar-delete="$delete"
      @toolbar-search="$search"
    >
      <template #userAgent="{ value }">
        <el-tooltip effect="dark" :content="value" placement="top">
          <el-image class="ua-img" :src="getUserAgentIcon(value)" lazy></el-image>
        </el-tooltip>
      </template>
    </lycoris-table>
  </page-layout>
</template>

<script setup name="statistics-brows">
import { reactive, ref, onMounted } from 'vue';
import PageLayout from '../layout/page-layout.vue';
import LycorisTable from '../../components/lycoris-table/index.vue';
import { getList, deleteLog } from '../../api/browse-log';
import { getUserAgentIcon } from '../../utils/user-agent';
import swal from '../../utils/swal';
import toast from '../../utils/toast';

const tableRef = ref();

const toolbar = reactive({
  search: true,
  delete: true
});

const model = reactive({
  loading: true,
  beginTime: '',
  endTime: '',
  path: '',
  ip: '',
  referer: ''
});

const column = ref([
  {
    column: 'pageName',
    name: '受访页面',
    width: '300px',
    overflow: true
  },
  {
    column: 'path',
    name: '受访地址',
    overflow: true
  },
  {
    column: 'referer',
    name: '跳转来源',
    overflow: true
  },
  {
    column: 'userAgent',
    name: 'UA',
    width: '80px',
    align: 'center'
  },
  {
    column: 'ip',
    name: 'IP地址',
    width: '200px'
  },
  {
    column: 'ipAddress',
    name: 'IP归属地',
    width: '150px'
  },

  {
    column: 'createTime',
    name: '访问时间',
    width: '220px'
  }
]);

const table = reactive({
  count: 0,
  list: [],
  pageIndex: 1,
  pageSize: 20,
  loading: false
});

onMounted(async () => {
  Object.freeze(toolbar);
  Object.freeze(column);
  await getTableList();
  model.loading = false;
});

const getTableList = async () => {
  table.loading = true;
  try {
    let res = await getList({
      pageIndex: table.pageIndex,
      pageSize: table.pageSize,
      ...model
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

const $delete = async () => {
  const rows = tableRef.value.getSelectionRows();
  if (rows && rows.length == 0) {
    toast.warn('请选择要删除的数据');
    return;
  }

  let result = await swal.confirm('确定要批量删除这些数据?', '删除警告');

  if (result) {
    tableRef.value.loading.delete = true;
    let ids = rows.map(x => x.id);

    try {
      let res = await deleteLog(ids);
      if (res && res.resCode == 0) {
        if (table.count <= table.pageSize) {
          for (let id of ids) {
            let index = table.list.findIndex(x => x.id == id);
            table.list.splice(index, 1);
          }
        } else {
          getTableList();
        }
        toast.success('删除成功');
      }
    } finally {
      tableRef.value.loading.delete = false;
    }
  }
};

const handleCurrentChange = pageIndex => {
  table.pageIndex = pageIndex;
  getTableList();
};
</script>

<style lang="scss" scoped>
$ua-img-size: 35px;
.ua-img {
  height: $ua-img-size;
  width: $ua-img-size;

  :deep(img) {
    height: $ua-img-size;
    width: $ua-img-size;
    object-fit: cover;
  }
}
</style>
