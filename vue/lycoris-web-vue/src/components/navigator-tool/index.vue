<template>
  <div class="navigator-tool">
    <div class="navigator-tool-btn flex-center-center">
      <el-icon :size="24" @click="showMenus">
        <component :is="'more'"></component>
      </el-icon>
    </div>

    <div class="navigator-menus-holder" :class="{ show: displayMenus }">
      <slot> </slot>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue';

const displayMenus = ref(false);

const showMenus = () => {
  console.log('1');
  displayMenus.value = !displayMenus.value;
};
</script>

<style lang="scss">
.navigator-tool {
  position: fixed;
  right: 30px;
  bottom: 25px;
  background: var(--main-linear-gradient);
  height: 40px;
  width: 40px;
  border-radius: 50%;
  cursor: pointer;

  transform: scale(1);
  transition: all 0.4s;

  .navigator-tool-btn {
    height: 100%;
    width: 100%;

    .el-icon {
      z-index: 2;
      color: #fff;
      transform: scale(1);
      transition: all 0.4s;
    }

    .el-icon:hover {
      transform: scale(1.2);
    }
  }
}

.navigator-tool:hover {
  transform: scale(1.2);
}

.navigator-menus-holder {
  top: -100px;
  left: -100px;
  width: 100px;
  height: 100px;
  position: relative;
  z-index: 100;
}

@media (max-width: 1920px) {
  .navigator-tool {
    right: 45px;
    bottom: 25px;
  }
}
</style>
