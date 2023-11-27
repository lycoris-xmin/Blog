<template>
  <el-dialog v-model="model.visible" title="网站收录" width="500px" :close-on-click-modal="false" :before-close="beforeClose">
    <el-form label-width="100px" label-position="top" class="form-ref" ref="formRef" :rules="model.rules" :model="model">
      <el-form-item label="收录名称" prop="name">
        <el-input v-model="model.name" show-word-limit maxlength="30"></el-input>
      </el-form-item>
      <el-form-item label="收录地址" prop="domain">
        <el-input v-model="model.domain"></el-input>
      </el-form-item>
      <el-form-item label="收录分组" prop="group">
        <el-select style="width: 100%" v-model="model.group" filterable allow-create default-first-option :reserve-keyword="false" placeholder="请选择收录分组">
          <el-option v-for="item in props.group" :key="item.value" :label="item.name" :value="item.value" />
        </el-select>
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
import { createSiteNavigation, updateSiteNavigation } from '../../../api/navigation';
import { urlValidator } from '../../../utils/formValidator';
import toast from '../../../utils/toast';

const formRef = ref();

const model = reactive({
  visible: false,
  index: -1,
  id: '',
  name: '',
  group: '',
  domain: '',
  btnLoading: false,
  rules: {
    name: [
      {
        required: true,
        message: '收录名称不能为空',
        trigger: 'blur'
      }
    ],
    domain: [
      {
        required: true,
        message: '收录地址不能为空',
        trigger: 'blur'
      },
      {
        validator: urlValidator,
        trigger: 'blur'
      }
    ],
    group: [
      {
        required: true,
        message: '收录分组不能为空',
        trigger: 'blur'
      }
    ]
  }
});

const props = defineProps({
  group: {
    type: Array,
    required: true
  }
});

const emit = defineEmits(['complete']);

const beforeClose = done => {
  done();

  //
  model.id = '';
  model.name = '';
  model.group = '';
  model.domain = '';
};

const show = ({ id, name, group, domain }, index) => {
  model.index = index;
  model.id = id || '';
  model.name = name || '';
  model.group = group || '';
  model.domain = domain || '';
  model.visible = true;
};

const close = () => {
  beforeClose(() => {
    model.visible = false;
  });
};

const submit = async () => {
  await formRef.value.validate();
  model.btnLoading = true;
  try {
    if (model.index == -1) {
      let res = await createSiteNavigation({
        name: model.name,
        domain: model.domain,
        group: model.group
      });

      if (res && res.resCode == 0) {
        emit('complete', res.data);
      }
    } else {
      let res = await updateSiteNavigation({
        id: model.id,
        name: model.name,
        domain: model.domain,
        group: model.group
      });

      if (res && res.resCode == 0) {
        emit('complete', res.data, model.index);
      }
    }

    toast.success('保存成功');
    close();
  } finally {
    model.btnLoading = false;
  }
};

defineExpose({
  show
});
</script>

<style lang="scss" scoped></style>
