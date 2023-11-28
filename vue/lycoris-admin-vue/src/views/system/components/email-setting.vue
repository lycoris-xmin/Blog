<template>
  <div class="tab-panel-container">
    <div class="harf-body">
      <el-form label-position="left" :model="model" label-width="120">
        <el-form-item label="发件人名称">
          <el-input v-model="model.emailUser"></el-input>
        </el-form-item>
        <el-form-item label="STMP服务器">
          <el-input v-model="model.stmpServer"></el-input>
        </el-form-item>
        <el-form-item label="SMTP端口">
          <el-input v-model="model.stmpPort" type="number"></el-input>
        </el-form-item>
        <el-form-item label="启用SSL">
          <el-select v-model="model.useSSL">
            <el-option :key="0" :label="'不启用'" :value="false" />
            <el-option :key="1" :label="'启用'" :value="true" />
          </el-select>
        </el-form-item>
        <el-form-item label="发件箱地址">
          <el-input v-model="model.emailAddress" autocomplete="off"></el-input>
        </el-form-item>
        <el-form-item label="发件箱授权码">
          <el-input v-model="model.emailPassword" type="password" show-password autocomplete="off"></el-input>
        </el-form-item>
        <el-form-item label="邮件署名">
          <el-input v-model="model.emailSignature"></el-input>
        </el-form-item>
        <div class="flex-start-center">
          <el-button @click="dialogShow">邮件测试</el-button>
          <el-button type="primary" :loading="model.lodading" @click="submit">保存</el-button>
        </div>
      </el-form>
    </div>

    <el-dialog v-model="model.visible" title="邮件服务测试" width="500px">
      <el-input v-model="model.testEmail" placeholder="请输入测试邮件地址"></el-input>
      <template #footer>
        <span class="dialog-footer">
          <el-button @click="dialogClose">取消</el-button>
          <el-button type="primary" @click="dialogSumit" :loading="model.testLoading"> 发送 </el-button>
        </span>
      </template>
    </el-dialog>
  </div>
</template>

<script setup>
import { onMounted, reactive } from 'vue';
import { getEmailSetting, saveEmailSetting, sendTestEmail } from '../../../api/configuration';
import toast from '../../../utils/toast';

const props = defineProps({
  value: {
    type: Number,
    required: true
  }
});

const emit = defineEmits(['tabComplete']);

const model = reactive({
  emailAddress: '',
  emailUser: '',
  stmpServer: '',
  stmpPort: '',
  emailPassword: '',
  emailSignature: '',
  useSSL: false,
  lodading: false,
  visible: false,
  testEmail: '',
  testLoading: false
});

onMounted(async () => {
  try {
    let res = await getEmailSetting();
    if (res && res.resCode == 0) {
      model.emailAddress = res.data.emailAddress || '';
      model.emailUser = res.data.emailUser || '';
      model.stmpServer = res.data.stmpServer || '';
      model.stmpPort = res.data.stmpPort || '';
      model.emailPassword = res.data.emailPassword || '';
      model.emailSignature = res.data.emailSignature || '';
      model.useSSL = res.data.useSSL || false;
    }
  } finally {
    emit('tabComplete', props.value);
  }
});

const submit = async () => {
  model.lodading = true;
  try {
    let res = await saveEmailSetting({ ...model });
    if (res && res.resCode == 0) {
      toast.success('保存成功');
    }
  } finally {
    model.lodading = false;
  }
};

const dialogShow = () => {
  if (!model.emailUser) {
    toast.warn('发件人名称不能为空');
    return;
  } else if (!model.stmpServer) {
    toast.warn('STMP服务器不能为空');
    return;
  } else if (!model.stmpPort) {
    toast.warn('SMTP端口不能为空');
    return;
  } else if (!model.emailAddress) {
    toast.warn('发件箱地址不能为空');
    return;
  } else if (!model.emailPassword) {
    toast.warn('发件箱授权码不能为空');
    return;
  } else if (!model.emailSignature) {
    toast.warn('邮件署名不能为空');
    return;
  }

  model.visible = true;
};

const dialogClose = () => {
  model.visible = false;
};

const dialogSumit = async () => {
  if (!model.testEmail) {
    toast.warn('测试邮件地址不能为空');
    return;
  }

  if (model.testEmail == model.emailAddress) {
    toast.warn('测试邮件地址不能与邮件服务发件箱地址相同');
    return;
  }

  model.testLoading = true;
  try {
    let res = await sendTestEmail({ ...model });
    if (res && res.resCode == 0) {
      //
      toast.success('测试邮件发送成功');
      dialogClose();
    }
  } finally {
    model.testLoading = false;
  }
};
</script>

<style lang="scss" scoped>
.el-select {
  width: 100%;
}

.flex-start-center {
  .el-button {
    width: 120px;
  }
}
</style>
