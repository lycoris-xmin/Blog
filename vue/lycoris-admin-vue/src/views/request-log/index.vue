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

        <el-form-item class="form-group" label="来源IP">
          <el-input v-model="model.ip" placeholder="精准查询"></el-input>
        </el-form-item>

        <el-form-item class="form-group" label="响应状态">
          <el-select v-model="model.status" class="m-2" placeholder="- 全部 -" clearable>
            <el-option label="正常" value="1" />
            <el-option label="异常" value="2" />
          </el-select>
        </el-form-item>

        <el-form-item class="form-group" label="响应等级">
          <el-select v-model="model.elapsed" class="m-2" placeholder="- 全部 -" clearable>
            <el-option label="优 (2s内)" value="1" />
            <el-option label="良 (2s-5s)" value="2" />
            <el-option label="差 (5s-10s)" value="3" />
            <el-option label="异常 (10s以上)" value="4" />
          </el-select>
        </el-form-item>

        <el-form-item class="form-group form-group-lg" label="请求地址">
          <el-input v-model="model.route" placeholder="支持模糊查询"></el-input>
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
      @toolbar-delete="$delete"
      @toolbar-search="$search"
    >
      <template #route="{ row }">
        <p class="request-route">
          <span class="http-method" :class="{ get: row.httpMethod == 'GET', post: row.httpMethod == 'POST' }">{{ row.httpMethod }}</span>
          {{ row.route }}
        </p>
      </template>

      <template #success="{ value }">
        <el-tag v-if="value">正常</el-tag>
        <el-tag type="danger" v-else>异常</el-tag>
      </template>

      <template #elapsedMilliseconds="{ value }">
        <span :style="{ color: elapsedMillisecondsColor(value) }">{{ value }}</span>
      </template>

      <template #ip="{ value }">
        <el-tag>{{ value }}</el-tag>
        <span></span>
      </template>

      <template #action="{ row }">
        <el-button type="info" plain @click="$viewLog(row)">详情</el-button>
        <el-button type="danger" v-if="!row.success && !row.route.startsWith(api.routePrefix)" plain @click="$accessContorl(row)" :loading="row.controlLoading">IP管控</el-button>
      </template>
    </lycoris-table>

    <detail-view ref="detailViewRef"></detail-view>
  </page-layout>
</template>

<script setup name="statistics-request">
import { reactive, ref, onMounted, onActivated } from 'vue';
import PageLayout from '../layout/page-layout.vue';
import LycorisTable from '../../components/lycoris-table/index.vue';
import detailView from './components/detail-view.vue';
import { getList, deleteLog, setAccessControl } from '../../api/request-log';
import swal from '../../utils/swal';
import toast from '../../utils/toast';
import { useRoute } from 'vue-router';
import { api } from '../../config.json';

const tableRef = ref();
const detailViewRef = ref();
const route = useRoute();

const model = reactive({
  loading: true,
  beginTime: '',
  endTime: '',
  ip: '',
  route: '',
  status: '',
  elapsed: ''
});

const toolbar = reactive({
  search: true,
  delete: true
});

const column = ref([
  {
    column: 'route',
    name: '路由地址',
    overflow: true
  },
  {
    column: 'success',
    name: '响应状态',
    width: '100px'
  },
  {
    column: 'elapsedMilliseconds',
    name: '耗时(ms)',
    width: '150px',
    align: 'right'
  },
  {
    column: 'ip',
    name: '来源IP',
    width: '200px'
  },
  {
    column: 'ipAddress',
    name: 'IP属地',
    width: '150px'
  },
  {
    column: 'createTime',
    name: '请求时间',
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

let mounted = true;

onMounted(async () => {
  Object.freeze(column);
  await getTableList();
  model.loading = false;
  mounted = false;
});

let searchKey = '';

onActivated(async () => {
  if (mounted) {
    return;
  }

  //
  if (route.params?.key) {
    if (searchKey != route.params.key) {
      searchKey = route.params.key;
      model.status = searchKey == 'api' ? '' : '2';
      await getTableList();
    }
  } else {
    searchKey = '';
    model.status = '';
    await getTableList();
  }
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

const elapsedMillisecondsColor = value => {
  if (value <= 3000) {
    return 'var(--color-dark-light)';
  } else if (value <= 10000) {
    return 'var(--color-warning)';
  } else {
    return 'var(--color-danger)';
  }
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

const $viewLog = async row => {
  table.loading = true;
  await detailViewRef.value.show(row);
  table.loading = false;
};

const $accessContorl = async row => {
  row.controlLoading = true;
  try {
    let res = await setAccessControl(row.ip);
    if (res && res.resCode == 0) {
      toast.success('加入IP管控成功');
    }
  } finally {
    row.controlLoading = false;
  }
};
</script>

<style lang="scss">
.request-route {
  .http-method {
    padding: 3px 8px;
    color: #fff;
    border-radius: 15px;
    font-size: 12px;
  }

  .get {
    background-color: var(--color-info);
  }

  .post {
    background-color: var(--color-purple);
  }
}
</style>
