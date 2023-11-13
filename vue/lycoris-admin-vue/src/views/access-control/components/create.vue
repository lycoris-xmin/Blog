<template>
  <el-dialog v-model="model.visible" width="500" title="新增访问管控">
    <el-form>
      <el-form-item label="IP">
        <el-input v-model="model.ip"></el-input>
      </el-form-item>
    </el-form>

    <template #footer>
      <span class="dialog-footer">
        <el-button @click="close">取消</el-button>
        <el-button type="primary" @click="sumit" :loading="model.loading"> 保存 </el-button>
      </span>
    </template>
  </el-dialog>
</template>

<script setup>
import { reactive } from 'vue';
import { createAccessControl } from '../../../api/accesscontrol';
import { ipRegex } from '../../../utils/regex';
import toast from '../../../utils/toast';

const model = reactive({
  visible: false,
  ip: '',
  loading: false
});

const emit = defineEmits(['sumit']);

const show = () => {
  model.visible = true;
};

const close = () => {
  model.visible = false;
  model.ip = '';
};

const sumit = async () => {
  if (!model.ip) {
    toast.warn('管控IP不能为空');
    return;
  } else if (!ipRegex(model.ip)) {
    toast.warn('IP格式错误');
    return;
  }

  model.loading = true;
  try {
    let res = await createAccessControl(model.ip);
    if (res && res.resCode == 0) {
      emit('sumit', res.data);
      close();
    }
  } finally {
    model.loading = false;
  }
};

defineExpose({
  show,
  close
});
</script>

<style lang="scss" scoped></style>
