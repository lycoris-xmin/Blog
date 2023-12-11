<template>
  <el-dialog v-model="model.visible" title="新增友链" width="650px" :close-on-click-modal="false">
    <el-form ref="formRef" :model="model" :rules="model.rules" label-width="100px" label-position="top" class="form-ref">
      <el-form-item label="网站名称" prop="name">
        <el-input v-model="model.name" maxlength="30" show-word-limit></el-input>
      </el-form-item>
      <el-form-item label="网站链接" prop="link">
        <el-input v-model="model.link" maxlength="500" show-word-limit></el-input>
      </el-form-item>
      <el-form-item label="头像链接" prop="icon">
        <el-input v-model="model.icon" maxlength="500" show-word-limit></el-input>
      </el-form-item>
      <el-form-item label="网站介绍" prop="description">
        <el-input v-model="model.description" type="textarea" :autosize="{ minRows: 6, maxRows: 10 }" maxlength="100" show-word-limit></el-input>
      </el-form-item>
    </el-form>
    <template #footer>
      <span class="dialog-footer">
        <el-button @click="close">取消</el-button>
        <el-button type="primary" @click="submit" :loading="model.btnLoading">保存</el-button>
      </span>
    </template>
  </el-dialog>
</template>

<script setup>
import { reactive, ref } from 'vue';
import { createFriendLink } from '../../../api/friend-link.js';
import { urlValidator } from '../../../utils/formValidator';
import toast from '../../../utils/toast';

const formRef = ref();

const model = reactive({
  visible: false,
  name: '',
  icon: '',
  link: '',
  description: '',
  btnLoading: false,
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
  }
});

const emit = defineEmits(['complete']);

const show = () => {
  model.visible = true;
};

const close = () => {
  formRef.value.resetFields();
  model.visible = false;
};

const submit = async () => {
  model.btnLoading = true;
  //
  try {
    if (await formRef.value.validate()) {
      if (model.name.length > 30) {
        toast.warn('网站名称不能超过30个字符');
        return;
      } else if (model.link.length > 500) {
        toast.warn('网站链接不能超过30个字符');
        return;
      } else if (model.icon.length > 500) {
        toast.warn('头像链接不能超过30个字符');
        return;
      }

      let res = await createFriendLink({ ...model });
      if (res && res.resCode == 0) {
        toast.success('新增友链成功');
        emit('complete', res.data);
        close();
      }
    }
  } finally {
    model.btnLoading = false;
  }
};

defineExpose({
  show,
  close
});
</script>

<style lang="scss" scoped></style>
