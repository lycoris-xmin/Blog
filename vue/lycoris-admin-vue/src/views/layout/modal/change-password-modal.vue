<template>
  <el-dialog v-model="model.dialogVisible" title="修改密码" width="500px" :close-on-click-modal="false">
    <el-form label-position="left" label-width="80" ref="formRef" :model="form" :rules="fromRules" @submit.prevent>
      <el-form-item label="旧密码">
        <el-input v-model="form.oldPassword" type="password" autocomplete="off" />
      </el-form-item>
      <el-form-item label="密码" prop="password">
        <el-input v-model="form.password" type="password" autocomplete="off" />
      </el-form-item>
      <el-form-item label="密码确认" prop="confirmPassword">
        <el-input v-model="form.confirmPassword" type="password" autocomplete="off" />
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
import { passwordRegex } from '../../../utils/regex';
import toast from '../../../utils/toast';

const formRef = ref();

const passwordValidator = function (rule, value, callback) {
  if (value == '') {
    callback(new Error('密码不能为空'));
    return;
  }

  if (!passwordRegex(value)) {
    callback(new Error('密码必须包含大写字母，小写字母，数字，特殊符号 `@#$%^&*`~()-+=` 中任意3项密码'));
    return;
  }

  callback();
};

const passwordConfirmValidator = function (rule, value, callback) {
  if (value == '') {
    callback(new Error('密码不能为空'));
    return;
  }

  if (!passwordRegex(value)) {
    callback(new Error('密码必须包含大写字母，小写字母，数字，特殊符号 `@#$%^&*`~()-+=` 中任意3项密码'));
    return;
  }

  if (value != model.form.password) {
    callback(new Error('两次输入的密码不一致'));
    return;
  }

  callback();
};

const model = reactive({
  dialogVisible: false
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
      validator: passwordConfirmValidator,
      trigger: 'blur'
    }
  ]
});

const submit = function () {
  toast.success('修改成功');
  formRef.value.validate(valid => {
    if (valid) {
      console.log(form);
    }
  });
};

const close = function () {
  model.dialogVisible = false;
  setTimeout(() => {
    formRef.value.resetFields();
  }, 500);
};

defineExpose({
  show: () => {
    model.dialogVisible = true;
  },
  close
});
</script>

<style lang="scss" scoped>
:deep(.el-form-item) {
  margin-bottom: 25px;
}

:deep(.el-form-item:last-child) {
  margin-bottom: 20px;
}
</style>
