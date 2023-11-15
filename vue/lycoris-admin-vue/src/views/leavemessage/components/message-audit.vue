<template>
  <el-dialog v-model="model.visible" title="原始内容" width="650px">
    <p class="message-content">{{ model.content }}</p>
    <template #footer>
      <span class="dialog-footer">
        <el-button @click="close">关闭</el-button>
        <el-button v-if="model.showSetBtn" type="danger" @click="submit" :loading="model.btnLoading">设置为违规内容</el-button>
      </span>
    </template>
  </el-dialog>
</template>

<script setup>
import { reactive, nextTick } from 'vue';
import { setViolation } from '../../../api/leave-message';
import toast from '../../../utils/toast';

const model = reactive({
  visible: false,
  showSetBtn: true,
  content: '',
  id: 0,
  index: -1
});

const emit = defineEmits(['complete']);

const show = (row, index) => {
  model.id = row.id;
  model.content = row.originalContent;
  model.index = index;
  model.showSetBtn = row.status == 0;

  model.visible = true;
};

const close = () => {
  model.visible = false;
  nextTick(() => {
    model.id = 0;
    model.content = '';
    model.index = -1;
  });
};

const submit = async () => {
  try {
    let res = await setViolation(model.id);
    if (res && res.resCode == 0) {
      toast.success('设置成功');
      emit('complete', model.index);
      close();
    }
  } catch (error) {}
};

defineExpose({
  show
});
</script>

<style lang="scss" scoped>
.message-content {
  line-height: 28px;
}
</style>
