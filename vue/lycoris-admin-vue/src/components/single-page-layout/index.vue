<template>
  <div class="page">
    <div class="page-container">
      <slot></slot>
    </div>

    <div class="page-footer flex-center-center">
      <div class="page-footer-item">Copyright © 2020 - {{ new Date().getFullYear() }} All Rights Reserved.</div>

      <div class="page-footer-item">
        <a class="icp flex-center-center" href="https://beian.miit.gov.cn/" target="_blank">
          <img src="/icon/gongan.png" />
          <span>{{ web.icp }}</span>
        </a>
      </div>

      <div class="page-footer-item">
        <router-link to="/">{{ web.name }} - {{ stores.owner?.nickName }}</router-link>
      </div>

      <div class="page-footer-item">
        <router-link to="/about/web">关于本站</router-link>
      </div>
      <div class="page-footer-item">
        <router-link to="/about/me">关于我</router-link>
      </div>
    </div>
    <loading-line :loading="props.loading"></loading-line>
  </div>
</template>

<script setup>
import loadingLine from '../loadings/loading-line.vue';
import { stores } from '../../stores';
import { web } from '../../config.json';

const props = defineProps({
  loading: {
    type: Boolean,
    default: false
  }
});
</script>

<style lang="scss" scoped>
.page {
  position: relative;
  height: 100vh;
  width: 100%;

  .page-container {
    min-height: calc(100vh - 40px);
  }

  .page-footer {
    height: 40px;
    border-top: solid 1px var(--el-menu-border-color);

    .page-footer-item {
      margin-right: 10px;
      a {
        color: var(--color-dark);
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

    .page-footer-item:last-child {
      margin: 0;
    }
  }
}
</style>
