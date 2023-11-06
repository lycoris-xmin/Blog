<template>
  <Teleport to="body">
    <transition-fade>
      <div class="post-img-view" :style="{ display: model.show ? 'flex' : 'none' }" @click="clickOutSiede">
        <div class="img-body" ref="imgPriewRef">
          <img :src="model.src" />
          <div class="close">
            <el-button link @click="model.show = false">
              <el-icon :size="26">
                <component :is="'close'"></component>
              </el-icon>
            </el-button>
          </div>
        </div>
      </div>
    </transition-fade>
  </Teleport>
</template>

<script setup>
import { reactive, ref, onMounted } from 'vue';
import transitionFade from '../../../components/transitions/fade.vue';

const imgPriewRef = ref();

const model = reactive({
  src: '',
  show: false
});

onMounted(() => {});

const show = src => {
  model.src = src;
  model.show = true;
};

const hide = () => {
  model.show = false;
};

const clickOutSiede = e => {
  if (imgPriewRef.value.contains(e.target)) {
    return;
  }

  hide();
};

defineExpose({
  show,
  hide
});
</script>

<style lang="scss" scoped>
.post-img-view {
  padding: 0;
  margin: 0;
  position: fixed;
  top: 0;
  left: 0;
  height: 100vh;
  width: 100%;

  display: none;
  justify-content: center;
  align-items: center;

  z-index: 9999;

  .img-body {
    position: relative;
    border-radius: 15px;
    display: flex;
    justify-content: center;
    align-items: center;
    padding: 50px;
    background-color: rgb(0 0 0 / 40%);

    > img {
      border-radius: 5px;
      min-width: 800px;
      max-height: 800px;
      max-width: 1000px;
      object-fit: fill;
    }

    .close {
      position: absolute;
      right: 0;
      top: 0;
      padding: 5px 10px;

      .el-icon {
        color: #fff;
        transition: color 0.4s;

        &:hover {
          color: var(--color-danger);
        }
      }
    }
  }
}
</style>
