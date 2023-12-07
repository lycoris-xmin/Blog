<template>
  <div class="lycoris-table" ref="lycorisTableRef">
    <div class="table-toolbar flex-start-center">
      <el-button v-if="props.toolbar && props.toolbar.create" type="primary" @click="$create" plain :loading="loading.create">新增</el-button>
      <el-button v-if="props.toolbar && props.toolbar.delete" type="danger" @click="$delete" plain :loading="loading.delete">删除</el-button>
      <el-button v-if="props.toolbar && props.toolbar.search" type="success" @click="$search" plain :loading="loading.search">查询</el-button>

      <slot name="toolbar"> </slot>
    </div>

    <el-table
      stripe
      class="card-table"
      ref="table"
      :data="props.list"
      :showHeader="props.showHeader"
      :row-key="props.rowKey"
      v-loading="props.loading"
      element-loading-text="数据加载中..."
      @selection-change="handleSelectionChange"
    >
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
      <div class="table-total">
        <span v-show="props.list.length < props.count">
          <span
            >第
            <span>{{ (props.pageIndex - 1) * props.pageSize + 1 }}</span>
            -
            <span>{{ props.pageIndex * props.pageSize < props.count ? props.pageIndex * props.pageSize : props.count }}</span>
            条记录
          </span>
          <span>/</span>
        </span>
        共 {{ props.count }} 条记录
      </div>
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
import { debounce } from '../../utils/tool';

const lycorisTableRef = ref();
const table = ref();
const pageIndex = ref(1);
const loading = ref({
  create: false,
  search: false,
  delete: false
});

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
  },
  toolbar: {
    type: Object,
    default: void 0
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

let lastPageIndex = 1;

watch(pageIndex, (val, oldVale) => {
  if (val != oldVale) {
    pageChange(val);
  }
});

const emit = defineEmits(['update:selected', 'pageChange', 'toolbar-create', 'toolbar-search', 'toolbar-delete']);

onMounted(() => {
  if (props.tableHeight) {
    let _table = lycorisTableRef.value.querySelector('.card-table'),
      _tableWrapper = lycorisTableRef.value.querySelector('.el-table__inner-wrapper');

    _table.style.height = props.tableHeight;
    _tableWrapper.style.height = props.tableHeight;
  }

  let index = parseInt(props.pageIndex);
  pageIndex.value = isNaN(index) ? 1 : index;
});

const pageChange = debounce(index => {
  if (lastPageIndex != index) {
    emit('pageChange', index);
    lastPageIndex = index;
  }
}, 100);

const handleSelectionChange = rows => {
  emit('update:selected', rows);
};

const getSelectionRows = () => {
  return table.value.getSelectionRows();
};

const clearSelection = () => {
  table.value.clearSelection();
};

const $create = e => {
  emit('toolbar-create', e);
};

const $delete = e => {
  emit('toolbar-delete', e);
};

const $search = e => {
  emit('toolbar-search', e);
};

const setPageIndex = index => {
  pageIndex.value = index;
};

defineExpose({
  getSelectionRows,
  clearSelection,
  setPageIndex,
  loading
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
          font-size: 14px;
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

  :deep(.el-scrollbar) {
    border-bottom: 1px solid var(--color-secondary);

    .el-scrollbar__wrap {
      border-bottom: 1px solid var(--color-secondary);
    }
  }

  .table-toolbar {
    :deep(.el-button) {
      width: 100px;
    }
  }
}
</style>
