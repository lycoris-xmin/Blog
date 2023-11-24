<template>
  <el-config-provider :locale="options.locale" :button="options.button" :message="options.message">
    <router-view v-slot="{ Component }">
      <transition name="router_animate" mode="out-in">
        <component :is="Component" />
      </transition>
    </router-view>
  </el-config-provider>
</template>

<script setup>
import { onBeforeMount, reactive } from 'vue';
import { ElConfigProvider } from 'element-plus';
import zhCn from 'element-plus/lib/locale/lang/zh-cn';

const options = reactive({
  locale: zhCn,
  button: {
    autoInsertSpace: true
  },
  message: {
    max: 3
  }
});

onBeforeMount(() => {});
</script>

<style lang="scss" scoped>
.router_animate-enter-active {
  animation: fadeIn 0.5s;
}

.router_animate-leave-active {
  animation: fadeOut 0.3s;
}
</style>
