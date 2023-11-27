<template>
  <page-layout :loading="model.loading">
    <div class="search-panel">
      <el-form :label-position="'top'" ref="formRef">
        <el-form-item class="form-group" label="IP">
          <el-input v-model="model.ip" placeholder="精准查询"></el-input>
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
      :toolbar="{ search: true, create: true }"
      @page-change="handleCurrentChange"
      @toolbar-search="$search"
      @toolbar-create="$create"
    >
      <template #action="{ row, index }">
        <el-button plain type="info" @click="viewLog(row, index)">访问日志</el-button>
        <el-button plain type="danger" :loading="row.removeLoading" @click="remove(row, index)">移除管控</el-button>
      </template>
    </lycoris-table>

    <access-control-create ref="createModalRef" @sumit="createSumit"></access-control-create>
    <access-control-log ref="logListModalRef" @count-update="handleCountUpdate"></access-control-log>
  </page-layout>
</template>

<script setup name="access-control">
import { reactive, ref, onMounted } from 'vue';
import PageLayout from '../layout/page-layout.vue';
import LycorisTable from '../../components/lycoris-table/index.vue';
import accessControlCreate from './components/access-control-create.vue';
import accessControlLog from './components/access-control-log.vue';
import { getList, deleteAccessControl } from '../../api/accessControl';
import toast from '../../utils/toast';
import swal from '../../utils/swal';

const createModalRef = ref();
const logListModalRef = ref();

const model = reactive({
  loading: true,
  ip: ''
});

const column = ref([
  {
    column: 'ip',
    name: 'IP地址',
    width: '250px'
  },
  {
    column: 'ipAddress',
    name: 'IP归属地',
    overflow: true
  },
  {
    column: 'count',
    name: '请求次数',
    width: '150px'
  },
  {
    column: 'lastAccessTime',
    name: '最后请求时间',
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

onMounted(async () => {
  await getTableList();
  model.loading = false;
});

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

const $create = () => {
  //
  createModalRef.value.show();
};

const $search = () => {
  table.pageIndex = 1;
  getTableList();
};

const handleCurrentChange = pageIndex => {
  table.pageIndex = pageIndex;
  getTableList();
};

const remove = async (row, index) => {
  let result = await swal.confirm('确定要删除该访问管控数据吗？', '删除提示');

  if (result) {
    row.removeLoading = true;
    try {
      //
      let res = await deleteAccessControl(row.id);
      if (res && res.resCode == 0) {
        toast.success('移除访问管控成功');
        table.list.splice(index, 1);
        table.count--;
      }
    } finally {
      row.removeLoading = false;
    }
  }
};

const viewLog = (row, index) => {
  logListModalRef.value.show(row.id, index);
};

const createSumit = data => {
  if (table.list.length == table.pageSize) {
    //
    table.list.pop();
  }

  table.list.unshift(data);

  toast.success('保存成功');
};

const handleCountUpdate = ({ count, index }) => {
  if (table.list[index].count != count) {
    table.list[index].count = count;
  }
};
</script>

<style lang="scss" scoped></style>
