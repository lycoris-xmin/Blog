<template>
  <transition-group name="transitiongroupfade" :tag="props.tag || 'ul'">
    <slot></slot>
  </transition-group>
</template>

<script setup>
const props = defineProps({
  tag: {
    type: String,
    default: 'ul'
  }
});
</script>

<style lang="scss">
.transitiongroupfade-move,
.transitiongroupfade-enter-active,
.transitiongroupfade-leave-active {
  transition: all 0.5s cubic-bezier(0.55, 0, 0.1, 1);
}

.transitiongroupfade-enter-from,
.transitiongroupfade-leave-to {
  opacity: 0;
  transform: scaleY(0.01) translate(30px, 0);
}

.transitiongroupfade-leave-active {
  position: absolute;
}
</style>
