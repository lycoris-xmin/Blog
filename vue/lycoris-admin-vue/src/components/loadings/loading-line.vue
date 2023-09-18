<template>
  <div class="loading-preloader flex-center-center" v-show="props.loading">
    <div class="loading-preloader-inner"></div>
    <span class="loading-text" v-if="props.showText">{{ props.text }}</span>
  </div>
</template>

<script setup>
const props = defineProps({
  loading: {
    type: Boolean,
    default: true
  },
  text: {
    type: String,
    default: '加载中...'
  },
  showText: {
    type: Boolean,
    default: false
  }
});
</script>

<style lang="scss" scoped>
.loading-preloader {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background-color: #fff;
  z-index: 1000;
  flex-direction: column;

  .loading-preloader-inner {
    display: block;
    width: 80px;
    height: 80px;
    border-radius: 50%;
    border: 3px solid transparent;
    border-top-color: #3498db;
    animation: spin 2s linear infinite;
  }

  .loading-preloader-inner:before {
    content: '';
    position: absolute;
    top: 5px;
    left: 5px;
    right: 5px;
    bottom: 5px;
    border-radius: 50%;
    border: 3px solid transparent;
    border-top-color: #e74c3c;
    animation: spin 3s linear infinite;
  }

  .loading-preloader-inner:after {
    content: '';
    position: absolute;
    top: 15px;
    left: 15px;
    right: 15px;
    bottom: 15px;
    border-radius: 50%;
    border: 3px solid transparent;
    border-top-color: #f9c922;
    animation: spin 1.5s linear infinite;
  }

  @keyframes spin {
    0% {
      transform: rotate(0deg);
    }
    to {
      transform: rotate(1turn);
    }
  }

  .loading-text {
    padding-top: 20px;
    width: 100%;
    text-align: center;
    font-size: 14px;
    font-weight: 500;
    letter-spacing: 2px;
  }
}
</style>
