<template>
  <el-dialog v-model="model.dialogVisible" :title="`个人资料（${stores.owner.email}）`" width="820px" :close-on-click-modal="false">
    <el-form label-position="top" label-width="60" ref="formRef" :model="form" :rules="formRules" @submit.prevent>
      <el-row :gutter="24">
        <el-col :span="24">
          <el-tooltip effect="dark" content="点击更换头像" placement="top">
            <el-form-item class="profile-avatar">
              <input type="file" accept="image/*" ref="uploadAvatar" hidden @change="fileChange" />
              <el-avatar class="profile-avatar-img" :size="100" :src="form.avatar" v-if="form.avatar" @click="upload"></el-avatar>
              <el-avatar :size="100" :icon="UserFilled" v-else @click="upload" />
            </el-form-item>
          </el-tooltip>
        </el-col>

        <el-col :span="12">
          <el-tooltip effect="dark" content="邮箱作为登录帐号，无法修改" placement="bottom">
            <el-form-item label="邮箱" prop="email">
              <el-input :value="form.email" disabled />
            </el-form-item>
          </el-tooltip>
        </el-col>

        <el-col :span="12">
          <el-form-item label="昵称" prop="nickName">
            <el-input v-model="form.nickName" autocomplete="off" />
          </el-form-item>
        </el-col>

        <el-col :span="12">
          <el-form-item label="QQ" prop="qq">
            <el-input v-model="form.qq" autocomplete="off" placeholder="会在前台页面进行展示，若不想展示，请不要输入" />
          </el-form-item>
        </el-col>

        <el-col :span="12">
          <el-form-item label="微信" prop="wechat">
            <el-input v-model="form.wechat" autocomplete="off" placeholder="会在前台页面进行展示，若不想展示，请不要输入" />
          </el-form-item>
        </el-col>

        <el-col :span="12">
          <el-form-item label="GitHub" prop="gitHub">
            <el-input v-model="form.gitHub" autocomplete="off" placeholder="请输入github个人主页地址" />
          </el-form-item>
        </el-col>

        <el-col :span="12">
          <el-form-item label="Gitee" prop="gitee">
            <el-input v-model="form.gitee" autocomplete="off" placeholder="请输入码云个人主页地址" />
          </el-form-item>
        </el-col>

        <el-col :span="12">
          <el-form-item label="哔哩哔哩" prop="gitHub">
            <el-input v-model="form.bilibili" autocomplete="off" placeholder="请输入哔哩哔哩个人主页地址" />
          </el-form-item>
        </el-col>

        <el-col :span="12">
          <el-form-item label="网易云音乐" prop="gitHub">
            <el-input v-model="form.cloudMusic" autocomplete="off" placeholder="请输入网易云音乐个人主页地址" />
          </el-form-item>
        </el-col>
      </el-row>
    </el-form>
    <template #footer>
      <span class="dialog-footer">
        <el-button @click="close">取消</el-button>
        <el-button type="primary" @click="submit" :loading="model.loading">保存</el-button>
      </span>
    </template>
  </el-dialog>
</template>

<script setup>
import { reactive, ref } from 'vue';
import { UserFilled } from '@element-plus/icons-vue';
import { stores } from '../../../stores';
import { updateUserBrief } from '../../../api/user';
import { uploadStaticFile } from '../../../api/staticFile';
import UploadType from '../../../constants/UploadType';
import toast from '../../../utils/toast';

const formRef = ref();
const uploadAvatar = ref();

const model = reactive({
  dialogVisible: false,
  loading: false
});

const form = reactive({
  nickName: '',
  avatar: '',
  email: '',
  qq: '',
  wechat: '',
  gitHub: '',
  gitee: '',
  bilibili: '',
  cloudMusic: '',
  file: ''
});

const formRules = reactive({});

const upload = () => {
  uploadAvatar.value.click();
};

const fileChange = e => {
  if (e.target && e.target.files.length) {
    form.file = e.target.files[0];
    form.avatar = URL.createObjectURL(form.file);
  }
};

const submit = async () => {
  model.loading = true;
  try {
    if (form.file) {
      let fileRes = await uploadStaticFile(UploadType.AVATAR, form.file);
      if (!fileRes || fileRes.resCode != 0) {
        return;
      }

      form.avatar = fileRes.data;
    }

    let res = await updateUserBrief({ ...form });
    if (res && res.resCode == 0) {
      stores.owner.setData({ ...form });
      toast.success('修改成功');
      model.dialogVisible = false;
    }
  } catch (err) {
    console.log(err);
    toast.success('修改失败');
  } finally {
    model.loading = false;
  }
};

const show = () => {
  form.nickName = stores.owner.nickName || '';
  form.avatar = stores.owner.avatar || '';
  form.email = stores.owner.email || '';
  form.qq = stores.owner.qq || '';
  form.wechat = stores.owner.wechat || '';
  form.gitHub = stores.owner.gitHub || '';
  form.cloudMusic = stores.owner.cloudMusic || '';
  form.bilibili = stores.owner.bilibili || '';

  model.dialogVisible = true;
};

const close = () => {
  model.dialogVisible = false;
  setTimeout(() => {
    formRef.value.resetFields();
  }, 500);
};

defineExpose({
  show,
  close
});
</script>

<style lang="scss" scoped>
:deep(.el-form-item) {
  margin-bottom: 25px;
}

:deep(.el-form-item:last-child) {
  margin-bottom: 20px;
}

.profile-avatar {
  :deep(.el-form-item__content) {
    display: flex;
    justify-content: center;
    align-items: center;
    margin: 0 !important;
  }

  .profile-avatar-img {
    cursor: pointer;
  }

  :deep(.el-avatar) {
    cursor: pointer;
  }

  :deep(.el-icon) {
    height: 80px !important;
    width: 80px !important;

    svg {
      height: 55px !important;
      width: 55px !important;
    }
  }
}
</style>
