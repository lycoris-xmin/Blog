<template>
  <TransitionGroup :tag="props.tag || 'ul'" :css="false" @before-enter="onBeforeEnter" @enter="onEnter" @leave="onLeave">
    <slot></slot>
  </TransitionGroup>
</template>

<script setup>
import gsap from 'gsap';

const props = defineProps({
  tag: {
    type: String,
    reqired: true
  },
  delay: {
    type: Number,
    default: 0.15
  }
});

const onBeforeEnter = el => {
  console.log(el);
  el.style.opacity = 0;
  el.style.height = 0;
};

const onEnter = (el, done) => {
  console.log(el);
  gsap.to(el, {
    opacity: 1,
    height: 'auto',
    delay: el.dataset.index * (props.delay || 0.15),
    onComplete: done
  });
};

const onLeave = (el, done) => {
  console.log(el);
  gsap.to(el, {
    opacity: 0,
    height: 0,
    delay: el.dataset.index * (props.delay || 0.15),
    onComplete: done
  });
};
</script>

<style lang="scss" scoped></style>
