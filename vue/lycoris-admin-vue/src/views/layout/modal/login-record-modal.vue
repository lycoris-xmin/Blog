<template>
  <el-dialog v-model="model.dialogVisible" title="登录记录" width="1200px">
    <div class="record-table">
      <lycoris-table
        :column="column"
        :page-index="table.pageIndex"
        :table-height="'512px'"
        :page-size="table.pageSize"
        :hide-on-single-page="true"
        :list="table.list"
        :count="table.count"
        :loading="table.loading"
        @page-change="handleCurrentChange"
      >
        <template #success="{ value }">
          <el-tag :type="value ? 'success' : 'danger'">{{ value ? '成功' : '失败' }}</el-tag>
        </template>

        <template #browser="{ row }">
          <el-tooltip effect="dark" :content="row.browser" placement="top" v-if="row.browserIcon">
            <img class="row-img" :src="`/icon/browser/${row.browserIcon}`" :alt="row.browser" />
          </el-tooltip>
        </template>

        <template #os="{ row }">
          <el-tooltip effect="dark" :content="row.os" placement="top" v-if="row.osIcon">
            <img class="row-img" :src="`/icon/os/${row.osIcon}`" :alt="row.os" />
          </el-tooltip>
        </template>

        <template #device="{ row }">
          <el-tooltip effect="dark" :content="row.device" placement="top" v-if="row.deviceIcon">
            <img class="row-img" :src="`/icon/device/${row.deviceIcon}`" :alt="row.device" />
          </el-tooltip>
        </template>
      </lycoris-table>
    </div>
  </el-dialog>
</template>

<script setup>
import { reactive } from 'vue';
import LycorisTable from '../../../components/lycoris-table/index.vue';
import { getList } from '../../../api/loginRecord';

const model = reactive({
  dialogVisible: false,
  form: {}
});

const column = [
  {
    column: 'loginTime',
    name: '登录时间',
    width: '200px'
  },
  {
    column: 'ip',
    name: '登录IP',
    width: '150px'
  },
  {
    column: 'ipAddress',
    name: 'IP归属地',
    width: '250px'
  },
  {
    column: 'success',
    name: '状态',
    width: '100px'
  },
  {
    column: 'browser',
    name: '浏览器',
    width: '80px',
    aligt: 'center'
  },
  {
    column: 'os',
    name: '系统',
    width: '80px',
    aligt: 'center'
  },
  {
    column: 'device',
    name: '设备',
    width: '80px',
    aligt: 'center'
  },
  {
    column: 'remark',
    name: '备注',
    overflow: true
  }
];

const table = reactive({
  count: 0,
  list: [],
  pageIndex: 1,
  pageSize: 10,
  loading: false
});

const getLoginRecordList = async () => {
  table.loading = true;
  try {
    let res = await getList({ ...table });

    if (res && res.resCode == 0) {
      table.count = res.data.count;
      table.list = res.data.list;
    }
  } finally {
    table.loading = false;
  }
};

const show = async () => {
  if (table.list.length == 0) {
    getLoginRecordList();
  }
  model.dialogVisible = true;
};

const handleCurrentChange = index => {
  table.pageIndex = index;
  getLoginRecordList();
};

defineExpose({
  show
});
</script>

<style lang="scss" scoped>
.record-table {
  .row-img {
    height: 25px;
    width: 25px;
    object-fit: fill;
    cursor: pointer;
  }
}
</style>
