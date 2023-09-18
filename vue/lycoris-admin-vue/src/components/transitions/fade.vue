<template>
  <transition name="fade" :mode="props.mode">
    <slot></slot>
  </transition>
</template>

<script setup>
const props = defineProps({
  mode: {
    type: String,
    default: 'out-in'
  }
});
</script>

<style lang="scss">
.fade-enter-active {
  animation: fadeIn 0.5s;
}

.fade-leave-active {
  animation: fadeOut 0.3s;
}
</style>
