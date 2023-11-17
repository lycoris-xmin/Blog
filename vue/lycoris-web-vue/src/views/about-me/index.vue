<template>
  <div class="container" @wheel="pageWheel">
    <div class="logo">
      <img src="/icon/logo/logo-lycoirs.png" @click="toHome" />
    </div>

    <page-home :index="model.index" :height="model.height" :description="model.info.description || []"></page-home>

    <page-owner :index="model.index" :height="model.height" :data="model.info"></page-owner>

    <page-skill :index="model.index" :height="model.height" :data="model.skill"></page-skill>

    <page-link :index="model.index" :height="model.height"></page-link>

    <div class="page-select">
      <ul>
        <li v-for="item in model.maxScreen" :key="item">
          <div @click="model.index = item" class="radio" :class="{ active: model.index == item }"></div>
        </li>
      </ul>
    </div>

    <div class="footer flex-center-center">
      <div class="footer-item">Copyright © 2020 - {{ new Date().getFullYear() }} All Rights Reserved.</div>

      <div class="footer-item">
        <a class="icp flex-center-center" href="https://beian.miit.gov.cn/" target="_blank">
          <img src="/icon/gongan.png" />
          <span>{{ web.icp }}</span>
        </a>
      </div>

      <div class="footer-item">
        <router-link to="/">{{ web.name }} - {{ stores.owner?.nickName }}</router-link>
      </div>

      <div class="footer-item">
        <router-link to="/about/web">关于本站</router-link>
      </div>
    </div>
  </div>
</template>

<script setup>
import { onMounted, onDeactivated, reactive } from 'vue';
import { useRouter } from 'vue-router';
import pageHome from './components/page-home.vue';
import pageOwner from './components/page-owner.vue';
import pageSkill from './components/page-skill.vue';
import pageLink from './components/page-link.vue';
import { stores } from '../../stores';
import { getWebOwner, getAboutMe } from '../../api/home';
import { web } from '../../config.json';

const router = useRouter();

const model = reactive({
  height: '100vh',
  index: 1,
  maxScreen: 4,
  info: {},
  skill: {},
  project: []
});

const emit = defineEmits(['loading']);

onMounted(async () => {
  try {
    await webOwnerStoreInit();

    let res = await getAboutMe();
    if (res && res.resCode == 0) {
      model.info = res.data.info || {};
      model.skill = res.data.skill || {};
      model.project = res.data.project || [];
    }
  } finally {
    emit('loading', false);
  }
});

onDeactivated(() => {
  model.index = 1;
});

const webOwnerStoreInit = async () => {
  try {
    if (!stores.owner.isValid) {
      let res = await getWebOwner();

      if (res && res.resCode == 0) {
        stores.owner.setData(res.data);
      }
    }
  } catch (error) {}
};

const toHome = () => {
  router.push({
    name: 'home'
  });
};

let isAnimate = false;
const pageWheel = e => {
  if (!e.deltaY) {
    return;
  }

  if (isAnimate) {
    return;
  }

  isAnimate = true;

  if (e.deltaY > 0) {
    if (model.index < model.maxScreen) {
      model.index++;
    }
  } else {
    if (model.index > 1) {
      model.index--;
    }
  }

  setTimeout(() => {
    isAnimate = false;
  }, 750);
};
</script>

<style lang="scss" scoped>
@import url('../../assets/theme/theme.min.css');

.container {
  height: 100vh;
  width: 100%;
  overflow: hidden;
  position: relative;

  .logo {
    position: fixed;
    left: 0;
    top: 0;
    z-index: 100;
    padding: 20px;

    > img {
      cursor: pointer;
    }
  }

  .page-select {
    position: absolute;
    z-index: 50;
    right: 0;
    top: 50%;
    transform: translateY(-50%);

    li {
      padding: 10px 10px;
      list-style: none;

      .radio {
        background-color: transparent;
        border-radius: 50%;
        height: 25px;
        width: 25px;
        border: 2px dashed var(--color-secondary);
        cursor: pointer;
        transition: all 0.4s;
      }

      .radio.active,
      .radio:hover {
        background-color: rgba(255, 255, 255, 0.596);
        border-color: #fff;
        border-style: solid;
      }
    }
  }

  $text-color: #fff;
  .footer {
    position: fixed;
    bottom: 0;
    z-index: 40;
    width: 100%;
    height: 40px;
    color: $text-color;

    .footer-item {
      margin-right: 10px;

      a {
        color: $text-color;
        text-decoration: none;
        transition: color 0.5s;
        cursor: pointer;
      }

      a:active,
      a:hover {
        color: var(--color-primary) !important;
      }

      .icp {
        span {
          padding-left: 8px;
        }
      }
    }

    .footer-item:last-child {
      margin: 0;
    }
  }
}
</style>

<style lang="scss">
@import url(../../assets/theme/theme.min.css);

.about-me-screen-view {
  position: absolute;
  left: 0;
  top: 0;
  width: 100%;
  overflow: hidden;
  transition: height 1s;

  .view-body {
    position: relative;
    height: 100%;
    width: 100%;

    > img {
      position: absolute;
      left: 0;
      top: 0;
      height: 100%;
      width: 100%;
      object-fit: cover;
    }
  }
}

@for $i from 1 through 6 {
  .about-me-screen-view:nth-child(#{$i + 1}) {
    z-index: 20 - $i;
  }
}
</style>
