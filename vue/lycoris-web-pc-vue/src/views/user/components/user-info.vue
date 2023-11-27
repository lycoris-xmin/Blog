<template>
  <div class="tab-panel">
    <div class="form-group avatar">
      <el-upload :accept="uploadAccept.imgAccept" :show-file-list="false" :on-change="handleChange" :before-remove="handleRemove" :auto-upload="false">
        <el-image :src="model.avatar" lazy>
          <template #placeholder>
            <div class="image-slot" style="font-size: 14px">Loading<span class="dot">...</span></div>
          </template>

          <template #error>
            <div class="image-slot">
              <el-icon>
                <component :is="'picture'"></component>
              </el-icon>
            </div>
          </template>
        </el-image>
      </el-upload>
    </div>
    <div class="form-group">
      <label>昵称</label>
      <el-input v-model="model.nickName"></el-input>
    </div>
    <div class="form-group">
      <label>个人博客</label>
      <el-input v-model="model.blog"></el-input>
    </div>
    <div class="form-group">
      <label>QQ</label>
      <el-input v-model="model.qq"></el-input>
    </div>
    <div class="form-group">
      <label>微信</label>
      <el-input v-model="model.wechat"></el-input>
    </div>
    <div class="form-group">
      <label>Github</label>
      <el-input v-model="model.github"></el-input>
    </div>
    <div class="form-group">
      <label>码云</label>
      <el-input v-model="model.gitee"></el-input>
    </div>
    <div class="form-group">
      <label>哔哩哔哩</label>
      <el-input v-model="model.bilibili"></el-input>
    </div>
    <div class="form-group">
      <label>网易云音乐</label>
      <el-input v-model="model.cloudMusic"></el-input>
    </div>
    <div class="form-group sumit-btn">
      <el-button type="primary" :loading="model.loading" @click="sumit">保存</el-button>
    </div>
  </div>
</template>

<script setup>
import { onMounted, reactive } from 'vue';
import { stores } from '@/stores';
import { uploadAccept } from '@/config.json';
import { updateUserBrief } from '@/api/user';
import toast from '@/utils/toast';

const model = reactive({
  nickName: '',
  avatar: '',
  blog: '',
  qq: '',
  wechat: '',
  github: '',
  gitee: '',
  bilibili: '',
  cloudMusic: '',
  file: void 0,
  loading: false
});

onMounted(() => {
  let value = stores.user;

  model.nickName = value.nickName;
  model.avatar = value.avatar;
  model.blog = value.blog;
  model.qq = value.qq;
  model.wechat = value.wechat;
  model.github = value.github;
  model.gitee = value.gitee;
  model.bilibili = value.bilibili;
  model.cloudMusic = value.cloudMusic;
});

const handleChange = rawFile => {
  model.file = rawFile.raw;
  model.avatar = URL.createObjectURL(model.file);
};

const handleRemove = () => {
  model.avatar = void 0;
  return true;
};

const sumit = async () => {
  //
  model.loading = true;
  try {
    let data = { ...model };
    let res = await updateUserBrief(data);
    if (res && res.resCode == 0) {
      toast.success('保存成功');
      stores.user.updateUser(res.data);
    }
  } finally {
    model.loading = false;
  }
};
</script>

<style lang="scss" scoped>
.form-group {
  width: 500px !important;

  :deep(.el-image) {
    min-height: 100px;
    min-width: 100px;
    border-radius: 50%;
    border: 2px solid var(--color-secondary);

    .image-slot {
      display: flex;
      justify-content: center;
      align-items: center;
      width: 100%;
      height: 100%;
      background: var(--el-fill-color-light);
      color: var(--el-text-color-secondary);

      .dot {
        animation: dot 2s infinite steps(3, start);
        overflow: hidden;
      }

      .el-icon {
        font-size: 30px;
      }
    }

    img {
      height: 100px;
      width: 100px;
      border-radius: 50%;
    }
  }

  &.avatar {
    justify-content: center;
  }
}
</style>
