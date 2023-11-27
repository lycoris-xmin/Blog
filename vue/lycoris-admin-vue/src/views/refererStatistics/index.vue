<template>
  <page-layout :loading="model.loading">
    <lycoris-table
      ref="tableRef"
      :table-height="'calc(100vh - 305px)'"
      :column="column"
      :show-selection="true"
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
    </lycoris-table>
  </page-layout>
</template>

<script setup name="statistics-browse">
import { reactive, ref, onMounted } from 'vue';
import PageLayout from '../layout/page-layout.vue';
import LycorisTable from '../../components/lycoris-table/index.vue';
import { getRefererList, deleteReferer } from '../../api/browse-log';
import toast from '../../utils/toast';
import swal from '../../utils/swal';

const tableRef = ref();
const model = reactive({
  loading: true
});

const column = ref([
  {
    column: 'domain',
    name: '来源域名',
    width: '500px',
    overflow: true
  },
  {
    column: 'referer',
    name: '来源地址',
    overflow: true
  },
  {
    column: 'count',
    name: '统计次数',
    width: '120px',
    align: 'center'
  }
]);

const toolbar = reactive({
  search: true,
  delete: true
});

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
    let res = await getRefererList({
      ...table
    });
    if (res && res.resCode == 0) {
      table.count = res.data.count;
      table.list = res.data.list;
    }
  } finally {
    table.loading = false;
  }
};

const handleCurrentChange = pageIndex => {
  table.pageIndex = pageIndex;
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
      let res = await deleteReferer(ids);
      if (res && res.resCode == 0) {
        if (table.count <= table.pageSize) {
          if (ids.length == table.list.length) {
            table.list = [];
          } else {
            for (let id of ids) {
              let index = table.list.findIndex(x => x.id == id);
              table.list.splice(index, 1);
            }
          }

          table.count = table.count - ids.length;
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

const $search = () => {
  table.pageIndex = 1;
  getTableList();
};
</script>

<style lang="scss" scoped></style>
