<template>
  <div class="tab-panel">
    <div class="form-group">
      <label>邮箱</label>
      <el-input v-model="model.email" disabled></el-input>
      <el-button @click="showEmailDialog">更换邮箱</el-button>
    </div>

    <div class="form-group">
      <label>密码</label>
      <el-input v-model="model.viewPassword" disabled></el-input>
      <el-button @click="showPasswordDialog">修改密码</el-button>
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
      <el-form label-position="top">
        <el-form-item label="绑定邮箱">
          <el-input v-model="model.changeEmail">
            <template #append>
              <el-button type="primary" plain @click="handleChangeEmailCode" :disabled="model.changeEmailText != '验证码'">{{ model.changeEmailText }}</el-button>
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
          <el-button type="primary" @click="changeEmailSumit"> 保存 </el-button>
        </span>
      </template>
    </el-dialog>

    <el-dialog v-model="model.passwordDialogVisible" title="修改密码" width="500">
      <el-form label-position="top">
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
          <el-button @click="model.passwordDialogVisible = false">取消</el-button>
          <el-button type="primary" @click="changeEmailSumit"> 保存 </el-button>
        </span>
      </template>
    </el-dialog>
  </div>
</template>

<script setup>
import { onMounted, reactive } from 'vue';
import { stores } from '../../../stores';

const model = reactive({
  email: '',
  emailDialogVisible: false,
  changeEmail: '',
  changeEmailCode: '',
  changeEmailText: '验证码',
  viewPassword: '************',
  passwordDialogVisible: false,
  oldPassword: '',
  password: '',
  confirmPassword: ''
});

onMounted(() => {
  model.email = stores.user.email;
});

const showEmailDialog = () => {
  //
  model.emailDialogVisible = true;
};

const showPasswordDialog = () => {
  model.oldPassword = '';
  model.password = '';
  model.confirmPassword = '';
  model.passwordDialogVisible = true;
};

const handleChangeEmailCode = () => {
  //

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
};

const changeEmailSumit = () => {
  model.emailDialogVisible = false;
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
