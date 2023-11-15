<template>
  <page-layout :loading="loading">
    <lycoris-table
      :column="column"
      :table-height="'calc(100vh - 310px)'"
      :page-index="table.pageIndex"
      :page-size="table.pageSize"
      :list="table.list"
      :count="table.count"
      :loading="table.loading"
      :toolbar="toolbar"
      @page-change="handleCurrentChange"
      @toolbar-create="$create"
      @toolbar-search="$search"
    >
      <template #icon="{ value }">
        <div class="icon-template flex-center-center">
          <img :src="value" onerror="javascript:this.src='/images/404.png'" />
        </div>
      </template>

      <template #keyword="{ value }">
        <div v-if="value.length" class="keyword-group flex-start-center">
          <el-tag v-for="item in value" :key="item"> {{ item }}</el-tag>
        </div>
        <div v-else>-</div>
      </template>

      <template #postCount="{ value }">
        <el-statistic :value="value" :class="{ 'rang-value': value > 0 }"> </el-statistic>
      </template>

      <template #action="{ index, row }">
        <el-button type="primary" plain @click="$update(index, row)">编辑</el-button>
        <el-popconfirm title="确定要删除吗?" @confirm="$delete(index, row)">
          <template #reference>
            <el-button type="danger" :loading="row.loading" plain>删除</el-button>
          </template>
        </el-popconfirm>
      </template>
    </lycoris-table>
    <createorupdate ref="modal" @complete="complete"> </createorupdate>
  </page-layout>
</template>

<script setup name="category">
import { reactive, ref, onMounted } from 'vue';
import PageLayout from '../layout/page-layout.vue';
import LycorisTable from '../../components/lycoris-table/index.vue';
import createorupdate from './components/createorupdate.vue';
import { getList, deleteCategory } from '../../api/category';
import toast from '../../utils/toast';

const modal = ref();
const loading = ref(true);

const toolbar = reactive({
  search: true,
  create: true
});

const column = ref([
  {
    column: 'icon',
    name: '分类图片',
    width: 230
  },
  {
    column: 'name',
    name: '分类名称',
    width: 250
  },
  {
    column: 'keyword',
    name: '关键词'
  },
  {
    column: 'postCount',
    name: '分类文章数',
    width: 220,
    align: 'center'
  },
  {
    column: 'createTime',
    name: '创建时间',
    width: 220
  },
  {
    column: 'action',
    name: '操作',
    width: 200,
    fixed: 'right'
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
  await getTableList(false);
  loading.value = false;
});

const getTableList = async () => {
  table.loading = true;

  try {
    let res = await getList({
      pageIndex: table.pageIndex,
      pageSize: table.pageSize
    });

    if (res && res.resCode == 0) {
      table.count = res.data.count;
      table.list = res.data.list;

      toast.success('查询成功');
    }
  } finally {
    table.loading = false;
  }
};

const $search = () => {
  table.pageIndex = 1;
  getTableList();
};

const $create = () => {
  modal.value.show(-1);
};

const $update = (index, row) => {
  modal.value.show(index, row);
};

const complete = (row, index) => {
  if (index == -1) {
    if (table.list.length < table.pageSize) {
      table.list.push(row);
    }
  } else {
    table.list[index] = row;
  }
};

const $delete = async (index, row) => {
  row.loading = true;

  try {
    let res = await deleteCategory(row.id);
    if (res && res.resCode == 0) {
      toast.success('删除成功');

      if (table.list.length == table.count || table.list.length < table.pageSize) {
        table.list.splice(index, 1);
      } else {
        getTableList();
      }
    }
  } finally {
    row.loading = false;
  }
};

const handleCurrentChange = pageIndex => {
  table.pageIndex = pageIndex;
  getTableList();
};
</script>

<style lang="scss" scoped>
.icon-template {
  height: 140px;
  width: 210px;

  img {
    height: 140px;
    width: 210px;
    object-fit: cover;
    border: 1px solid var(--color-secondary);
    border-radius: 8px;
    cursor: pointer;
  }
}

.keyword-group {
  gap: 10px;
}

.el-statistic :deep(.el-statistic__number) {
  font-size: 16px;
  color: var(--color-danger);
}

.rang-value :deep(.el-statistic__number) {
  color: var(--color-success) !important;
}
</style>
