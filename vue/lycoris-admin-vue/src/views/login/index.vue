<template>
  <div class="bg">
    <sky-area></sky-area>

    <div class="login-container">
      <h1><b>用户登录</b></h1>
      <form>
        <div class="input" :class="{ focus: computedEmail }">
          <input type="text" v-model="model.form.email" class="input-el" @focus="model.focus.email = true" @blur="model.focus.email = false" autocomplete="off" />
          <span data-placeholder="邮箱"></span>
        </div>
        <div class="input" :class="{ focus: computedPassword }">
          <input type="password" v-model="model.form.password" class="input-el" @focus="model.focus.password = true" @blur="model.focus.password = false" autocomplete="off" />
          <span data-placeholder="密码"></span>
        </div>
        <div class="remember-switch">
          <el-switch v-model="model.form.remember" />
          <span class="remember-text">记住我</span>
        </div>
        <div class="login-footer">
          <button class="login-btn" type="button" @click="userLogin" :disabled="model.disabled">登录</button>
        </div>
        <a class="footer flex-center-center" href="https://beian.miit.gov.cn/" target="_blank"><img src="/icon/gongan.png" alt="" />{{ stores.webSetting.icp }}</a>
      </form>
    </div>

    <loading-line :loading="model.ssoLoading" :show-text="true" :text="'登录中,请稍候...'"></loading-line>
  </div>
</template>

<script setup>
import { reactive, computed, onMounted } from 'vue';
import { useRouter, useRoute } from 'vue-router';
import loadingLine from '@/components/loadings/loading-line.vue';
import skyArea from '@/components/sky-area-bg/index.vue';
import { ssoLogin, loginValidate, login } from '@/api/authentication';
import { stores } from '../../stores';
import { debounce } from '../../utils/tool';
import toast from '../../utils/toast';
import secret from '../../utils/secret';

const route = useRoute();
const router = useRouter();

const model = reactive({
  form: {
    email: '',
    password: '',
    remember: false
  },
  focus: {
    email: false,
    password: false
  },
  disabled: false,
  ssoLoading: false
});

const computedEmail = computed(() => {
  return model.focus.email || model.form.email.length;
});

const computedPassword = computed(() => {
  return model.focus.password || model.form.password.length;
});

onMounted(async () => {
  // if (document.referrer && !document.referrer.includes(web.ssoReferer)) {
  //   return;
  // }

  if (route.query.key) {
    let token = secret.decrypt(decodeURIComponent(route.query.key));
    if (token) {
      model.ssoLoading = true;
      try {
        let res = await ssoLogin(token);
        if (res) {
          if (res.resCode == 0) {
            stores.authorize.setUserLoginState(res.data);
            toDashboardPage();
          } else if (res.resCode == -21) {
            toast.info('登录凭证已过期，请重新登录');
          }
        }
      } catch (error) {
        if (error.statusCode != undefined && error.statusCode == 401) {
          toast.info('登录凭证已过期，请重新登录');
        } else {
          console.log(error);
        }
      } finally {
        model.ssoLoading = false;
      }
    }
  }
});

const userLogin = debounce(async () => {
  if (model.form.email == '') {
    toast.warn('邮箱不能为空');
    return;
  } else if (model.form.password == '') {
    toast.warn('密码不能为空');
    return;
  }

  model.disabled = true;

  let res = await loginValidate(model.form).catch(err => {
    model.disabled = false;
    throw err;
  });

  try {
    if (res && res.resCode == 0) {
      let data = {
        email: model.form.email,
        oathCode: res.data.oathCode,
        remember: model.form.remember
      };

      res = await login(data);

      if (res && res.resCode == 0) {
        stores.authorize.setUserLoginState(res.data);
        stores.screenLock.setActive();
        toDashboardPage();
      }
    }
  } finally {
    model.disabled = false;
  }
}, 300);

const toDashboardPage = () => {
  router.push({
    name: 'dashboard'
  });
};
</script>

<style lang="scss" scoped>
body {
  font-family: AlimamaFangYuan;
  letter-spacing: 1.5px;

  img {
    -webkit-user-drag: none;
  }
}
.bg {
  margin: 0;
  padding: 0;
  height: 100vh;
  width: 100%;
  position: relative;

  .login-container {
    width: 500px;
    background: rgb(241 241 241 / 55%);
    padding: 30px 40px;
    border-radius: 10px;
    position: absolute;
    opacity: 0.9;
    left: 50%;
    top: 45%;
    transform: translate(-50%, -50%);

    h1 {
      text-align: center;
      margin-top: 20px;
      margin-bottom: 45px;
    }

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
        color: #adadad;
        transform: translateY(-50%);
        z-index: -1;
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
      margin-bottom: 20px;

      .login-btn {
        display: block;
        width: 100%;
        height: 50px;
        border: none;
        background: linear-gradient(120deg, #3498db, #8e44ad, #3498db);
        background-size: 200%;
        color: #fff;
        outline: none;
        cursor: pointer;
        transition: 0.5s;
        border-radius: 8px;
        letter-spacing: 30px;
        text-indent: 30px;
      }

      .login-btn:hover {
        background-position: right;
      }
    }

    .footer {
      cursor: pointer;
      transform: color 0.3s;
      img {
        height: 18px;
        width: 18px;
        margin-right: 5px;
      }

      &:hover {
        color: var(--color-purple);
      }
    }
  }
}
</style>
