<template>
  <el-dialog class="friend-link-dialog" v-model="model.visible" title="友链申请" width="650px" :before-close="handleClose">
    <p style="color: var(--color-danger); margin-bottom: 10px">由于展示需要，带*的字段为必填项，感谢各位大佬的支持</p>

    <el-form ref="formRef" label-position="top" :model="form" :rules="model.rules" status-icon>
      <el-form-item label="网站名称或用户昵称" prop="name">
        <el-input v-model="form.name" />
      </el-form-item>
      <el-form-item label="网站链接" prop="link">
        <el-input v-model="form.link" />
      </el-form-item>
      <el-form-item label="头像链接" prop="icon">
        <el-input v-model="form.icon" />
      </el-form-item>
      <el-form-item label="网站介绍" prop="description">
        <el-input v-model="form.description" type="textarea" :autosize="{ minRows: 3, maxRows: 6 }" maxlength="100" show-word-limit />
      </el-form-item>
    </el-form>
    <template #footer>
      <span class="dialog-footer">
        <el-button @click="close">取消</el-button>
        <el-button type="primary" @click="submit" :loading="model.btnLoading"> 提交 </el-button>
      </span>
    </template>
  </el-dialog>
</template>

<script setup>
import { reactive, ref } from 'vue';

import { friendLinkApply } from '@/api/friendLink';
import { urlValidator } from '@/utils/formValidator';
import toast from '@/utils/toast';

const formRef = ref();

const model = reactive({
  visible: false,
  rules: {
    name: [
      {
        required: true,
        message: '请输入网站名称或用户昵称',
        trigger: 'blur'
      }
    ],
    link: [
      {
        required: true,
        message: '请输入网站链接',
        trigger: 'blur'
      },
      {
        validator: urlValidator,
        trigger: 'blur'
      }
    ],
    icon: [
      {
        required: true,
        message: '请输入头像链接',
        trigger: 'blur'
      },
      {
        validator: urlValidator,
        trigger: 'blur'
      }
    ]
  },
  btnLoading: false
});

const form = reactive({
  name: '',
  link: '',
  icon: '',
  description: ''
});

const handleClose = done => {
  formRef.value.resetFields();
  done();
};

const show = () => {
  model.visible = true;
};

const close = () => {
  handleClose(() => {
    model.visible = false;
  });
};

const submit = async () => {
  //
  try {
    if (await formRef.value.validate()) {
      // 请求接口
      model.btnLoading = true;
      let res = await friendLinkApply({ ...form });
      if (res && res.resCode == 0) {
        toast.success('提交成功');
        model.visible = false;
      }
    }
  } finally {
    if (model.btnLoading) {
      model.btnLoading = false;
    }
  }
};

defineExpose({
  show
});
</script>

<style lang="scss">
.friend-link-dialog {
  .el-dialog__body {
    overflow-y: auto;
    overflow-x: hidden;

    @media (max-width: 1920px) {
      max-height: 550px;
    }
  }
}
</style>
