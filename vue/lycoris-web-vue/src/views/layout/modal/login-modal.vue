<template>
  <el-dialog v-model="model.visible" width="400px" :lock-scroll="true" class="el-layout-modal" :before-close="beforeClose">
    <div class="login-body">
      <div class="title">
        <span>{{ model.title }}</span>
      </div>
      <form>
        <TransitionGroup name="flipIn" tag="div">
          <div class="login-form" v-if="model.type == 0" :key="'user'">
            <div class="input" :class="{ focus: computedEmail }">
              <input type="text" v-model="form.email" class="input-el" @focus="model.focus.email = true" @blur="model.focus.email = false" autocomplete="off" />
              <span data-placeholder="邮箱"></span>
            </div>
            <div class="input" :class="{ focus: computedPassword }">
              <input type="password" v-model="form.password" class="input-el" @focus="model.focus.password = true" @blur="model.focus.password = false" autocomplete="off" />
              <span data-placeholder="密码"></span>
            </div>
            <div class="other">
              <span @click="changeType(1)">立即注册</span>
              <span @click="changeType(2)">忘记密码</span>
            </div>
            <div class="login-footer flex-center-center">
              <el-button class="login-btn" @click="userLogin" :disabled="model.disabled">登录</el-button>
            </div>
          </div>
          <div class="login-form" v-else-if="model.type == 1" :key="'register'">
            <el-form label-width="80px" label-position="top">
              <el-form-item>
                <el-input v-model="registerForm.email" placeholder="邮箱"></el-input>
              </el-form-item>
              <el-form-item>
                <el-input v-model="registerForm.captcha" placeholder="验证码">
                  <template #append>
                    <el-button @click="emailCaptchaCode($event, 1)" :disabled="registerForm.emailCaptchaTime">验证码</el-button>
                  </template>
                </el-input>
              </el-form-item>
              <el-form-item>
                <el-input v-model="registerForm.password" type="password" show-password placeholder="密码"></el-input>
              </el-form-item>
              <el-form-item>
                <el-input v-model="registerForm.confirmPassword" type="password" show-password placeholder="密码验证"></el-input>
              </el-form-item>
              <div style="text-align: center; padding: 10px 0">
                <el-button class="btn" type="primary" @click="userRegister" :loading="model.btnLoading">立即注册</el-button>
              </div>
            </el-form>
          </div>
          <div class="login-form" v-else :key="'forget'">
            <el-form label-width="80px" label-position="top">
              <el-form-item>
                <el-input v-model="forgetForm.email" placeholder="邮箱"></el-input>
              </el-form-item>
              <el-form-item>
                <el-input v-model="forgetForm.captcha" placeholder="验证码">
                  <template #append>
                    <el-button type="info" @click="emailCaptchaCode($event, 2)" :disabled="forgetForm.emailCaptchaTime">验证码</el-button>
                  </template>
                </el-input>
              </el-form-item>
              <el-form-item>
                <el-input v-model="forgetForm.password" type="password" show-password placeholder="密码"></el-input>
              </el-form-item>
              <el-form-item>
                <el-input v-model="forgetForm.confirmPassword" type="password" show-password placeholder="密码验证"></el-input>
              </el-form-item>
              <div style="text-align: center; padding: 10px 0">
                <el-button class="btn reset-password-btn" :loading="model.btnLoading">重置密码</el-button>
              </div>
            </el-form>
          </div>
        </TransitionGroup>
      </form>
      <div class="other-login-group flex-center-center">
        <div class="platform flex-center-center" v-show="model.type != 0" @click="changeType(0)">
          <el-icon :size="24">
            <component :is="'user-filled'"></component>
          </el-icon>
        </div>
        <div class="platform"><img src="/icon/platform/qq.png" /></div>
        <div class="platform"><img src="/icon/platform/wechat.png" /></div>
      </div>
    </div>
  </el-dialog>
</template>

<script setup>
import { reactive, computed } from 'vue';
import { loginValidate, login, registerCaptcha, register } from '../../../api/authentication';
import { stores } from '../../../stores';
import { passwordRegex, emailRegex } from '../../../utils/regex';
import { debounce } from '../../../utils/tool';
import toast from '../../../utils/toast';
import swal from '../../../utils/swal';

const model = reactive({
  visible: false,
  focus: {
    email: false,
    password: false
  },
  disabled: false,
  title: '用户登录',
  type: 0,
  btnLoading: false
});

const form = reactive({
  email: '',
  password: ''
});

const registerForm = reactive({
  email: '',
  captcha: '',
  password: '',
  confirmPassword: '',
  emailCaptchaTime: false
});

const forgetForm = reactive({
  email: '',
  captcha: '',
  password: '',
  confirmPassword: '',
  emailCaptchaTime: false
});

const props = defineProps({
  owner: {
    type: Object,
    required: true,
    default: void 0
  }
});

const computedEmail = computed(() => {
  return model.focus.email || form.email.length;
});

const computedPassword = computed(() => {
  return model.focus.password || form.password.length;
});

const emit = defineEmits(['refreshUserBrief']);

const changeType = type => {
  model.type = type;

  if (model.type == 0) {
    model.title = '用户登录';
  } else if (model.type == 1) {
    model.title = '用户注册';
  } else if (model.type == 2) {
    model.title = '忘记密码';
  }
};

const emailCaptchaCode = async (e, actionType) => {
  try {
    let res = await registerCaptcha({
      email: actionType == 1 ? registerForm.email : forgetForm.email,
      actionType: actionType
    });

    if (res && res.resCode == 0) {
      emailCaptchaTime(e, actionType);

      if (actionType == 2) {
        toast.success(`如果收不到邮箱验证码，请使用你注册的邮箱发送相关信息至站长邮箱：${props.owner.email}，核实完身份，将手动为您重置密码`);
      } else {
        const emailHost = getEmailHost(registerForm.email);
        if (emailHost) {
          await swal.success(`如果收不到邮箱验证码，请使用你想注册的邮箱发送相关信息至站长邮箱：${props.owner.email}，并注明注册信息`, '发送成功');
          if (emailHost.includes('https://mail.qq.com')) {
            openQQMail();
          } else {
            window.open(emailHost);
          }
        } else {
          toast.success(`如果收不到邮箱验证码，请使用你想注册的邮箱发送相关信息至站长邮箱：${props.owner.email}，并注明注册信息`);
        }
      }
    }
  } catch (error) {
    if (error.data.resCode == -99 && error.data.data) {
      emailCaptchaTime(e, actionType, parseInt(error.data.data));
    }
  }
};

const emailCaptchaTime = (e, type, time = 59) => {
  if (isNaN(time)) {
    return;
  }

  if (time <= 0) {
    return;
  }

  if (type == 1) {
    registerForm.emailCaptchaTime = true;
  } else {
    forgetForm.emailCaptchaTime = true;
  }

  let captchaTime = setInterval(() => {
    if (time == 0) {
      e.target.innerText = '验证码';
      if (type == 0) {
        registerForm.emailCaptchaTime = false;
      } else {
        forgetForm.emailCaptchaTime = false;
      }
      clearInterval(captchaTime);
      return;
    } else {
      e.target.innerText = `${time}秒`;
    }

    time--;
  }, 1000);
};

const show = () => {
  model.visible = true;
};

const close = () => {
  beforeClose(() => {
    model.visible = false;
  });
};

const beforeClose = done => {
  done();

  changeType(0);
  form.password = '';
  registerForm.captcha = '';
  registerForm.password = '';
  registerForm.confirmPassword = '';
  forgetForm.captcha = '';
  forgetForm.password = '';
  forgetForm.confirmPassword = '';
};

const userLogin = debounce(async () => {
  if (!form.email && !form.password) {
    return;
  }

  if (form.email == '') {
    toast.warn('邮箱不能为空');
    return;
  } else if (!emailRegex(form.email)) {
    toast.warn('邮箱格式错误');
    return;
  }

  if (form.password == '') {
    toast.warn('密码不能为空');
    return;
  } else if (!passwordRegex(form.password)) {
    toast.warn('帐号或密码错误');
    return;
  }

  model.disabled = true;

  let res = await loginValidate({
    ...form
  }).catch(err => {
    model.disabled = false;
    throw err;
  });

  if (res && res.resCode == 0) {
    let data = {
      email: form.email,
      oathCode: res.data.oathCode,
      remember: true
    };

    res = await login(data).catch(err => {
      model.disabled = false;
      throw err;
    });

    if (res && res.resCode == 0) {
      stores.authorize.setUserLoginState(res.data);

      toast.success('登录成功');
      close();
      //
      emit('refreshUserBrief');
    }
  }
}, 300);

const userRegister = debounce(async () => {
  if (!registerForm.email && !registerForm.captcha && !registerForm.password) {
    return;
  }

  if (registerForm.email == '') {
    toast.warn('邮箱不能为空');
    return;
  } else if (!emailRegex(registerForm.email)) {
    toast.warn('邮箱格式错误');
    return;
  }

  if (registerForm.password == '') {
    toast.warn('密码不能为空');
    return;
  } else if (!passwordRegex(registerForm.password)) {
    toast.warn('帐号或密码错误');
    return;
  }

  if (registerForm.password != registerForm.confirmPassword) {
    toast.warn('密码不能为空');
    return;
  }

  model.btnLoading = true;
  try {
    let res = await register({
      ...registerForm
    });
    if (res && res.resCode == 0) {
      toast.success('注册成功，感谢您愿意成为本站的一员');
      changeType(0);
    }
  } finally {
    model.btnLoading = false;
  }
}, 300);

const getEmailHost = email => {
  //
  if (email.endsWith('@163.com') || email.endsWith('@qq.com')) {
    var arr1 = email.split('@');
    var arr2 = arr1[1].split('.');
    return `https://mail.${arr2[0]}.com`;
  } else if (email.endsWith('@outlook.com')) {
    return `https://www.outlook.com`;
  }

  return '';
};

function openQQMail() {
  var form = document.createElement('form');
  form.method = 'POST';
  form.action = 'https://mail.qq.com';
  form.style.display = 'none';
  form.target = '_blank';

  document.body.appendChild(form);
  form.submit();
  form.remove();
}

defineExpose({
  show,
  close
});
</script>

<style lang="scss" scoped>
.login-body {
  padding: 0 10px;

  .title {
    padding-bottom: 10px;
    width: 100%;
    text-align: center;

    span {
      font-size: 18px;
    }
  }

  form:nth-child(2) {
    height: 270px;
    overflow: hidden;
  }

  .login-form {
    .input {
      border-bottom: 2px solid #adadad;
      position: relative;
      margin: 30px 0;

      .input-el {
        font-size: 15px;
        color: #333;
        border: none;
        width: 100%;
        outline: none;
        background: none;
        padding: 0 5px;
        height: 40px;
      }

      .input-el:-internal-autofill-selected {
        -webkit-appearance: menulist-button;
        appearance: menulist-button;
        background-image: none !important;
        background-color: transparent !important;
        color: fieldtext !important;
      }

      span {
        position: static;
      }

      span::before {
        content: attr(data-placeholder);
        position: absolute;
        top: 50%;
        left: 5px;
        color: var(--color-dark);
        transform: translateY(-50%);
        z-index: 0;
        transition: 0.5s;
      }

      span::after {
        content: '';
        position: absolute;
        left: 0%;
        top: 100%;
        width: 0%;
        height: 2px;
        background: linear-gradient(120deg, #3498db, #8e44ad);
        transition: 0.5s;
      }
    }

    .focus {
      span::before {
        top: -8px;
      }

      span::after {
        width: 100%;
      }
    }

    .remember-switch {
      font-size: 15px;
      cursor: pointer;

      .remember-text {
        margin-left: 10px;
      }
    }

    .login-footer {
      padding-top: 20px;
      margin-bottom: 15px;

      .login-btn {
        display: block;
        width: 60%;
        height: 40px;
        border: none;
        background: linear-gradient(120deg, #3498db, #8e44ad, #3498db);
        background-size: 200%;
        color: #fff;
        transition: 0.5s;
        border-radius: 8px;
      }

      .login-btn:hover {
        background-position: right;
      }
    }

    .other {
      display: flex;
      justify-content: space-between;
      align-items: center;

      span {
        cursor: pointer;
        color: var(--color-dark);
        transition: all 0.4s;
      }

      span:hover {
        color: var(--color-info);
      }
    }

    .btn {
      width: 140px;
      height: 35px;
    }

    .reset-password-btn {
      background-color: var(--color-purple);
      border-color: var(--color-purple-light);
      transition: all 0.4s;

      :deep(span) {
        color: #fff;
      }
    }

    .reset-password-btn:hover {
      background-color: var(--color-purple-light);
      border-color: var(--color-purple-light);
    }
  }

  .other-login-group {
    padding-top: 10px;

    .platform {
      margin: 0 5px;
      height: 40px;
      width: 40px;
      cursor: pointer;
      transform: scale(1);
      transition: all 0.5s;

      img {
        height: 40px;
        width: 40px;
        object-fit: fill;
      }
    }

    .platform:hover {
      transform: scale(1.2);
      background-color: var(--color-secondary-light);
      border-radius: 25px;
    }
  }

  .el-input-group__append {
    .el-button {
      width: 80px;
      border-color: var(--color-secondary);
      border-left-color: transparent;
      transition: all 0.4s;

      :deep(span) {
        transition: all 0.4s;
      }
    }

    .el-button:hover {
      background-color: var(--color-info);
      border-left-color: transparent;

      :deep(span) {
        color: #fff;
      }
    }
  }
}

.flipIn-enter-active {
  animation: flipInY 0.5s;
}

.flipIn-leave-active {
  animation: flipOutY 0.3s;
}
</style>
