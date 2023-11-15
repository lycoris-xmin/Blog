<template>
  <el-dialog v-model="model.visible" title="访问管控日志" class="control-log">
    <lycoris-table
      ref="tableRef"
      :show-selection="true"
      :column="column"
      :page-index="table.pageIndex"
      :table-height="'500px'"
      :page-size="table.pageSize"
      :hide-on-single-page="true"
      :list="table.list"
      :count="table.count"
      :loading="table.loading"
      @page-change="handleCurrentChange"
    >
      <template #route="{ row }">
        <p class="request-route">
          <span class="http-method" :class="{ get: row.method == 'GET', post: row.method == 'POST' }">{{ row.method }}</span> {{ row.route }}
        </p>
      </template>
    </lycoris-table>
  </el-dialog>
</template>

<script setup>
import { reactive, ref } from 'vue';
import lycorisTable from '../../../components/lycoris-table/index.vue';
import { getAccessControlLogList } from '../../../api/accesscontrol';

const model = reactive({
  visible: false,
  id: '',
  index: ''
});

const column = ref([
  {
    column: 'route',
    name: '请求地址',
    overflow: true
  },
  {
    column: 'params',
    name: '请求参数',
    width: '250px'
  },
  {
    column: 'statusCode',
    name: '状态码',
    width: '100px'
  },
  {
    column: 'response',
    name: '响应',
    width: '220px'
  },
  {
    column: 'createTime',
    name: '请求时间',
    width: '220px'
  }
]);

const table = reactive({
  count: 0,
  list: [],
  pageIndex: 1,
  pageSize: 20,
  loading: false
});

const emit = defineEmits(['count-update']);

const show = (id, index) => {
  model.visible = true;
  model.id = id;
  model.index = index;
  getTableList();
};

const close = () => {
  model.visible = false;
  model.list = [];
  model.pageIndex = 1;
};

const getTableList = async () => {
  //

  table.loading = true;
  try {
    let res = await getAccessControlLogList({
      id: model.id,
      pageIndex: table.pageIndex,
      pageSize: table.pageSize
    });

    if (res && res.resCode == 0) {
      table.count = res.data.count;
      table.list = res.data.list;
      emit('count-update', {
        count: table.count,
        index: model.index
      });
    }
  } finally {
    table.loading = false;
  }
};

const handleCurrentChange = pageIndex => {
  table.pageIndex = pageIndex;
  getTableList();
};

defineExpose({
  show,
  close
});
</script>

<style lang="scss">
.control-log {
  width: 1400px;
}
</style>
