<template>
  <div class="about-panel">
    <div class="li-list-body">
      <div>
        <el-button type="primary" plain @click="$add">添加</el-button>
      </div>
      <ul>
        <li class="li-item li-grid">
          <div v-for="item in props.title" :key="item">{{ item }}</div>
        </li>

        <li class="li-item li-grid" v-for="(item, index) in props.list" :key="item.time">
          <div class="flex-start-center">
            <span v-if="item.endTime">{{ item.beginTime }} 至 {{ item.endTime }}</span>
            <span v-else>{{ item.beginTime }} 至今</span>
          </div>
          <div class="flex-start-center">
            <span>{{ item.name }}</span>
          </div>
          <div class="flex-start-center">
            <span style="padding: 5px 0">{{ item.info }}</span>
          </div>
          <div class="li-action">
            <el-button type="warning" plain @click="$edit(item, index)"> 编辑 </el-button>
            <el-button type="danger" plain @click="$delete(item, index)"> 删除 </el-button>
          </div>
        </li>
      </ul>
    </div>

    <slot name="modal"></slot>
  </div>
</template>

<script setup>
const props = defineProps({
  title: {
    type: Array,
    default: void 0
  },
  list: {
    type: Array,
    default: void 0
  }
});

const emit = defineEmits(['add', 'edit', 'delete']);

const $add = () => {
  emit('add');
};

const $edit = (item, index) => {
  emit('edit', item, index);
};

const $delete = (item, index) => {
  emit('delete', item, index);
};
</script>

<style lang="scss" scoped>
.li-list-body {
  ul {
    margin: 10px 0;
    border-top: 1px solid var(--color-secondary);
  }

  .li-item {
    list-style: none;
    padding: 10px 5px;
    border-radius: 5px;
    border-bottom: 1px solid var(--color-secondary);
    transition: all 0.3s;

    .li-action {
      display: flex;
      justify-content: flex-start;
      align-items: center;
      gap: 10px;

      .el-icon:first-child {
        color: var(--color-warning);
      }

      .el-icon:last-child {
        color: var(--color-danger);
      }
    }

    .el-icon {
      cursor: pointer;
      transition: all 0.4s;
    }

    .el-icon:hover {
      color: var(--color-info) !important;
    }

    .time {
      gap: 10px;
    }
  }

  .li-item:hover {
    background-color: var(--color-secondary);
  }

  .li-item.li-grid {
    display: grid;
    grid-template-columns: 300px 400px 1fr 150px;
    grid-gap: 10px;

    .flex-start-center {
      span {
        cursor: default;
      }
    }
  }
}
</style>
