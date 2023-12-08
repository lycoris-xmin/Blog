<template>
  <el-dialog v-model="model.visible" title="网站收录分组" width="500px" :before-close="beforeClose">
    <ul class="group-order-ul" ref="groupListRef">
      <li v-for="(item, index) in props.group" :key="item.value">
        <span>{{ item.name }}</span>
        <el-button plain type="danger" :loading="model.deleteLoading[index]" @click="handleDeleteGroup(index, item.value)">删除</el-button>
      </li>
    </ul>
    <template #footer>
      <span class="dialog-footer">
        <el-button @click="close">关闭</el-button>
        <el-button type="primary" @click="submit" :loading="model.btnLoading">保存</el-button>
      </span>
    </template>
  </el-dialog>
</template>

<script setup>
import { nextTick, reactive, ref } from 'vue';
import Sortable from 'sortablejs/modular/sortable.core.esm.js';
import { setGroupOrder, deleteGroup } from '../../../api/navigation';
import toast from '../../../utils/toast';

const groupListRef = ref();

const props = defineProps({
  group: {
    type: Array,
    require: true
  }
});

const emit = defineEmits(['groupOrder']);

const model = reactive({
  visible: false,
  list: [],
  deleteLoading: [],
  btnLoading: false
});

const beforeClose = done => {
  done();
};

const show = () => {
  model.visible = true;
  model.list = [...props.group];
  model.deleteLoading = props.group.map(() => false);

  nextTick(() => {
    new Sortable(groupListRef.value, {
      animation: 150,
      chosenClass: 'active',
      onEnd: function (evt) {
        const changeData = model.list.splice(evt.oldIndex || 0, 1);
        model.list.splice(evt.newIndex || 0, 0, changeData[0]);
      }
    });
  });
};

const close = () => {
  model.visible = false;
};

const submit = async () => {
  //
  model.btnLoading = true;

  try {
    let oldIds = props.group.map(x => x.value);
    const ids = model.list.map(x => x.value);

    if (JSON.stringify(oldIds) == JSON.stringify(ids)) {
      close();
      return;
    }

    let res = await setGroupOrder(ids);
    if (res && res.resCode == 0) {
      toast.success('保存成功');
      close();
      emit('groupOrder', model.list);
    }
  } finally {
    model.btnLoading = false;
  }
};

const handleDeleteGroup = async (index, id) => {
  model.deleteLoading[index] = true;
  try {
    let res = await deleteGroup(id);
    if (res && res.resCode == 0) {
      let index = model.list.findIndex(x => x.value == id);
      if (index > -1) {
        model.list.splice(index, 1);
        emit('groupOrder', model.list);
        toast.success('删除成功');
      }
    }
  } finally {
    model.deleteLoading[index] = false;
  }
};

defineExpose({
  show,
  close
});
</script>

<style lang="scss" scoped>
.group-order-ul {
  max-height: 800px;
  overflow-y: auto;
  padding: 10px 10px;
  border-top: 1px solid var(--color-secondary);
  border-bottom: 1px solid var(--color-secondary);

  li {
    list-style: none;
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin: 5px 0;

    border-radius: 5px;
    border: 1px solid var(--color-secondary);
    padding: 10px 15px;
    cursor: move;
    transition: all 0.4s;

    :deep(.el-button) {
      padding: 3px 8px;

      span {
        font-weight: 400;
      }
    }

    &.active {
      color: #fff;
      background-color: var(--color-primary-light);
    }
  }
}
</style>
