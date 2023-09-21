<template>
  <div class="common-layout">
    <el-container class="layout-container">
      <layout-aside :menus="model.menus" :web-path="model.webPath"></layout-aside>
      <el-container>
        <el-header class="layout-header">
          <div class="layout-header-lef flex-center-center">
            <el-icon>
              <component :is="model.slide"></component>
            </el-icon>
          </div>
          <div class="layout-header-right flex-start-center">
            <el-avatar :size="40" :src="stores.owner.avatar" v-if="stores.owner.avatar && stores.owner.avatar.length" />
            <el-avatar :icon="UserFilled" v-else />
            <span class="user-name" @click="showDrawer">{{ stores.owner.nickName }}</span>
          </div>
        </el-header>
        <el-main class="layout-content">
          <div class="layout-content-body">
            <div class="view-body">
              <router-view v-slot="{ Component }">
                <transition name="router_animate" mode="out-in">
                  <keep-alive :max="10" :include="keepAliveCompoents">
                    <component :is="Component" :key="$route.fullPath" />
                  </keep-alive>
                </transition>
              </router-view>
            </div>
          </div>
        </el-main>
      </el-container>
    </el-container>
    <user-drawer ref="drawer" :visible="model.showDrawer"></user-drawer>
  </div>
</template>

<script setup>
import { onMounted, onUnmounted, reactive, ref, computed, provide } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { UserFilled } from '@element-plus/icons-vue';
import layoutAside from './components/layout-aside.vue';
import UserDrawer from './components/layout-user-drawer.vue';
import { menus } from '../../router';
import SignalRHelper from '../../utils/signalR';
import { refreshToken } from '../../api/authentication';
import { getUserBrief } from '../../api/user';
import { getWebSettings } from '../../api/configuration';
import { stores } from '../../stores';
import { debounce } from '../../utils/tool';

const route = useRoute();
const router = useRouter();

const signalR = new SignalRHelper();
provide('$signalR', signalR);

const drawer = ref(null);

const model = reactive({
  showDrawer: false,
  slide: 'fold',
  menus: menus,
  webPath: ''
});

const keepAliveCompoents = computed(() => {
  let compoents = [];
  for (let item of menus) {
    if (item.component && item.component.keepAlive) {
      compoents.push(item.component.name);
    }

    if (item.routes && item.routes.length) {
      for (let route of item.routes) {
        if (route.component && route.component.keepAlive) {
          compoents.push(route.component.name);
        }
      }
    }
  }

  return compoents;
});

const screenLock = {
  body: document.querySelector('html'),
  timer: void 0
};

onMounted(async () => {
  try {
    if (!stores.owner.isValid) {
      let res = await getUserBrief();

      if (res && res.resCode == 0) {
        stores.owner.setData(res.data);
      }
    }

    checkLossOfActivityTo();

    screenLock.timer = setInterval(checkLossOfActivityTo, 10000);

    await getWebPath();

    await signalR.setupSignalR('/lycoris/hub/dashboard');

    subscribeAuthroization();
    subscribeRefreshToken();

    // 鼠标移动
    screenLock.body?.addEventListener('mousemove', handleEvent);
    // 鼠标滚动事件
    screenLock.body?.addEventListener('mousewheel', handleEvent);
  } catch (error) {}
});

onUnmounted(async () => {
  screenLock.body?.removeEventListener('mousemove', handleEvent);
  screenLock.body?.removeEventListener('mousewheel', handleEvent);

  clearInterval(screenLock.timer);

  await signalR.stop();
});

const getWebPath = async () => {
  let res = await getWebSettings();
  if (res && res.resCode == 0) {
    model.webPath = res.data.webPath;
  }
};

const showDrawer = () => {
  model.showDrawer = true;
  drawer.value.show();
};

const subscribeAuthroization = () => {
  signalR.subscribe('authroization', () => {
    signalR.invoke('userAuthroization', stores.authorize.token);
  });
};

const subscribeRefreshToken = () => {
  signalR.subscribe('refreshToken', async () => {
    let res = await refreshToken(stores.authorize.refreshToken);
    if (res && res.resCode == 0) {
      stores.authorize.setUserLoginState(res.data.token);
    } else {
      signalR.stop();
      stores.authorize.setUserLogoutState();
    }
  });
};

const handleEvent = debounce(() => {
  // 设置活动时间
  if (!stores.screenLock.checkLossOfActivity()) {
    stores.screenLock.setActive();
  }
}, 5000);

const checkLossOfActivityTo = () => {
  //
  if (stores.screenLock.checkLossOfActivity()) {
    // 跳转锁屏页
    router.push({
      name: 'screen-lock',
      query: {
        path: route.path
      }
    });
  }
};
</script>

<style lang="scss" scoped>
$layout-header-h: 64px;

.common-layout {
  position: relative;
  padding: 0;
  margin: 0;
  color: var(--color-text);

  .layout-container {
    min-height: 100vh;
    width: 100%;

    .layout-header {
      height: $layout-header-h;
      width: 100%;
      padding: 10px 20px;
      display: flex;
      justify-content: space-between;
      align-items: center;
      border-bottom: 1px solid --color-secondary;
      box-shadow: 0 5px 8px -5px var(--color-secondary);

      .layout-header-left {
        height: 100%;
        cursor: pointer;
      }

      .layout-header-right {
        height: 100%;
        min-width: 120px;
        padding-right: 20px;

        .el-avatar {
          cursor: pointer;
        }

        .user-name {
          padding-left: 10px;
          font-size: 16px;
          font-weight: 500;
          cursor: pointer;
          transition: all 0.5s;
        }

        .user-name:hover {
          color: var(--color-primary);
        }
      }
    }

    .layout-content {
      min-height: calc(100vh - $layout-header-h);
      background-color: var(--color-secondary);

      .layout-content-body {
        min-height: calc(100vh - $layout-header-h - 40px);
        width: 100%;
        background-color: #fff;
        border-radius: 15px;
        padding: 10px;

        .view-body {
          overflow: hidden;
          height: 100%;

          .router_animate-enter-active {
            animation: slideInRight 0.5s;
          }

          .router_animate-leave-active {
            animation: slideOutLeft 0.3s;
          }
        }
      }
    }
  }
}
</style>

<style lang="scss">
body {
  * {
    font-weight: 600;
    letter-spacing: 1.5px;
  }
}
</style>
