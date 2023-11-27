<template>
  <div class="page-footer flex-center-center">
    <div class="page-footer-item" style="color: var(--color-dark)">Copyright © 2020 - {{ new Date().getFullYear() }} All Rights Reserved.</div>

    <div class="page-footer-item">
      <a class="icp flex-center-center" href="https://beian.miit.gov.cn/" target="_blank">
        <img src="/icon/gongan.png" title="备案号" />
        <span>{{ stores.webSetting.icp }}</span>
      </a>
    </div>

    <div class="page-footer-item">
      <router-link to="/">{{ stores.webSetting.webName }} - {{ stores.owner?.nickName }}</router-link>
    </div>

    <div class="page-footer-item">
      <router-link to="/about/web">关于本站</router-link>
    </div>
    <div class="page-footer-item">
      <router-link to="/about/me">关于我</router-link>
    </div>
  </div>
</template>

<script setup>
import { stores } from '@/stores';
</script>

<style lang="scss" scoped>
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
      > img {
        height: 20px;
        width: 20px;
      }
      span {
        padding-left: 8px;
      }
    }
  }

  .page-footer-item:last-child {
    margin: 0;
  }
}
</style>
