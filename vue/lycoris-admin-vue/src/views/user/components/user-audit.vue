<template>
  <el-dialog v-model="model.visible" title="用户审核" width="500">
    <el-form class="audit-user-form" label-position="top">
      <el-form-item label="审核状态">
        <el-select v-model="model.status" placeholder="- 请选择 -" @change="handleStatusChange">
          <el-option v-for="item in model.statusOptions" :key="item.value" :label="item.name" :value="item.value" />
        </el-select>
      </el-form-item>
      <el-form-item label="审核备注" v-show="model.remarkDisplay">
        <el-input v-model="model.remark" type="textarea" :autosize="{ minRows: 3 }" maxlength="100" show-word-limit></el-input>
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
import { reactive, onMounted } from 'vue';
import { getUserStatusEnum, auditUser } from '../../../api/user';
import toast from '../../../utils/toast';

const model = reactive({
  visible: false,
  loading: false,
  index: -1,
  id: '',
  oldSatus: '',
  status: '',
  statusOptions: [],
  remark: '',
  remarkDisplay: false
});

const emit = defineEmits(['submit']);

onMounted(async () => {
  //
  try {
    let res = await getUserStatusEnum();
    if (res && res.resCode == 0) {
      model.statusOptions = res.data.list.filter(x => x.value != 0);
    }
  } catch (error) {}
});

const show = async (row, index) => {
  model.visible = true;
  model.index = index;
  model.id = row.id;
  model.remark = row.remark;
  model.oldSatus = row.status;

  if (row.status != 0) {
    model.status = row.status;
  }

  if (model.status == -1) {
    model.remarkDisplay = true;
  }
};

const close = () => {
  model.visible = false;
};

const handleStatusChange = value => {
  // 黑名单
  if (value != 1) {
    //
    model.remarkDisplay = true;
  } else {
    model.remarkDisplay = false;
  }
};

const sumit = async () => {
  //
  model.loading = true;
  try {
    let res = await auditUser({
      ...model
    });
    if (res && res.resCode == 0) {
      //
      emit('submit', {
        index: model.index,
        id: model.id,
        status: model.status,
        remark: model.remark
      });

      toast.success('审核成功');

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

<style lang="scss" scoped>
.audit-user-form {
  .el-select {
    width: 100%;
  }
}
</style>
