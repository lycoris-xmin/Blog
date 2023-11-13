<template>
  <div style="padding-top: 18px">
    <el-form label-position="left" :model="model" label-width="120">
      <el-row :gutter="24">
        <el-col :span="6">
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
            <el-input v-model="model.emailAddress"></el-input>
          </el-form-item>
          <el-form-item label="发件箱授权码">
            <el-input v-model="model.emailPassword" type="password" show-password></el-input>
          </el-form-item>
          <el-form-item label="邮件署名">
            <el-input v-model="model.emailSignature"></el-input>
          </el-form-item>
          <div class="flex-start-center">
            <el-button @click="emailTest">邮件测试</el-button>
            <el-button type="primary" :loading="model.lodading" @click="submit">保存</el-button>
          </div>
        </el-col>
      </el-row>
    </el-form>
  </div>
</template>

<script setup>
import { onMounted, reactive } from 'vue';
import { getEmailSettings, saveEmailSettings } from '../../../api/configuration';
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
  lodading: false
});

onMounted(async () => {
  try {
    let res = await getEmailSettings();
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

const emailTest = () => {};

const submit = async () => {
  model.lodading = true;
  try {
    let res = await saveEmailSettings({ ...model });
    if (res && res.resCode == 0) {
      toast.success('保存成功');
    }
  } finally {
    model.lodading = false;
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
