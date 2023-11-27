<template>
  <el-config-provider :locale="options.locale" :button="options.button" :message="options.message">
    <router-view v-slot="{ Component }">
      <transition name="router_animate" mode="out-in">
        <keep-alive>
          <component :is="Component" />
        </keep-alive>
      </transition>
    </router-view>
  </el-config-provider>
</template>

<script setup>
import { ElConfigProvider } from 'element-plus';
import zhCn from 'element-plus/lib/locale/lang/zh-cn';
import { reactive, onBeforeMount } from 'vue';
import { getWebSetting } from '@/api/home';
import { stores } from './stores';
import { useRouter } from 'vue-router';
import $document from '@/utils/document';

const router = useRouter();

const options = reactive({
  locale: zhCn,
  button: {
    autoInsertSpace: true
  },
  message: {
    max: 1
  }
});

onBeforeMount(async () => {
  await webSettingInit();
  $document.setTitle();
});

const webSettingInit = async () => {
  try {
    let res = await getWebSetting();

    if (!res || res.resCode != 0) {
      router.push({
        name: 'server-error',
        params: {
          statusCode: 404
        }
      });
      return;
    }

    if (res.data.common) {
      stores.webSetting.setData(res.data.common);
    }

    if (res.data.owner) {
      stores.owner.setData(res.data.owner);
    }

    if (res.data.common && res.data.owner) {
      console.log(
        `%c ${stores.webSetting.webName} %c By ${stores.owner.nickName} %c`,
        'background:#e9546b ; padding: 1px; border-radius: 3px 0 0 3px;  color: #fff; padding:5px 0;',
        'background:#ec8c69 ; padding: 1px; border-radius: 0 3px 3px 0;  color: #000; padding:5px 0;',
        'background:transparent'
      );
    }
  } catch (error) {
    router.push({
      name: 'server-error',
      params: {
        statusCode: 404
      }
    });
  }
};
</script>

<style lang="scss" scoped>
.router_animate-enter-active {
  animation: fadeIn 0.5s;
}

.router_animate-leave-active {
  animation: fadeOut 0.3s;
}
</style>
