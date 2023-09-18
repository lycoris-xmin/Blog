<template>
  <page-layout :loading="model.loading">
    <div class="search-panel">
      <el-form :label-position="'top'" ref="formRef">
        <el-form-item class="form-group" label="开始时间">
          <el-date-picker v-model="model.beginTime" type="datetime" placeholder="开始时间" format="YYYY-MM-DD HH:mm:ss" value-format="YYYY-MM-DD HH:mm:ss" />
        </el-form-item>

        <el-form-item class="form-group" label="结束时间">
          <el-date-picker v-model="model.endTime" type="datetime" placeholder="结束时间" format="YYYY-MM-DD HH:mm:ss" value-format="YYYY-MM-DD HH:mm:ss" />
        </el-form-item>

        <el-form-item class="form-group" label="留言内容">
          <el-input v-model="model.content" placeholder="支持模糊查询"></el-input>
        </el-form-item>

        <el-form-item class="form-group" label="来源IP">
          <el-input v-model="model.ip" placeholder="精准查询"></el-input>
        </el-form-item>

        <el-form-item class="form-group" label="状态">
          <el-select v-model="model.status" class="m-2" placeholder="- 全部 -" clearable>
            <el-option label="正常" value="0" />
            <el-option label="违规" value="1" />
            <el-option label="用户删除" value="2" />
          </el-select>
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
      :list="table.list"
      :count="table.count"
      :loading="table.loading"
      :toolbar="toolbar"
      @page-change="handleCurrentChange"
      @toolbar-delete="$delete"
      @toolbar-search="$search"
    >
      <template #createUser="{ row }">
        <el-tooltip v-if="row.isOwner" effect="dark" content="自己" placement="top">
          <span>{{ row.createUser }}</span>
        </el-tooltip>
        <span v-else>{{ row.createUser }}</span>
      </template>

      <template #status="{ row }">
        <el-tag v-if="row.status == 0 && !row.originalContent">正常</el-tag>
        <el-tag v-else-if="row.status == 0 && row.originalContent" type="warning">敏感内容</el-tag>
        <el-tag type="danger" v-else-if="row.status == 1">违规</el-tag>
        <el-tag type="info" v-else-if="row.status == 2">用户删除</el-tag>
        <el-tag v-else>未知状态</el-tag>
      </template>

      <template #action="{ row, index }">
        <el-button v-if="row.originalContent" type="success" plain @click="audit(row, index)">查看</el-button>
        <el-button v-else type="primary" plain>设置</el-button>
      </template>
    </lycoris-table>

    <audit-message ref="auditModalRef" @complete="auditComplete"></audit-message>
  </page-layout>
</template>

<script setup name="message">
import { reactive, ref, onMounted } from 'vue';
import PageLayout from '../layout/page-layout.vue';
import LycorisTable from '../../components/lycoris-table/index.vue';
import auditMessage from './components/audit-message.vue';
import { getList, deleteMessage } from '../../api/leave-message';
import swal from '../../utils/swal';
import toast from '../../utils/toast';

const tableRef = ref();
const auditModalRef = ref();

const toolbar = reactive({
  delete: true,
  search: true
});

const model = reactive({
  loading: true,
  beginTime: '',
  endTime: '',
  content: '',
  ip: '',
  status: ''
});

const column = ref([
  {
    column: 'content',
    name: '留言内容'
  },
  {
    column: 'createUser',
    name: '留言用户',
    width: '150px',
    overflow: true
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
    name: '留言时间',
    width: '220px'
  },
  {
    column: 'status',
    name: '状态',
    width: '120px',
    align: 'center'
  },
  {
    column: 'action',
    name: '操作',
    align: 'center',
    width: '100px'
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
      table.list = res.data.list || [];
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
      let res = await deleteMessage(ids);
      if (res && res.resCode == 0) {
        getTableList();
        toast.success('删除成功');
      }
    } finally {
      tableRef.value.loading.delete = false;
    }
  }
};

const audit = (row, index) => {
  auditModalRef.value.show(row, index);
};

const handleCurrentChange = pageIndex => {
  table.pageIndex = pageIndex;
  getTableList();
};

const auditComplete = index => {
  table.list[index].status = 1;
  toast.success('设置成功');
};
</script>

<style lang="scss" scoped></style>
