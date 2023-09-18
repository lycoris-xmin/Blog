<template>
  <pageLayout title="个人中心" icon="chat-dot-round">
    <div class="user-container card">
      <div>
        <ul></ul>
        <ul></ul>
      </div>
      <div class="user-view"></div>
    </div>
  </pageLayout>
</template>

<script setup name="cooperate">
import { onMounted } from 'vue';
import pageLayout from '../layout/page-layout.vue';

const emit = defineEmits(['loading', 'browse']);

onMounted(() => {
  emit('loading', false);
  emit('browse');
});
</script>

<style lang="scss" scoped>
.user-container {
  min-height: calc(100vh - 305px);
  width: 100%;
  padding: 10px;

  display: grid;
  grid-template-columns: 300px minmax(500px, 1fr);

  .user-view {
    min-height: calc(100vh - 325px);
  }
}
</style>
