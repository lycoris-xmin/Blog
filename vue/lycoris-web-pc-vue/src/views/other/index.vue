<template>
  <div class="container">
    <div class="info">还没开发呢，别看了</div>
  </div>
</template>

<script setup name="other">
import { onMounted } from 'vue';

const emit = defineEmits(['loading', 'browse']);

onMounted(() => {
  emit('loading', false);
  emit('browse');
});
</script>

<style lang="scss" scoped>
.container {
  height: calc(100vh - 140px);
  width: 100%;

  padding: 45px 0;

  .info {
    display: flex;
    justify-content: center;
    align-items: center;
    height: 100%;
    background-color: #fff;
    border-radius: var(--main-container-radius);

    font-size: 50px;
    letter-spacing: 10px;
  }
}
</style>
