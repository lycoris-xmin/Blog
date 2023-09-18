<template>
  <div>
    <div v-if="model.code == 404">
      <p>找不到资源</p>
    </div>
    <div v-else-if="model.code == 500">
      <p>服务异常</p>
    </div>
    <div v-else>
      <p>未知错误</p>
    </div>
  </div>
</template>

<script setup name="server-error">
import { onMounted } from 'vue';
import { useRoute } from 'vue-router';

const route = useRoute();

const model = {
  code: 500
};

onMounted(() => {
  model.code = route.params?.code || 500;
});
</script>

<style lang="scss" scoped></style>
