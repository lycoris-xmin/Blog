<template>
  <el-dialog v-model="model.visible" title="说说管理" width="600px" :close-on-click-modal="false" :before-close="beforeClose">
    <el-form label-width="100px" label-position="top" class="form-ref" ref="formRef">
      <el-form-item>
        <el-input v-model="model.content" :autosize="{ minRows: 14, maxRows: 14 }" type="textarea" show-word-limit maxlength="300" resize="none"></el-input>
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
import { reactive } from 'vue';
import { createorupdate } from '../../../api/talk';
import toast from '../../../utils/toast';

const model = reactive({
  visible: false,
  index: -1,
  id: '',
  oldContent: '',
  content: '',
  btnLoading: false
});

const emit = defineEmits(['complete']);

const beforeClose = done => {
  //
  model.id = '';
  model.oldContent = '';
  model.content = '';

  done();
};

const show = ({ id, content }, index) => {
  model.index = index == undefined ? -1 : index;
  model.id = id || '';
  model.oldContent = content || '';
  model.content = content || '';
  model.visible = true;
};

const close = () => {
  beforeClose(() => {
    model.visible = false;
  });
};

const submit = async () => {
  if (model.content == model.oldContent) {
    close();
    return;
  }

  model.btnLoading = true;
  try {
    //
    let data = {
      content: model.content
    };

    if (model.id) {
      data.id = model.id;
    }

    let res = await createorupdate(data);
    if (res.resCode == 0) {
      toast.success('保存成功');
      close();
      emit('complete', { index: model.index, row: res.data });
    }
  } finally {
    model.btnLoading = false;
  }
};

defineExpose({
  show
});
</script>

<style lang="scss" scoped></style>
