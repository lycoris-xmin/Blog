<template>
  <el-dialog v-model="model.visible" title="新增用户" width="500">
    <el-form>
      <el-form-item label="邮箱帐号">
        <el-input v-model="form.email" placeholder="请输入邮箱帐号" autocomplete="off"></el-input>
      </el-form-item>
      <el-form-item label="用户昵称">
        <el-input v-model="form.nickName" placeholder="请输入用户昵称" autocomplete="off"></el-input>
      </el-form-item>
      <el-form-item label="登录密码">
        <el-input v-model="form.passwrod" placeholder="请输入登录密码" type="password" show-password autocomplete="off"></el-input>
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
import { nextTick, reactive } from 'vue';
import { createUser } from '../../../api/user';
import toast from '../../../utils/toast';

const model = reactive({
  visible: false,
  loading: false
});

const form = reactive({
  nickName: '',
  email: '',
  passwrod: ''
});

const emit = defineEmits(['complete']);

const show = () => {
  form.nickName = '';
  form.email = '';
  form.passwrod = '';

  nextTick(() => {
    model.visible = true;
  });
};

const close = () => {
  model.visible = false;
};

const sumit = async () => {
  //
  model.loading = true;
  try {
    let res = await createUser({ ...form });
    if (res && res.resCode == 0) {
      toast.success('保存成功');
      emit('complete', res.data);
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
