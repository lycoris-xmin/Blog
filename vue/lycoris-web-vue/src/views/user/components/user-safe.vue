<template>
  <div class="tab-panel">
    <div class="form-group">
      <label>邮箱</label>
      <el-input v-model="model.email" disabled></el-input>
      <el-button @click="model.emailDialogVisible = true">更换邮箱</el-button>
    </div>

    <div class="form-group">
      <label>密码</label>
      <el-input v-model="model.viewPassword" disabled></el-input>
      <el-button @click="model.passwordDialogVisible = true">修改密码</el-button>
    </div>

    <div class="last-form-group">
      <label>帐号注销</label>
      <div class="card">
        <p class="view-info">注销帐号是不可恢复的操作，如需要请自行备份帐号相关的信息和数据。注销帐号后你将丢失该帐号自注册以来产生的数据和记录，注销后相关数据将不可恢复。</p>
        <el-button type="danger" plain>注销</el-button>
      </div>
    </div>

    <el-dialog v-model="model.emailDialogVisible" title="更换邮箱" width="500">
      <p class="dange-info">*邮箱修改成功后，登录帐号也会随之修改为新绑定的邮箱</p>
      <el-form label-position="top" @submit.prevent>
        <el-form-item label="绑定邮箱">
          <el-input v-model="model.changeEmail">
            <template #append>
              <el-button type="primary" plain @click="handleChangeEmailCode" :loading="model.changeEmailCodeLoading" :disabled="model.changeEmailText != '验证码'">{{ model.changeEmailText }}</el-button>
            </template>
          </el-input>
        </el-form-item>
        <el-form-item label="邮箱验证码">
          <el-input v-model="model.changeEmailCode"></el-input>
        </el-form-item>
      </el-form>
      <template #footer>
        <span class="dialog-footer">
          <el-button @click="model.emailDialogVisible = false">取消</el-button>
          <el-button type="primary" @click="changeEmailSumit" :loading="model.changeEmailLoading"> 保存 </el-button>
        </span>
      </template>
    </el-dialog>

    <el-dialog v-model="model.passwordDialogVisible" title="修改密码" width="500">
      <el-form label-position="top" @submit.prevent>
        <el-form-item label="原密码">
          <el-input v-model="model.oldPassword" type="password" show-password></el-input>
        </el-form-item>
        <el-form-item label="新密码">
          <el-input v-model="model.password" type="password" show-password></el-input>
        </el-form-item>
        <el-form-item label="确认密码">
          <el-input v-model="model.confirmPassword" type="password" show-password></el-input>
        </el-form-item>
      </el-form>
      <template #footer>
        <span class="dialog-footer">
          <el-button @click="closePasswordDialog">取消</el-button>
          <el-button type="primary" @click="changePasswordSumit" :loading="model.changePasswordLoading"> 保存 </el-button>
        </span>
      </template>
    </el-dialog>
  </div>
</template>

<script setup>
import { onMounted, reactive } from 'vue';
import { stores } from '../../../stores';
import { changeEmailCode, changeEmail, changePassword } from '../../../api/authentication';
import { passwordRegex, emailRegex } from '../../../utils/regex';
import toast from '../../../utils/toast';
import swal from '../../../utils/swal';
import { useRouter } from 'vue-router';
import { getEmailHost, openQQMail } from '../../../utils/email-tool';

const router = useRouter();

const model = reactive({
  email: '',
  emailDialogVisible: false,
  changeEmail: '',
  changeEmailCodeLoading: false,
  changeEmailCode: '',
  changeEmailText: '验证码',
  changeEmailLoading: false,
  viewPassword: '************',
  passwordDialogVisible: false,
  oldPassword: '',
  password: '',
  confirmPassword: '',
  changePasswordLoading: false
});

onMounted(() => {
  model.email = stores.user.email;
});

const checkEmailInput = email => {
  if (email == '') {
    toast.warn('绑定邮箱不能为空');
    return false;
  } else if (!emailRegex(email)) {
    toast.warn('绑定邮箱格式错误');
    return false;
  }

  return true;
};

const handleChangeEmailCode = async () => {
  //
  if (!checkEmailInput(model.email)) {
    return;
  }
  model.changeEmail = model.changeEmail.trim();
  model.changeEmailCodeLoading = true;
  try {
    let res = await changeEmailCode(model.changeEmail);
    if (res && res.resCode == 0) {
      let i = 59;
      const codeTime = setInterval(() => {
        if (i == 0) {
          clearInterval(codeTime);
          model.changeEmailText = '验证码';
          return;
        }
        model.changeEmailText = `${i}秒重新发送`;
        i--;
      }, 1000);

      await swal.success(`验证已发送至邮箱: ${model.changeEmail}`);
      const emailHost = getEmailHost(model.email);
      if (emailHost.includes('https://mail.qq.com')) {
        openQQMail();
      } else {
        window.open(emailHost);
      }
    }
  } catch (err) {
    console.log(err);
  } finally {
    model.changeEmailCodeLoading = false;
  }
};

const changeEmailSumit = async () => {
  if (!checkEmailInput(model.email)) {
    return;
  }

  model.changeEmailCode = model.changeEmailCode.trim();
  if (model.changeEmailCode.length != 6) {
    toast.warn('验证码格式错误');
    return;
  }

  model.changeEmailLoading = true;

  try {
    let res = await changeEmail({ email: model.changeEmail, captcha: model.changeEmailCode });
    if (res && res.resCode == 0) {
      await swal.success('绑定邮箱修改成功,需要使用新邮箱重新登录', '绑定成功通知');
      stores.authorize.setUserLogoutState();
      stores.user.setLogoutState();
      router.push({ name: 'home' });
    }
  } catch (err) {
    console.log(err);
  } finally {
    model.changeEmailLoading = false;
  }

  model.emailDialogVisible = false;
};

const closePasswordDialog = () => {
  model.oldPassword = '';
  model.password = '';
  model.confirmPassword = '';
  model.passwordDialogVisible = false;
};

const changePasswordSumit = async () => {
  //
  if (!model.oldPassword) {
    toast.warn('原密码不能为空');
    return;
  } else if (!passwordRegex(model.oldPassword)) {
    toast.warn('原密码错误');
    return;
  } else if (!model.password) {
    toast.warn('新密码不能为空');
    return;
  } else if (!passwordRegex(model.password)) {
    toast.warn('密码必须包含大写字母，小写字母，数字，特殊符号 `@#$%^&*`~()-+=` 中任意3项密码');
    return;
  } else if (!model.confirmPassword) {
    toast.warn('确认密码不能为空');
    return;
  } else if (model.password != model.confirmPassword) {
    toast.warn('两次输入的密码不一致');
    return;
  }

  model.changePasswordLoading = true;
  try {
    let res = await changePassword({
      oldPassword: model.oldPassword,
      password: model.password
    });

    if (res && res.resCode == 0) {
      toast.success('修改密码成功');
      stores.authorize.setUserLoginState(res.data);
      closePasswordDialog();
    }
  } finally {
    model.changePasswordLoading = false;
  }
};
</script>

<style lang="scss" scoped>
.form-group {
  width: 400px !important;

  .el-button {
    margin: 0 5px;
  }
}

p.dange-info {
  color: var(--color-danger);
  padding-bottom: 10px;
}

.last-form-group {
  margin-top: 28px;

  > label {
    font-size: 16px;
    font-weight: 600;
  }

  .card {
    margin-top: 10px;
    border: 1px solid var(--color-secondary);
    border-radius: 5px;
    display: flex;
    justify-content: space-between;
    align-items: center;
    gap: 10px;

    .view-info {
      line-height: 30px;
      letter-spacing: 1.5px;
    }
  }
}
</style>
