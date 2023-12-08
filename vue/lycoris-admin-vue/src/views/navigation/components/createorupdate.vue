<template>
  <el-dialog v-model="model.visible" title="网站收录" width="500px" :close-on-click-modal="false" :before-close="beforeClose">
    <el-form label-width="100px" label-position="top" class="form-ref" ref="formRef" :rules="model.rules" :model="model">
      <el-form-item label="收录名称" prop="name">
        <el-input v-model="model.name" show-word-limit maxlength="30"></el-input>
      </el-form-item>
      <el-form-item label="收录地址" prop="url">
        <el-input v-model="model.url"></el-input>
      </el-form-item>
      <el-form-item label="收录分组" prop="groupId">
        <el-select style="width: 100%" v-model="model.groupId" filterable allow-create default-first-option :reserve-keyword="false" placeholder="请选择收录分组" @change="handleGropupChange">
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
  groupId: '',
  groupName: '',
  url: '',
  newGroup: false,
  btnLoading: false,
  rules: {
    name: [
      {
        required: true,
        message: '收录名称不能为空',
        trigger: 'blur'
      }
    ],
    url: [
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
    groupId: [
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
  model.groupId = '';
  model.groupName = '';
  model.url = '';
  model.newGroup = false;
};

const show = ({ id, name, groupId, url }, index) => {
  model.index = index;
  model.id = id || '';
  model.name = name || '';
  model.groupId = groupId || '';
  model.groupName = '';
  model.url = url || '';
  model.newGroup = false;
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
      let res = await createSiteNavigation({ ...model });

      if (res && res.resCode == 0) {
        emit('complete', res.data, model.newGroup);
      }
    } else {
      let res = await updateSiteNavigation({ ...model });

      if (res && res.resCode == 0) {
        emit('complete', res.data, model.index, model.newGroup);
      }
    }

    toast.success('保存成功');
    close();
  } finally {
    model.btnLoading = false;
  }
};

const handleGropupChange = value => {
  const index = props.group.findIndex(x => x.value == value);
  if (index > -1) {
    model.newGroup = false;
    model.groupName = props.group[index].name;
  } else {
    model.newGroup = true;
    model.groupName = value;
  }
};

defineExpose({
  show
});
</script>

<style lang="scss" scoped></style>
