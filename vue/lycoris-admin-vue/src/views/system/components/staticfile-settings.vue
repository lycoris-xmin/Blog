<template>
  <div style="padding-top: 18px">
    <el-form label-position="left" :model="model" label-width="120">
      <el-row :gutter="24">
        <el-col :span="8">
          <el-form-item label="保存位置">
            <el-select v-model="model.uploadChannel" placeholder="请选择保存位置">
              <el-option v-for="item in model.uploadChannelEnum" :key="item.value" :label="item.name" :value="item.value" />
            </el-select>
          </el-form-item>
          <el-form-item label="本地备份" v-if="model.uploadChannel != 0">
            <el-select v-model="model.localBackup" @change="localBackupChange">
              <el-option :key="false" :label="'不启用'" :value="false" />
              <el-option :key="true" :label="'启用'" :value="true" />
            </el-select>
          </el-form-item>
          <el-form-item label="加载方式" v-if="model.uploadChannel != 0">
            <el-select v-model="model.loadFileSrc" :disabled="model.loadFileSrcState">
              <el-option :key="0" :label="'本地仓库'" :value="0" />
              <el-option :key="1" :label="'远端仓库'" :value="1" />
            </el-select>
          </el-form-item>
          <div>
            <transition-list tag="div">
              <div v-if="model.uploadChannel == 10">
                <el-form-item label="Github仓库">
                  <el-input v-model="github.repositoryUrl" placeholder="https://github.com/user/repository.git"></el-input>
                </el-form-item>
                <el-form-item label="AccessToken">
                  <el-input v-model="github.accessToken"></el-input>
                </el-form-item>
                <el-form-item label="上传者名称">
                  <el-input v-model="github.committerName"></el-input>
                </el-form-item>
                <el-form-item label="上传者邮箱">
                  <el-input v-model="github.committerEmail"></el-input>
                </el-form-item>
                <el-form-item label="JsDelivrCDN">
                  <el-input v-model="github.cdn"></el-input>
                </el-form-item>
              </div>
              <div v-else-if="model.uploadChannel == 20">
                <el-form-item label="服务地址">
                  <el-input v-model="minio.endpoint" placeholder="http(s)://host:port"></el-input>
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
                <el-form-item label="存储桶">
                  <el-input v-model="minio.bucket"></el-input>
                </el-form-item>
              </div>
              <div v-else-if="model.uploadChannel == 30">
                <p>阿里云存储</p>
                <p>暂未开发</p>
              </div>
              <div v-else-if="model.uploadChannel == 40">
                <p>腾讯云存储</p>
                <p>暂未开发</p>
              </div>
              <div v-else-if="model.uploadChannel == 50">
                <p>华为云存储</p>
                <p>暂未开发</p>
              </div>
              <div v-else-if="model.uploadChannel == 60">
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
import { getUploadChannelEnum, getStaticFileSettings, saveStaticFileSettings } from '../../../api/configuration';
import toast from '../../../utils/toast';

const model = reactive({
  uploadChannel: 0,
  loading: false,
  uploadChannelEnum: [],
  localBackup: true,
  loadFileSrc: 0,
  loadFileSrcState: false
});

const github = reactive({
  accessToken: '',
  repositoryUrl: '',
  committerName: '',
  committerEmail: '',
  cdn: ''
});

const minio = reactive({
  endpoint: '',
  accessKey: '',
  secretKey: '',
  ssl: false,
  bucket: ''
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
    let res = await getUploadChannelEnum();
    if (res != null && res.resCode == 0) {
      model.uploadChannelEnum = res.data.list;
    }
  } catch (error) {}
};

const getSettings = async () => {
  try {
    let res = await getStaticFileSettings();
    if (res && res.resCode == 0) {
      model.uploadChannel = res.data.uploadChannel;
      model.localBackup = res.data.localBackup;
      model.loadFileSrc = res.data.loadFileSrc;

      if (model.localBackup == false) {
        model.loadFileSrc = 1;
        model.loadFileSrcState = true;
      }

      monioSetting(res.data?.minio || {});
      githubSeting(res.data?.github || {});
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
  minio.bucket = setting?.bucket || '';
};

const githubSeting = setting => {
  github.accessToken = setting?.accessToken || '';
  github.repositoryUrl = setting?.repositoryUrl || '';
  github.committerName = setting?.committerName || '';
  github.committerEmail = setting?.committerEmail || '';
  github.cdn = setting?.cdn || '';
};

const localBackupChange = value => {
  //
  if (value == false) {
    //
    model.loadFileSrc = 1;
    model.loadFileSrcState = true;
  } else {
    model.loadFileSrcState = false;
  }
};

const submit = async () => {
  model.loading = true;
  try {
    let data = {
      uploadChannel: model.uploadChannel,
      localBackup: model.localBackup,
      loadFileSrc: model.loadFileSrc
    };

    if (data.uploadChannel === 10) {
      data.github = { ...github };
    } else if (data.uploadChannel === 20) {
      data.minio = { ...minio };
    } else if (data.uploadChannel === 30) {
      data.oss = { ...oss };
    } else if (data.uploadChannel === 40) {
      data.cos = { ...cos };
    } else if (data.uploadChannel === 50) {
      data.obs = { ...obs };
    } else if (data.uploadChannel === 60) {
      data.kodo = { ...kodo };
    }

    let res = await saveStaticFileSettings(data);
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
