<template>
  <div class="container">
    <div class="header flex-start-center" v-if="props.title">
      <div class="flex-start-center">
        <el-icon :size="36" v-if="props.icon">
          <component :is="props.icon"></component>
        </el-icon>
        <span>{{ props.title }}</span>
      </div>
    </div>
    <transition enter-active-class="animate__animated animate__fadeIn" duration="200">
      <div style="margin-top: 20px">
        <slot></slot>
      </div>
    </transition>
  </div>
</template>

<script setup>
const props = defineProps({
  title: {
    type: String,
    default: ''
  },
  icon: {
    type: String,
    default: ''
  }
});
</script>

<style lang="scss" scoped>
.container {
  min-height: calc(100vh - 140px);
  width: 100%;
  padding: 45px 0;

  .header {
    > div {
      background: var(--main-linear-gradient) no-repeat left bottom;
      background-size: 85% 3px;

      .el-icon {
        margin-right: 15px;
        color: #fff;
      }

      span {
        font-size: 34px;
        color: #fff;
      }
    }
  }
}
</style>
