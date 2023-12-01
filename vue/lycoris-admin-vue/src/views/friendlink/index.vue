<template>
  <page-layout :loading="model.loading">
    <div class="search-panel">
      <el-form :label-position="'top'" ref="formRef">
        <el-form-item class="form-group" label="网站名称">
          <el-input v-model="model.name" placeholder="支持模糊查询" />
        </el-form-item>
        <el-form-item class="form-group" label="状态">
          <el-select v-model="model.status" placeholder="- 全部 -" clearable>
            <el-option :key="0" :label="'未审核'" :value="0" />
            <el-option :key="1" :label="'已审核'" :value="1" />
            <el-option :key="2" :label="'拒绝'" :value="2" />
          </el-select>
        </el-form-item>
      </el-form>
    </div>

    <lycoris-table
      :column="column"
      :table-height="'calc(100vh - 390px)'"
      :page-index="table.pageIndex"
      :page-size="table.pageSize"
      :list="table.list"
      :count="table.count"
      :loading="table.loading"
      :toolbar="{ search: true, create: true }"
      @page-change="handleCurrentChange"
      @toolbar-search="$search"
      @toolbar-create="$create"
    >
      <template #icon="{ value }">
        <div class="icon flex-center-center">
          <el-image :src="value" lazy>
            <template #error>
              <img class="error" src="/images/404.png" />
            </template>
          </el-image>
        </div>
      </template>

      <template #link="{ value }">
        <a class="link" :href="value" target="_blank">{{ value }}</a>
      </template>

      <template #status="{ value }">
        <el-tag type="info" v-if="value == 0">未审核</el-tag>
        <el-tag type="success" v-else-if="value == 1">通过</el-tag>
        <el-tag type="danger" v-else-if="value == 2">拒绝</el-tag>
      </template>

      <template #action="{ index, row }">
        <el-button type="primary" plain @click="$audit(index, row)" v-if="row.status != 1">审核</el-button>
        <el-popconfirm title="确定要删除吗?" @confirm="$delete(index, row)">
          <template #reference>
            <el-button type="danger" plain :loading="row.loading">删除</el-button>
          </template>
        </el-popconfirm>
      </template>
    </lycoris-table>

    <friendlink-create ref="createModalRef" @complete="handleComplete"></friendlink-create>
    <friendlink-audit ref="auditModalRef" @complete="handleComplete"></friendlink-audit>
  </page-layout>
</template>

<script setup name="friendlink">
import { reactive, ref, onMounted } from 'vue';
import pageLayout from '../layout/page-layout.vue';
import LycorisTable from '../../components/lycoris-table/index.vue';
import friendlinkCreate from './components/friendlink-create.vue';
import friendlinkAudit from './components/friendlink-audit.vue';
import { getList, deleteFriendLink } from '../../api/friend-link.js';

const createModalRef = ref();
const auditModalRef = ref();

const model = reactive({
  loading: true,
  name: '',
  status: ''
});

const column = ref([
  {
    column: 'icon',
    name: '网站头像',
    width: '150px'
  },
  {
    column: 'name',
    name: '网站名称',
    width: '300px',
    overflow: true
  },
  {
    column: 'link',
    name: '网站链接'
  },
  {
    column: 'status',
    name: '友链状态',
    width: '100px'
  },
  {
    column: 'createUserName',
    name: '申请用户',
    width: '200px'
  },
  {
    column: 'createTime',
    name: '申请时间',
    width: '220px'
  },
  {
    column: 'action',
    name: '操作',
    width: '170px',
    align: 'center'
  }
]);

const table = reactive({
  count: 0,
  list: [],
  pageIndex: 1,
  pageSize: 10,
  loading: false
});

onMounted(async () => {
  await getFriendLinkList();
  model.loading = false;
});

const getFriendLinkList = async () => {
  //
  table.loading = true;

  try {
    let res = await getList({
      pageIndex: table.pageIndex,
      pageSize: table.pageSize,
      name: model.name,
      status: model.status
    });

    if (res && res.resCode == 0) {
      table.count = res.data.count;
      table.list = res.data.list;
    }
  } finally {
    table.loading = false;
  }
};

const handleCurrentChange = index => {
  model.pageIndex = index;
  getFriendLinkList();
};

const $create = () => {
  createModalRef.value.show();
};

const $search = () => {
  model.pageIndex = 1;
  getFriendLinkList();
};

const $delete = async (index, row) => {
  row.loading = true;
  try {
    let res = await deleteFriendLink(row.id);
    if (res && res.resCode === 0) {
      if (table.list.length == table.count || table.list.length < table.pageSize) {
        table.list.splice(index, 1);
        table.count--;
      } else {
        getFriendLinkList();
      }
    }
  } finally {
    row.loading = false;
  }
};

const $audit = (index, row) => {
  auditModalRef.value.show(index, row);
};

const handleComplete = (row, index) => {
  if (index && index > -1) {
    table.list[index] = row;
  } else if (table.list.length < table.pageSize) {
    table.list.push(row);
  }
};
</script>

<style lang="scss" scoped>
.icon {
  :deep(.el-image) {
    min-height: 55px;
    min-width: 55px;

    img {
      height: 55px;
      width: 55px;
      border-radius: 50%;
      object-fit: cover;

      &.error {
        border: 1px solid var(--color-secondary);
      }
    }
  }
}

a.link {
  color: var(--color-info);
  transition: all 0.4s;

  &:hover {
    color: var(--color-purple);
  }
}
</style>
