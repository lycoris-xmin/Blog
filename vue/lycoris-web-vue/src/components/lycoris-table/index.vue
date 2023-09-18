<template>
  <div class="lycoris-table">
    <div class="toolbar flex-start-center">
      <slot name="toolbar"> </slot>
    </div>

    <el-table ref="table" :data="props.list" stripe class="card-table" :loading="props.loading" :showHeader="props.showHeader" :row-key="props.rowKey" @selection-change="handleSelectionChange">
      <el-table-column v-if="props.showSelection" type="selection" width="55" />
      <el-table-column
        v-for="item in column"
        :key="item.column"
        :property="item.column"
        :label="item.name"
        :width="item.width"
        :formatter="item.formatter"
        :show-overflow-tooltip="item.overflow"
        :align="item.align"
        :fixed="item.fixed"
      >
        <template #default="scope">
          <slot :name="item.column" :index="scope.$index" :row="scope.row" :value="scope.row[item.column]" :scope="scope">
            {{ item.formatter(scope.$index, scope.row[item.column], scope.row) }}
          </slot>
        </template>
      </el-table-column>
    </el-table>
    <div class="table-pagination">
      <div class="table-total">共 {{ props.count }} 条记录</div>
      <el-pagination
        background
        :hide-on-single-page="props.hideOnSinglePage"
        v-model:current-page="pageIndex"
        :total="props.count"
        :page-size="props.pageSize == undefined || props.pageSize == null || props.pageSize <= 0 ? 10 : parseInt(props.pageSize)"
        :layout="props.paginationLayout || 'prev, pager, next'"
      />
    </div>
  </div>
</template>

<script setup>
import { computed, onMounted, ref, watch } from 'vue';

const table = ref();
const pageIndex = ref(1);

const props = defineProps({
  loading: {
    type: Boolean,
    default: false
  },
  showHeader: {
    type: Boolean,
    default: true
  },
  rowKey: {
    type: [Function, String],
    default: ''
  },
  showSelection: {
    type: Boolean,
    default: false
  },
  column: {
    type: Array,
    required: true
  },
  count: {
    type: Number,
    default: 0
  },
  list: {
    type: Array,
    required: true,
    default: new Array()
  },
  pageIndex: {
    type: Number,
    default: 1
  },
  pageSize: {
    type: Number,
    default: 10
  },
  paginationLayout: {
    type: String,
    default: 'prev, pager, next'
  },
  hideOnSinglePage: {
    type: Boolean,
    default: true
  },
  tableHeight: {
    type: String,
    default: ''
  }
});

const column = computed(() => {
  let column = [];
  for (let prop of props.column) {
    let data = {
      column: prop.column,
      name: prop.name,
      width: prop.width || '',
      formatter: prop.formatter && typeof prop.formatter == 'function' ? prop.formatter : void 0,
      overflow: prop.overflow || false,
      align: prop.align || 'left',
      fixed: prop.fixed || false
    };

    if (data.formatter == undefined) {
      data.formatter = (index, value, row) => {
        if (index == -1) {
          return '';
        }

        if (!row || JSON.stringify(row) == '{}') {
          return;
        }

        if (Array.isArray(value)) {
          return value;
        }

        if (JSON.stringify(value) == '{}') {
          return value;
        }

        if (typeof value == 'number') {
          return value;
        }

        return value || '';
      };
    }

    column.push(data);
  }

  return column;
});

watch(pageIndex, (val, oldVale) => {
  if (val != oldVale) {
    emit('pageChange', pageIndex);
  }
});

const emit = defineEmits(['update:selected', 'pageChange']);

onMounted(() => {
  if (props.tableHeight) {
    let table = document.querySelector('.card-table'),
      tablewrapper = table.querySelector('.el-table__inner-wrapper');

    table.style.height = props.tableHeight;
    tablewrapper.style.height = props.tableHeight;
  }

  let index = parseInt(props.pageIndex);
  pageIndex.value = isNaN(index) ? 1 : index;
});

const handleSelectionChange = rows => {
  emit('update:selected', rows);
};

const getSelectionRows = () => {
  return table.value.getSelectionRows();
};

const clearSelection = () => {
  table.value.clearSelection();
};

defineExpose({
  getSelectionRows,
  clearSelection
});
</script>

<style lang="scss" scoped>
.lycoris-table {
  :deep(.el-table) {
    border-top: 1px solid var(--color-secondary);
    margin-top: 15px;
  }

  .card-table {
    width: 100%;
    height: auto;

    :deep(.el-table__inner-wrapper) {
      height: auto;

      :deep(.el-scrollbar) {
        border-bottom: 1px solid var(--color-secondary);

        :deep(.el-scrollbar__wrap) {
          border-bottom: 1px solid var(--color-secondary);
        }
      }
    }

    :deep(.el-table__row) {
      font-size: 16px;
      cursor: default;

      a {
        text-decoration: none;
      }

      .el-tag {
        .el-tag__content {
          font-size: 16px;
        }
      }
    }
  }

  .table-pagination {
    height: 50px;
    display: flex;
    justify-content: space-between;
    align-items: center;
    font-size: 16px;
    letter-spacing: 1px;
    font-weight: 500;
    padding: 5px 20px 0 10px;

    .table-total {
      line-height: 16px;
      cursor: default;
    }
  }
}
</style>

<style lang="scss">
.el-scrollbar {
  border-bottom: 1px solid var(--color-secondary);

  .el-scrollbar__wrap {
    border-bottom: 1px solid var(--color-secondary);
  }
}
</style>
