<template>
  <el-dialog v-model="model.visible" title="修改密码" width="500px" :close-on-click-modal="false">
    <el-form label-position="left" label-width="80" ref="formRef" :model="form" :rules="fromRules" @submit.prevent>
      <el-form-item label="旧密码">
        <el-input v-model="form.oldPassword" type="password" placeholder="未设置过登录密码，可不填" show-password autocomplete="off" />
      </el-form-item>
      <el-form-item label="密码" prop="password">
        <el-input v-model="form.password" type="password" show-password autocomplete="off" />
      </el-form-item>
      <el-form-item label="密码确认" prop="confirmPassword">
        <el-input v-model="form.confirmPassword" type="password" show-password autocomplete="off" />
      </el-form-item>
    </el-form>
    <template #footer>
      <span class="dialog-footer">
        <el-button @click="close">取消</el-button>
        <el-button type="primary" @click="submit">保存</el-button>
      </span>
    </template>
  </el-dialog>
</template>

<script setup>
import { reactive, ref } from 'vue';
import { changePassword } from '../../../api/authentication';
import { passwordValidator, passwordConfirmValidator } from '../../../utils/formValidator';
import toast from '../../../utils/toast';

const formRef = ref();

const model = reactive({
  visible: false
});

const form = reactive({
  oldPassword: '',
  password: '',
  confirmPassword: ''
});

const fromRules = reactive({
  password: [
    {
      validator: passwordValidator,
      trigger: 'blur'
    }
  ],
  confirmPassword: [
    {
      validator: (rule, value, callback) => passwordConfirmValidator(form.password, rule, value, callback),
      trigger: 'blur'
    }
  ]
});

const submit = async () => {
  try {
    let result = await formRef.value.validate();
    if (result) {
      //
      let res = await changePassword({ ...form });
      if (res && res.resCode == 0) {
        toast.success('修改成功');
        close();
        return;
      }
    }
  } catch (error) {}
};

const close = function () {
  model.visible = false;
  setTimeout(() => {
    formRef.value.resetFields();
  }, 500);
};

defineExpose({
  show: () => {
    model.visible = true;
  },
  close
});
</script>

<style lang="scss" scoped>
:deep(.el-form-item) {
  margin-bottom: 30px;
}

:deep(.el-form-item:last-child) {
  margin-bottom: 20px;
}
</style>
