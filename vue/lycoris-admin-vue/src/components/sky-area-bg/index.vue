<template>
  <div class="satic-area">
    <div class="dynamic-area1"></div>
    <div class="dynamic-area2"></div>
  </div>
</template>

<script setup></script>

<style lang="scss" scoped>
.satic-area {
  position: relative;
  width: 100%;
  height: 100%;
  background: url(../../assets/images/login-main.jpg) no-repeat center;
  background-size: cover;
}

.dynamic-area1 {
  position: absolute;
  width: 100%;
  height: 100%;
  top: 0;
  left: 0;
  background: url(../../assets/images/login-animate-2.png) repeat-x 0px 0px;
  background-size: cover;
  animation: posterDrop1 6000s linear infinite;
}

@keyframes posterDrop1 {
  from {
    background-position: 0 0;
  }
  to {
    background-position: 4000% 0;
  }
}

.dynamic-area2 {
  position: absolute;
  width: 100%;
  height: 100%;
  top: 0;
  left: 0;
  background: url(../../assets/images/login-animate-1.png) repeat-x 0px 0px;
  background-size: cover;
  animation: posterDrop2 8000s linear infinite;
}

@keyframes posterDrop2 {
  from {
    background-position: 0 0;
  }
  to {
    background-position: 30000% 0;
  }
}
</style>
