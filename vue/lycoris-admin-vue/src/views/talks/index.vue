<template>
  <page-layout :loading="model.loading">
    <lycoris-table
      :column="column"
      :table-height="'calc(100vh - 310px)'"
      :page-index="table.pageIndex"
      :page-size="table.pageSize"
      :list="table.list"
      :count="table.count"
      :loading="table.loading"
      :toolbar="{ search: true, create: true }"
      @page-change="handleCurrentChange"
      @toolbar-create="$create"
      @toolbar-search="$search"
    >
      <template #action="{ index, row }">
        <el-button type="primary" plain @click="$update(index, row)">编辑</el-button>
        <el-popconfirm title="确定要删除吗?" @confirm="$delete(index, row)">
          <template #reference>
            <el-button type="danger" :loading="row.loading" plain>删除</el-button>
          </template>
        </el-popconfirm>
      </template>
    </lycoris-table>

    <createorupdate ref="modalRef" @complete="modalComplete"></createorupdate>
  </page-layout>
</template>

<script setup name="talks">
import { reactive, ref, onMounted } from 'vue';
import pageLayout from '../layout/page-layout.vue';
import LycorisTable from '../../components/lycoris-table/index.vue';
import createorupdate from './components/createorupdate.vue';
import { getList, deleteTalk } from '../../api/talk';
import toast from '../../utils/toast';

const modalRef = ref();

const model = reactive({
  loading: true
});

const column = [
  {
    column: 'content',
    name: '说说内容'
  },
  {
    column: 'createTime',
    name: '发布时间',
    width: 220
  },
  {
    column: 'action',
    name: '操作',
    width: 200,
    align: 'center',
    fixed: 'right'
  }
];

const table = reactive({
  count: 0,
  list: [],
  pageIndex: 1,
  pageSize: 15,
  loading: false
});

onMounted(async () => {
  await getTableList();
  model.loading = false;
});

const getTableList = async (autoLoading = true) => {
  if (autoLoading) {
    table.loading = true;
  }
  try {
    let res = await getList({
      pageIndex: table.pageIndex,
      pageSize: table.pageSize
    });

    if (res && res.resCode == 0) {
      table.count = res.data.count;
      table.list = res.data.list;
    }
  } finally {
    if (autoLoading) {
      table.loading = false;
    }
  }
};

const handleCurrentChange = pageIndex => {
  table.pageIndex = pageIndex;
  getTableList();
};

const $search = () => {
  table.pageIndex = 1;
  getTableList();
};

const $create = () => {
  //
  modalRef.value.show({});
};

const $update = (index, row) => {
  modalRef.value.show(row, index);
};

const $delete = async (index, row) => {
  row.loading = true;

  try {
    let res = await deleteTalk(row.id);
    if (res && res.resCode == 0) {
      if (table.count <= table.pageSize) {
        table.list.splice(index, 1);
        table.count--;
      } else {
        getTableList();
      }

      toast.success('删除成功');
    }
  } finally {
    row.loading = false;
  }
};

const modalComplete = ({ index, row }) => {
  if (index == -1) {
    if (table.list.length >= table.pageSize) {
      if (table.pageIndex != 1) {
        return;
      }

      table.list.pop();
    }

    table.count++;
    table.list.unshift(row);
  } else {
    table.list[index] = row;
  }
};
</script>

<style lang="scss" scoped>
:deep(.el-statistic__number) {
  font-size: 16px;
}
</style>
