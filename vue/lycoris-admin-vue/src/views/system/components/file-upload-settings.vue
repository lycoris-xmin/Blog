<template>
  <div style="padding-top: 18px">
    <el-form label-position="left" :model="model" label-width="100">
      <el-row :gutter="24">
        <el-col :span="6">
          <el-form-item label="保存位置">
            <el-select v-model="model.saveChannel" placeholder="please select your zone">
              <el-option v-for="item in model.saveChannelEnum" :key="item.value" :label="item.name" :value="item.value" />
            </el-select>
          </el-form-item>
          <div style="min-height: 300px">
            <transition-list :tag="div">
              <div v-if="model.saveChannel == 10">
                <el-form-item label="服务地址">
                  <el-input v-model="minio.endpoint"></el-input>
                </el-form-item>
                <el-form-item label="AccessKey">
                  <el-input v-model="minio.accessKey"></el-input>
                </el-form-item>
                <el-form-item label="SecretKey">
                  <el-input v-model="minio.secretKey"></el-input>
                </el-form-item>
                <el-form-item label="SSL">
                  <el-select v-model="minio.ssl">
                    <el-option :value="false" label="不启用"></el-option>
                    <el-option :value="true" label="启用"></el-option>
                  </el-select>
                </el-form-item>
                <el-form-item label="默认存储桶">
                  <el-input v-model="minio.defaultBucket"></el-input>
                </el-form-item>
              </div>
              <div v-else-if="model.saveChannel == 20">
                <p>阿里云存储</p>
                <p>暂未开发</p>
              </div>
              <div v-else-if="model.saveChannel == 30">
                <p>腾讯云存储</p>
                <p>暂未开发</p>
              </div>
              <div v-else-if="model.saveChannel == 40">
                <p>华为云存储</p>
                <p>暂未开发</p>
              </div>
              <div v-else-if="model.saveChannel == 50">
                <p>七牛云存储</p>
                <p>暂未开发</p>
              </div>
              <div v-else></div>
            </transition-list>
          </div>
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
import transitionList from '../../../components/transitions/list-transition.vue';
import { getFileSaveChannelEnum, getfileUploadSettings, saveFileUploadSettings } from '../../../api/configuration';
import toast from '../../../utils/toast';

const model = reactive({
  saveChannel: '',
  loading: false,
  saveChannelEnum: []
});

const minio = reactive({
  endpoint: '',
  accessKey: '',
  secretKey: '',
  ssl: false,
  defaultBucket: ''
});

const oss = reactive({});

const cos = reactive({});

const obs = reactive({});

const kodo = reactive({});

const props = defineProps({
  value: {
    type: Number,
    required: true
  }
});

const emit = defineEmits(['tabComplete']);

onMounted(async () => {
  await getEnum();
  await getSettings();
});

const getEnum = async () => {
  try {
    let res = await getFileSaveChannelEnum();
    if (res != null && res.resCode == 0) {
      model.saveChannelEnum = res.data.list;
    }
  } catch (error) {}
};

const getSettings = async () => {
  try {
    let res = await getfileUploadSettings();
    if (res && res.resCode == 0) {
      monioSetting(res.data?.minio || {});
    }
  } finally {
    emit('tabComplete', props.value);
  }
};

const monioSetting = setting => {
  minio.endpoint = setting?.endpoint || '';
  minio.accessKey = setting?.accessKey || '';
  minio.secretKey = setting?.secretKey || '';
  minio.ssl = setting?.ssl == null ? false : setting.ssl;
  minio.defaultBucket = setting?.defaultBucket || '';
};

const submit = async () => {
  model.loading = true;
  try {
    let data = {
      saveChannel: model.saveChannel
    };

    if (data.saveChannel === 10) {
      data.minio = { ...minio };
    } else if (data.saveChannel === 20) {
      data.oss = { ...oss };
    } else if (data.saveChannel === 30) {
      data.cos = { ...cos };
    } else if (data.saveChannel === 40) {
      data.obs = { ...obs };
    } else if (data.saveChannel === 50) {
      data.kodo = { ...kodo };
    }

    let res = await saveFileUploadSettings(data);
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
