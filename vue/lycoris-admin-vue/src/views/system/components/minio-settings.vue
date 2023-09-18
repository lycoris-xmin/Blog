<template>
  <div style="padding-top: 18px">
    <el-form label-position="left" :model="model" label-width="100">
      <el-row :gutter="24">
        <el-col :span="6">
          <el-form-item label="服务地址">
            <el-input v-model="model.endpoint"></el-input>
          </el-form-item>
          <el-form-item label="AccessKey">
            <el-input v-model="model.accessKey"></el-input>
          </el-form-item>
          <el-form-item label="SecretKey">
            <el-input v-model="model.secretKey"></el-input>
          </el-form-item>
          <el-form-item label="SSL">
            <el-select v-model="model.ssl">
              <el-option :value="false" label="不启用"></el-option>
              <el-option :value="true" label="启用"></el-option>
            </el-select>
          </el-form-item>
          <el-form-item label="默认存储桶">
            <el-input v-model="model.defaultBucket"></el-input>
          </el-form-item>
          <div class="submit">
            <el-button type="primary" @click="submit" :loading="model.loading">保存</el-button>
          </div>
        </el-col>
      </el-row>
    </el-form>
  </div>
</template>

<script setup>
import { reactive, onMounted } from 'vue';
import { getMinioSettings, saveMinioSettings } from '../../../api/configuration';
import toast from '../../../utils/toast';

const model = reactive({
  endpoint: '',
  accessKey: '',
  secretKey: '',
  ssl: false,
  defaultBucket: '',
  loading: false
});

const props = defineProps({
  value: {
    type: Number,
    required: true
  }
});

const emit = defineEmits(['tabComplete']);

onMounted(async () => {
  try {
    let res = await getMinioSettings();
    if (res && res.resCode == 0) {
      model.endpoint = res.data.endpoint || '';
      model.accessKey = res.data.accessKey || '';
      model.secretKey = res.data.secretKey || '';
      model.ssl = res.data.ssl == null ? false : res.data.ssl;
      model.defaultBucket = res.data.defaultBucket || '';
    }
  } finally {
    emit('tabComplete', props.value);
  }
});

const submit = async () => {
  model.loading = true;
  try {
    let res = await saveMinioSettings({ ...model });
    if (res && res.resCode == 0) {
      toast.success('保存成功');
    }
  } finally {
    model.loading = false;
  }
};
</script>

<style lang="scss" scoped>
.el-select {
  width: 100%;
}

.submit {
  .el-button {
    width: 100px;
  }
}
</style>
