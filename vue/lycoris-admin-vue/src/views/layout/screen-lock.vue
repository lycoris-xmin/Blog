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
        <el-input size="large" type="password" placeholder="请输入密码解锁"></el-input>
      </div>
      <div class="lock-btn">
        <el-button type="primary" @click="unlock">解锁</el-button>
      </div>
    </div>
  </div>
</template>

<script setup name="screen-lock">
import { onMounted, reactive } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import skyArea from '../../components/sky-area-bg/index.vue';
import { stores } from '../../stores';
import { getUserBrief } from '../../api/user';

const route = useRoute();
const router = useRouter();

const model = reactive({
  path: ''
});

onMounted(async () => {
  //
  model.path = route.query.path;

  await ownerInit();

  setInterval(ownerInit, 60000);
});

const unlock = () => {
  stores.screenLock.setActive();
  router.push({ path: model.path });
};

const ownerInit = async () => {
  if (!stores.owner.isValid) {
    let res = await getUserBrief();

    if (res && res.resCode == 0) {
      stores.owner.setData(res.data);
    }
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
