<template>
  <div class="page" :class="{ 'global-gray': model.grayWebSite, 'page-overflow': model.loading }">
    <div class="banner"></div>
    <layout-nav @search="searchPost" @login="userLogin" @user-message="userMessage"></layout-nav>

    <div class="page-body">
      <router-view v-slot="{ Component }">
        <transition-fade>
          <keep-alive :max="10" :include="keepAliveComponents">
            <component :is="Component" :key="$route.fullPath" @loading="loading" @browse="browse" />
          </keep-alive>
        </transition-fade>
      </router-view>
    </div>

    <layout-footer></layout-footer>

    <search-modal ref="searchModalRef"></search-modal>
    <login-modal v-if="!stores.user.state" ref="loginModalRef" @refresh-user-brief="userBriefInit"></login-modal>
    <!-- <chat-modal v-if="stores.user.state" ref="chatModalRef"></chat-modal> -->
    <transition-fade>
      <loading-line :loading="model.loading" v-if="model.loading" style="height: 100vh; width: 100vw; position: fixed"></loading-line>
    </transition-fade>
  </div>
</template>

<script setup name="layout">
import { onMounted, provide, reactive, ref, watch, computed } from 'vue';
import { useRoute } from 'vue-router';
import transitionFade from '@/components/transitions/fade.vue';
import layoutNav from './components/layout-nav.vue';
import layoutFooter from './components/layout-footer.vue';
import searchModal from './modal/search-modal.vue';
import loginModal from './modal/login-modal.vue';
// import chatModal from './modal/chat/index.vue';
import loadingLine from '@/components/loadings/loading-line.vue';
import { keepAliveComponents } from '@/router';
import SignalRHelper from '@/utils/signalR';
import { stores } from '@/stores';
import { getUserBrief } from '@/api/user';
import { pageBrowse, getPostIcon, getpublishStatistics } from '@/api/home';

const route = useRoute();
const searchModalRef = ref();
const loginModalRef = ref();
const chatModalRef = ref();
const signalR = new SignalRHelper();
const chatSignalR = new SignalRHelper();

const domain = computed(() => `${location.protocol}//${location.host}`);

provide('$loginModal', loginModalRef);
provide('$chatModal', chatModalRef);
provide('$signalR', signalR);
provide('$chat-signalR', chatSignalR);
provide('$domain', domain);

const model = reactive({
  loading: true,
  grayWebSite: false
});

const closeModalPath = ['/server/error', '/server/notfound', '/about/me'];

watch(
  () => route.fullPath,
  value => {
    if (closeModalPath.includes(value)) {
      searchModalRef.value.close();
      if (loginModalRef.value) {
        loginModalRef.value.close();
      }
    }
  }
);

onMounted(async () => {
  if (!loginModalRef.value) {
    Object.freeze(loginModalRef);
  }

  publishStatisticsInit();
  postIcon();
  userBriefInit();
});

const publishStatisticsInit = async () => {
  try {
    let res = await getpublishStatistics();
    if (res && res.resCode == 0) {
      stores.owner.setStatistics(res.data);
    }
  } catch (error) {}
};

const postIcon = async () => {
  try {
    let res = await getPostIcon();
    if (res && res.resCode == 0) {
      stores.postIcon.setPostIcon(res.data.list);
    }
  } catch (error) {}
};

const userBriefInit = async () => {
  if (stores.authorize.refreshToken && stores.authorize.refreshTokenExpireTime > new Date().getTime()) {
    try {
      let res = await getUserBrief();
      if (res && res.resCode == 0) {
        stores.user.setLoginState(res.data);
        signalR.setupSignalR('/hub/home');
      } else {
        stores.user.setLogoutState();
      }
    } catch (error) {}
  }
};

const searchPost = () => {
  searchModalRef.value.show();
};

const userLogin = () => {
  loginModalRef.value.show();
};

const userMessage = () => {
  chatModalRef.value.show();
};

let live = false;
const loading = show => {
  if (!live) {
    model.loading = show;
    if (!show) {
      live = true;
    }
  }
};

const browse = data => {
  if (route) {
    if (stores.browse.checkCanSync(route.fullPath)) {
      data = data || {};
      if (!Object.keys(data).includes('pageName')) {
        data.pageName = route.meta.pageName;
      }

      pageBrowse(route.fullPath, {
        ...data,
        referer: document.referrer
      }).then(res => {
        if (res && res.resCode == 0) {
          stores.browse.setSync(route.fullPath);
        }
      });
    }
  }
};
</script>

<style lang="scss" scoped>
.page-overflow {
  overflow: hidden;
  height: 100vh;
}

.page {
  position: relative;
  margin: 0;
  padding: 0;
  min-height: 100vh;
  width: 100%;
  background-color: var(--main-background-color);

  .page-body {
    min-height: var(--page-min-height);
    position: relative;
    width: var(--container-width);
    margin: 0 auto;
    padding: 0 10px;
  }

  .banner {
    display: block;
    height: 600px;
    position: absolute;
    top: 0;
    left: 0;
    width: 100%;
    background: var(--main-linear-gradient);
    opacity: 0.99;
    clip-path: polygon(
      100% 0,
      0 0,
      0 77.5%,
      1% 77.4%,
      2% 77.1%,
      3% 76.6%,
      4% 75.9%,
      5% 75.05%,
      6% 74.05%,
      7% 72.95%,
      8% 71.75%,
      9% 70.55%,
      10% 69.3%,
      11% 68.05%,
      12% 66.9%,
      13% 65.8%,
      14% 64.8%,
      15% 64%,
      16% 63.35%,
      17% 62.85%,
      18% 62.6%,
      19% 62.5%,
      20% 62.65%,
      21% 63%,
      22% 63.5%,
      23% 64.2%,
      24% 65.1%,
      25% 66.1%,
      26% 67.2%,
      27% 68.4%,
      28% 69.65%,
      29% 70.9%,
      30% 72.15%,
      31% 73.3%,
      32% 74.35%,
      33% 75.3%,
      34% 76.1%,
      35% 76.75%,
      36% 77.2%,
      37% 77.45%,
      38% 77.5%,
      39% 77.3%,
      40% 76.95%,
      41% 76.4%,
      42% 75.65%,
      43% 74.75%,
      44% 73.75%,
      45% 72.6%,
      46% 71.4%,
      47% 70.15%,
      48% 68.9%,
      49% 67.7%,
      50% 66.55%,
      51% 65.5%,
      52% 64.55%,
      53% 63.75%,
      54% 63.15%,
      55% 62.75%,
      56% 62.55%,
      57% 62.5%,
      58% 62.7%,
      59% 63.1%,
      60% 63.7%,
      61% 64.45%,
      62% 65.4%,
      63% 66.45%,
      64% 67.6%,
      65% 68.8%,
      66% 70.05%,
      67% 71.3%,
      68% 72.5%,
      69% 73.6%,
      70% 74.65%,
      71% 75.55%,
      72% 76.35%,
      73% 76.9%,
      74% 77.3%,
      75% 77.5%,
      76% 77.45%,
      77% 77.25%,
      78% 76.8%,
      79% 76.2%,
      80% 75.4%,
      81% 74.45%,
      82% 73.4%,
      83% 72.25%,
      84% 71.05%,
      85% 69.8%,
      86% 68.55%,
      87% 67.35%,
      88% 66.2%,
      89% 65.2%,
      90% 64.3%,
      91% 63.55%,
      92% 63%,
      93% 62.65%,
      94% 62.5%,
      95% 62.55%,
      96% 62.8%,
      97% 63.3%,
      98% 63.9%,
      99% 64.75%,
      100% 65.7%
    );
  }
}
</style>

<style lang="scss">
:root {
  --page-min-height: calc(100vh - var(--page-sub-height));
  --page-sub-height: 140px;
  --container-width: 1500px;

  @media (max-width: 1920px) {
    --container-width: 1320px;
  }

  @media (max-width: 1440px) {
    --container-width: 1120px;
  }
}

.el-layout-modal {
  border-radius: 10px;
}

@import url(@/assets/theme/theme.min.css);
</style>
