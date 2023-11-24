<template>
  <div style="padding-top: 18px">
    <el-form label-position="left" :model="model" :label-width="110">
      <el-row :gutter="24">
        <el-col :span="8">
          <el-form-item label="前台域名">
            <el-input v-model="model.webPath"></el-input>
          </el-form-item>
          <el-form-item label="后台域名">
            <el-input v-model="model.adminPath"></el-input>
          </el-form-item>
          <el-form-item label="网站名称">
            <el-input v-model="model.webName"></el-input>
          </el-form-item>
          <el-form-item label="网站Logo" class="setting-avatar">
            <el-upload class="avatar-uploader" :accept="uploadAccept.imgAccept" :show-file-list="false" :on-change="raw => handleChange('logo', raw)" :before-remove="() => handleRemove('logo')" :auto-upload="false">
              <div v-if="model.logoDisplay" class="uploader-container">
                <img :src="model.logoDisplay" class="avatar" />
                <div class="overlay">
                  <el-icon @click.stop="deleterLogo">
                    <component :is="'delete'"></component>
                  </el-icon>
                </div>
              </div>
              <el-icon v-else class="avatar-uploader-icon">
                <component :is="'plus'"></component>
              </el-icon>
            </el-upload>
          </el-form-item>
          <el-form-item label="网站备案号">
            <el-input v-model="model.icp"></el-input>
          </el-form-item>
          <el-form-item label="网站描述">
            <el-input v-model="model.description" type="textarea" :autosize="{ minRows: 4 }" maxlength="300" show-word-limit></el-input>
          </el-form-item>
          <el-form-item label="网站建立时间">
            <el-date-picker class="date" v-model="model.buildTime" type="date" placeholder="网站建立时间" format="YYYY-MM-DD" />
          </el-form-item>
          <el-form-item label="用户默认头像" class="setting-avatar">
            <el-upload class="avatar-uploader" :accept="uploadAccept.imgAccept" :show-file-list="false" :on-change="raw => handleChange('avatar', raw)" :before-remove="() => handleRemove('avatar')" :auto-upload="false">
              <img v-if="model.avatarDisplay" :src="model.avatarDisplay" class="avatar" />
              <el-icon v-else class="avatar-uploader-icon">
                <component :is="'plus'"></component>
              </el-icon>
            </el-upload>
          </el-form-item>
          <div class="submit">
            <el-button type="primary" :loading="model.loading" @click="submit">保存</el-button>
          </div>
        </el-col>
      </el-row>
    </el-form>
  </div>
</template>

<script setup>
import { onMounted, reactive } from 'vue';
import { getWebSettings, saveWebSettings } from '../../../api/configuration';
import toast from '../../../utils/toast';
import { uploadAccept } from '../../../config.json';

const props = defineProps({
  value: {
    type: Number,
    required: true
  }
});

const model = reactive({
  webName: '',
  webPath: '',
  adminPath: '',
  logo: void 0,
  logoDisplay: '',
  icp: '',
  description: '',
  buildTime: '',
  avatar: void 0,
  avatarDisplay: '',
  loading: false
});

const emit = defineEmits(['tabComplete']);

onMounted(async () => {
  try {
    let res = await getWebSettings();
    if (res.resCode == 0) {
      setValue(res.data);
    }
  } finally {
    emit('tabComplete', props.value);
  }
});

const handleChange = (type, rawFile) => {
  if (type == 'logo') {
    model.logo = rawFile.raw;
    model.logoDisplay = URL.createObjectURL(rawFile.raw);
  } else {
    model.avatar = rawFile.raw;
    model.avatarDisplay = URL.createObjectURL(rawFile.raw);
  }
};

const handleRemove = type => {
  if (type == 'logo') {
    model.logo = void 0;
  } else {
    model.avatar = void 0;
  }
  return true;
};

const submit = async () => {
  model.loading = true;

  try {
    let res = await saveWebSettings({ ...model });
    if (res.resCode == 0) {
      toast.success('保存成功');
      model.avatar = void 0;
      model.avatarDisplay = res.data.defaultAvatar || '';
    }
  } finally {
    model.loading = false;
  }
};

const setValue = data => {
  model.webName = data.webName;
  model.webPath = data.webPath;
  model.adminPath = data.adminPath;
  model.logoDisplay = data.logo || '/logo/logo-lycoirs.png';
  model.icp = data.icp;
  model.description = data.description;
  model.buildTime = data.buildTime || '';
  model.avatarDisplay = data.defaultAvatar || '';
};

const deleterLogo = () => {
  model.logoDisplay = '';
  model.logo = void 0;
};
</script>

<style lang="scss" scoped>
.submit {
  text-align: left;

  .el-button {
    width: 120px;
  }
}

.uploader-container {
  position: relative;

  .overlay {
    position: absolute;
    top: 0;
    left: 0;
    right: 0;
    bottom: 0;
    display: none;
    justify-content: center;
    align-items: center;
    background-color: #00000061;
    cursor: default;

    .el-icon {
      font-size: 24px;
      color: var(--color-danger);
      cursor: pointer;
    }
  }

  &:hover {
    .overlay {
      display: flex;
    }
  }
}
</style>

<style lang="scss">
.setting-avatar {
  $avatar-size: 100px;

  .avatar-uploader {
    width: $avatar-size;
    height: $avatar-size;

    .avatar {
      width: calc($avatar-size - 1px);
      height: calc($avatar-size - 1px);
      display: block;
    }

    .overlay {
      width: calc($avatar-size - 1px);
      height: calc($avatar-size - 1px);
    }

    .el-upload {
      border: 1px dashed var(--el-border-color);
      border-radius: 6px;
      cursor: pointer;
      position: relative;
      overflow: hidden;
      transition: var(--el-transition-duration-fast);

      &:hover {
        border-color: var(--el-color-primary);
      }
    }
  }

  .el-icon.avatar-uploader-icon {
    font-size: 28px;
    color: #8c939d;
    width: $avatar-size;
    height: $avatar-size;
    text-align: center;
  }
}
</style>
