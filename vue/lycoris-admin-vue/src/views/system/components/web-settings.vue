<template>
  <div style="padding-top: 18px">
    <el-form label-position="left" :model="model" :label-width="110">
      <el-row :gutter="24">
        <el-col :span="8">
          <el-form-item label="网站名称">
            <el-input v-model="model.webName"></el-input>
          </el-form-item>
          <el-form-item label="网站前台地址">
            <el-input v-model="model.webPath"></el-input>
          </el-form-item>
          <el-form-item label="后台登录地址">
            <el-input v-model="model.adminPath"></el-input>
          </el-form-item>
          <el-form-item label="网站建立时间">
            <el-date-picker class="date" v-model="model.buildTime" type="date" placeholder="网站建立时间" format="YYYY-MM-DD" />
          </el-form-item>
          <el-form-item label="用户默认头像" class="setting-avatar">
            <el-upload class="avatar-uploader" :accept="uploadAccept.imgAccept" :show-file-list="false" :on-change="handleChange" :before-remove="handleRemove" :auto-upload="false">
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

const handleChange = rawFile => {
  model.avatar = rawFile.raw;
  model.avatarDisplay = URL.createObjectURL(model.avatar);
};

const handleRemove = () => {
  model.avatar = void 0;
  return true;
};

const submit = async () => {
  model.loading = true;

  try {
    let res = await saveWebSettings({ ...model });
    if (res.resCode == 0) {
      toast.success('保存成功');
      model.avatar = void 0;
      setValue(res.data);
    }
  } finally {
    model.loading = false;
  }
};

const setValue = data => {
  model.webName = data.webName;
  model.webPath = data.webPath;
  model.adminPath = data.adminPath;
  model.buildTime = data.buildTime || '';
  model.avatarDisplay = data.defaultAvatar || '';
};
</script>

<style lang="scss" scoped>
.submit {
  text-align: left;

  .el-button {
    width: 120px;
  }
}
</style>

<style lang="scss">
.setting-avatar {
  $avatar-size: 100px;

  .avatar-uploader {
    .avatar {
      width: $avatar-size;
      height: $avatar-size;
      display: block;
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
