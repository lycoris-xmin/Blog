<template>
  <el-dialog v-model="model.dialogVisible" title="登录记录" width="1200px">
    <div class="record-table">
      <el-table :data="model.list" highlight-current-row style="width: 100%" max-height="600">
        <el-table-column class-name="record-cell" property="time" label="登录时间" width="180" />
        <el-table-column class-name="record-cell" property="ip" label="登录IP" width="180" />
        <el-table-column class-name="record-cell" property="address" label="登录地址" width="180" />
        <el-table-column class-name="record-cell" property="userAgent" show-overflow-tooltip label="UA" />
      </el-table>
      <div class="record-pagination">
        <span>共 {{ model.total }} 条记录</span>
        <el-pagination :page-size="model.pagination.pageSize" layout="prev, pager, next" :total="model.total" :hide-on-single-page="true" />
      </div>
    </div>
  </el-dialog>
</template>

<script setup>
import { onBeforeMount, reactive } from 'vue'

const model = reactive({
  dialogVisible: false,
  pagination: {
    padeIndex: 1,
    pageSize: 10
  },
  total: 0,
  list: [],
  form: {}
})

onBeforeMount(() => {
  model.list = [
    {
      time: '2000-01-01 00:00:00',
      ip: '127.0.0.1',
      address: '福建省厦门市',
      userAgent: 'Mozilla/5.0 (iPhone; CPU iPhone OS 13_2_3 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/13.0.3 Mobile/15E148 Safari/604.1'
    },
    {
      time: '2000-01-01',
      ip: '127.0.0.1',
      address: '福建省厦门市'
    },
    {
      time: '2000-01-01',
      ip: '127.0.0.1',
      address: '福建省厦门市'
    },
    {
      time: '2000-01-01',
      ip: '127.0.0.1',
      address: '福建省厦门市'
    },
    {
      time: '2000-01-01',
      ip: '127.0.0.1',
      address: '福建省厦门市'
    },
    {
      time: '2000-01-01',
      ip: '127.0.0.1',
      address: '福建省厦门市'
    },
    {
      time: '2000-01-01',
      ip: '127.0.0.1',
      address: '福建省厦门市'
    },
    {
      time: '2000-01-01',
      ip: '127.0.0.1',
      address: '福建省厦门市'
    },
    {
      time: '2000-01-01',
      ip: '127.0.0.1',
      address: '福建省厦门市'
    },
    {
      time: '2000-01-01',
      ip: '127.0.0.1',
      address: '福建省厦门市'
    }
  ]

  model.total = model.list.length
})

defineExpose({
  show: () => {
    model.dialogVisible = true
  }
})
</script>

<style lang="scss" scoped>
.record-table {
  :deep(.record-cell) {
    font-size: 16px;
  }

  :deep(.el-table__row) {
    height: 55px;
    cursor: default;
  }

  .record-pagination {
    padding: 10px 20px;
    height: 65px;
    display: flex;
    justify-content: space-between;
    align-items: center;
  }
}
</style>
