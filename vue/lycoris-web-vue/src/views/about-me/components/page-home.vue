<template>
  <div class="about-me-screen-view" :style="{ height: props.index > 1 ? '0px' : props.height }">
    <div class="view-body flex-center-center">
      <img src="../../../assets/images/bg.jpg" alt="" />
      <div class="card" v-show="props.owner.isValid">
        <p :class="{ 'text-showup': props.index == 1 }">Hello,I`m {{ props.owner.nickName }}</p>
        <p :class="{ 'text-showup': props.index == 1 }" v-for="item in props.description" :key="item">{{ item }}</p>
      </div>
    </div>
  </div>
</template>

<script setup>
const props = defineProps({
  index: {
    type: Number,
    defalut: false
  },
  height: {
    type: String,
    defalut: '0px'
  },
  description: {
    type: Array,
    defalut: []
  },
  owner: {
    type: Object,
    defalut: void 0
  }
});
</script>

<style lang="scss" scoped>
.about-me-screen-view {
  .card {
    position: relative;
    background-color: transparent;
    color: #fff;
    transition: all 1s;
    cursor: default;

    > p:first-child {
      font-size: 68px;
    }

    > p:nth-of-type(n + 2) {
      font-size: 30px;
    }

    .text-showup {
      animation: text-showup 1.5s forwards;
    }
  }

  .card:hover {
    background-color: rgba(255, 255, 255, 0.467);
    box-shadow: 0 0 10px 8px var(--color-danger);
  }
}
</style>
