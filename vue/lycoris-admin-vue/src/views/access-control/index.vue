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
      :toolbar="{ search: true }"
      @page-change="handleCurrentChange"
      @toolbar-search="$search"
    >
      <template #action="{ row }">
        <el-button plain type="danger" :loading="row.removeLoading" @click="remove">移除管控</el-button>
      </template>
    </lycoris-table>
  </page-layout>
</template>

<script setup name="access-control">
import { reactive, ref, onMounted } from 'vue';
import PageLayout from '../layout/page-layout.vue';
import LycorisTable from '../../components/lycoris-table/index.vue';
import { getList } from '../../api/accesscontrol';

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
    width: '200px',
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

const $search = () => {
  table.pageIndex = 1;
  getTableList();
};

const handleCurrentChange = pageIndex => {
  table.pageIndex = pageIndex;
  getTableList();
};

const remove = row => {
  row.removeLoading = true;
  try {
    //
  } finally {
    row.removeLoading = false;
  }
};
</script>

<style lang="scss" scoped></style>
