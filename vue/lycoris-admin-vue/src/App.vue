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
import { reactive } from 'vue';
import { useRoute } from 'vue-router';
import { ElConfigProvider } from 'element-plus';
import zhCn from 'element-plus/lib/locale/lang/zh-cn';
import { getWebSetting } from '@/api/home';
import { stores } from './stores';

const options = reactive({
  locale: zhCn,
  button: {
    autoInsertSpace: true
  },
  message: {
    max: 5
  }
});

const route = useRoute();

const webSetting = async () => {
  let res = await getWebSetting();
  if (res && res.resCode == 0) {
    stores.webSetting.setData(res.data.common);

    if (stores.webSetting.webName) {
      document.title = `${route.meta.title}_${stores.webSetting.webName}`;
    } else {
      document.title = `${route.meta.title}_管理后台`;
    }
  }
};

webSetting();
</script>

<style lang="scss" scoped>
.router_animate-enter-active {
  animation: fadeIn 0.5s;
}

.router_animate-leave-active {
  animation: fadeOut 0.3s;
}
</style>
