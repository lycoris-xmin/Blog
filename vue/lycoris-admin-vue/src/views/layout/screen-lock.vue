<template>
  <div class="lock-screen">
    <sky-area></sky-area>

    <div class="lock-container">
      <div class="lock-text">锁屏中...</div>
      <div class="flex-center-center">
        <el-image class="avatar" :src="stores.owner.avatar" lazy />
      </div>
      <p class="nickname">{{ stores.owner.nickName }}</p>
      <div class="form-group">
        <el-input v-model="model.password" size="large" type="password" placeholder="请输入密码解锁"></el-input>
      </div>
      <div class="lock-btn">
        <el-button type="primary" @click="unlock">解锁</el-button>
      </div>
    </div>
  </div>
</template>

<script setup name="screen-lock">
import { onMounted, onUnmounted, reactive } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import skyArea from '../../components/sky-area-bg/index.vue';
import { stores } from '../../stores';
import { screenUnLock, refreshToken } from '../../api/authentication';
import { getUserBrief } from '../../api/user';
import { pageRoutes } from '../../router/routes';
import MessageChannel from '../../utils/message-channel';

const route = useRoute();
const router = useRouter();

const channelType = 'screen-lock';
// 创建MessageChannel实例并发送消息
const channel = new MessageChannel(channelType);

const model = reactive({
  path: '',
  password: '',
  display: true
});

onMounted(async () => {
  //
  model.path = route.query?.path || pageRoutes[0].children.filter(x => x.name === 'dashboard')[0].path;

  channel.sendMessage(0);

  await tokenCheck();

  await ownerInit();

  setInterval(tokenCheck, 60000);

  channel.onMessage(data => {
    // 0-其他窗口占用
    // 1-窗口关闭
    // 2-解锁成功
    if (data === 0) {
      model.display = false;
      //
    } else if (data === 1) {
      //
      model.display = true;
    } else if (data === 2) {
      router.push({ path: model.path });
    }
  });
});

onUnmounted(() => {
  channel.sendMessage(1);
  channel.close();
});

const unlock = async () => {
  try {
    if (model.password) {
      let res = await screenUnLock(model.password);
      if (res != null && res.resCode == 0) {
        stores.screenLock.setActive();

        // 通知其他窗口跳转
        channel.sendMessage(2);
        router.push({ path: model.path });
      }
    }
  } catch (error) {}
};

const ownerInit = async () => {
  if (!stores.owner.isValid) {
    let res = await getUserBrief();

    if (res && res.resCode == 0) {
      stores.owner.setData(res.data);
    }
  }
};

const tokenCheck = async () => {
  console.log('检查令牌');
  if (document.visibilityState === 'visible' && model.display) {
    // 当前页面为展示窗口
    // 验证令牌有效期
    let res = await refreshToken(stores.authorize.refreshToken);
    if (res && res.resCode == 0) {
      stores.authorize.setUserLoginState(res.data);
    }

    console.log('检查令牌执行');
  } else {
    console.log('检查令牌不执行');
  }
};
</script>

<style lang="scss" scoped>
.lock-screen {
  margin: 0;
  padding: 0;
  height: 100vh;
  width: 100%;
  position: relative;

  .lock-container {
    width: 500px;
    background: rgb(241 241 241 / 55%);
    padding: 30px 40px;
    border-radius: 10px;
    position: absolute;
    opacity: 0.9;
    left: 50%;
    top: 45%;
    transform: translate(-50%, -50%);

    .lock-text {
      font-size: 18px;
      text-align: center;
      margin-bottom: 20px;
    }

    .avatar {
      height: 100px;
      width: 100px;
      object-fit: cover;
      border-radius: 16px;
      border: 2px solid var(--color-purple);
    }

    .nickname {
      text-align: center;
      margin: 30px 0 15px;
      font-size: 24px;
    }

    .form-group {
      margin: 0 auto;
      width: 300px;
    }

    .lock-btn {
      text-align: center;
      margin: 20px 0 10px;

      .el-button {
        width: 100px;
      }
    }
  }
}
</style>
