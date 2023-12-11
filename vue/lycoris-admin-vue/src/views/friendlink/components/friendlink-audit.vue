<template>
  <el-dialog v-model="model.visible" title="友链审核" width="750px" :close-on-click-modal="false" @opened="handleOpened">
    <el-form label-width="100px" label-position="top" class="form-ref" ref="formRef">
      <div class="flex-center-center">
        <img class="icon" :src="model.row.icon" />
      </div>
      <el-form-item label="网站名称">
        <span class="name">{{ model.row.name }}</span>
      </el-form-item>
      <el-form-item label="网站链接">
        <a class="link" :href="model.row.link" target="_blank">{{ model.row.link }}</a>
      </el-form-item>
      <el-form-item label="网站介绍">
        <el-input v-model="model.row.description" type="textarea" :autosize="{ minRows: 8 }" readonly></el-input>
      </el-form-item>
      <el-form-item class="modal-item">
        <el-select v-model="model.row.status" placeholder="- 全部 -" @change="handleStatusChange" clearable>
          <el-option :key="1" :label="'通过'" :value="1" />
          <el-option :key="2" :label="'拒绝'" :value="2" />
        </el-select>
      </el-form-item>
      <el-form-item class="remark" label="备注">
        <el-input v-model="model.row.remark" type="textarea" :autosize="{ minRows: 6, maxRows: 10 }" maxlength="100" show-word-limit></el-input>
      </el-form-item>
    </el-form>
    <template #footer>
      <span class="dialog-footer">
        <el-button @click="close">取消</el-button>
        <el-button type="primary" @click="submit" :loading="model.btnLoading">审核</el-button>
      </span>
    </template>
  </el-dialog>
</template>

<script setup>
import { reactive } from 'vue';
import { auditFriendLink } from '../../../api/friend-link';
import toast from '../../../utils/toast';

const model = reactive({
  visible: false,
  index: -1,
  row: {
    name: '',
    status: 0,
    description: '',
    remark: ''
  },
  btnLoading: false
});

const emit = defineEmits(['complete']);

const handleOpened = () => {
  handleStatusChange(model.row.status);
};

const show = (index, row) => {
  model.index = index;
  model.row = { ...row };
  if (model.row.status == 0) {
    model.row.status = 1;
  }

  model.visible = true;
};

const close = () => {
  model.visible = false;
};

const submit = async () => {
  model.btnLoading = true;
  //
  try {
    let res = await auditFriendLink({
      ...model.row
    });

    if (res && res.resCode == 0) {
      toast.success('审核成功');
      model.row.description = res.data;
      emit('complete', model.row, model.index);
      model.visible = false;
    }
  } finally {
    model.btnLoading = false;
  }
};

const handleStatusChange = val => {
  const dom = document.querySelector('.el-form-item.remark');
  if (val == 2) {
    dom.style.height = `${dom.scrollHeight}px`;
  } else {
    dom.style.height = '0px';
  }
};

defineExpose({
  show,
  close
});
</script>

<style lang="scss" scoped>
.icon {
  height: 75px;
  width: 75px;
  border-radius: 50%;
  object-fit: cover;
  border: 1px solid var(--color-secondary);
}

span.name {
  cursor: default;
  font-size: 16px;
}

a.link {
  overflow: hidden;
  text-overflow: ellipsis;
  word-wrap: break-word;
  word-break: normal;

  color: var(--color-info);
  transition: all 0.4s;

  &:hover {
    color: var(--color-purple);
  }
}

.modal-item {
  :deep(.el-select) {
    width: 100%;
  }
}

.remark {
  margin: 0;
  height: 0px;
  overflow: hidden;
  transition: all 0.4s;
}
</style>
