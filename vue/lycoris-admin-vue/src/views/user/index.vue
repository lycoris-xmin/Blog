<template>
  <page-layout :loading="model.loading">
    <div class="search-panel">
      <el-form :label-position="'top'" ref="formRef">
        <el-form-item class="form-group" label="昵称">
          <el-input v-model="model.nickName" placeholder="模糊查询"></el-input>
        </el-form-item>
        <el-form-item class="form-group" label="邮箱">
          <el-input v-model="model.email" placeholder="精准查询"></el-input>
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
      <template #avatar="{ row }">
        <div class="table-avatar">
          <el-image v-if="row.avatar.startsWith('/avatar')" :src="`${api.server}${row.avatar}`"> </el-image>
          <el-image v-else :src="row.avatar"></el-image>
        </div>
      </template>

      <template #status="{ value }">
        <el-tag type="info" v-if="value == 0">未审核</el-tag>
        <el-tag v-else-if="value == 1">已审核</el-tag>
        <el-tag type="danger" v-else-if="value == 100">帐号注销</el-tag>
      </template>

      <template #action="{ row, index }">
        <el-button plain type="info" @click="viewDetail(row)">详细</el-button>
        <el-button plain type="danger" :loading="row.auditLoading" @click="$audit(row, index)">审核</el-button>
      </template>
    </lycoris-table>

    <user-create ref="userCreateRef" @complete="handleComplete"></user-create>
    <user-detail ref="userDetailRef"></user-detail>
    <user-audit ref="userAuditRef" @submit="handleAuditUser"></user-audit>
  </page-layout>
</template>

<script setup name="user">
import { reactive, ref, onMounted } from 'vue';
import PageLayout from '../layout/page-layout.vue';
import LycorisTable from '../../components/lycoris-table/index.vue';
import { getList } from '../../api/user';
import { api } from '../../config.json';
import userCreate from './components/user-create.vue';
import userDetail from './components/user-detail.vue';
import userAudit from './components/user-audit.vue';

const userCreateRef = ref();
const userDetailRef = ref();
const userAuditRef = ref();

const model = reactive({
  loading: true,
  nickName: '',
  email: ''
});

const column = ref([
  {
    column: 'avatar',
    name: '用户头像',
    width: '150px'
  },
  {
    column: 'nickName',
    name: '用户昵称',
    overflow: true
  },
  {
    column: 'email',
    name: '邮箱',
    overflow: true
  },
  {
    column: 'status',
    name: '状态',
    width: '100px'
  },
  {
    column: 'createTime',
    name: '注册时间',
    width: '220px'
  },
  {
    column: 'action',
    name: '操作',
    width: '180px',
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
  Object.freeze(column);
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

const $create = () => {
  userCreateRef.value.show();
};

const viewDetail = row => {
  userDetailRef.value.show(row);
};

const $audit = (row, index) => {
  //
  userAuditRef.value.show(row, index);
};

const handleAuditUser = data => {
  table.list[data.index].status = data.status;
  table.list[data.index].remark = data.remark;
};

const handleComplete = data => {
  //
  if (table.list.length == table.pageSize) {
    table.list.pop();
  }

  table.list.unshift(data);
};
</script>

<style lang="scss" scoped>
.table-avatar {
  display: flex;
  justify-content: center;
  align-items: center;

  $img-size: 70px;
  :deep(.el-image) {
    height: $img-size;
    width: $img-size;
    border-radius: 50%;
    overflow: hidden;

    img {
      height: $img-size;
      width: $img-size;
      object-fit: cover;
    }
  }
}
</style>
