<template>
  <div class="empty-result flex-center-center">
    <div class="icon">
      <el-image src="/images/technology-is-not-good.jpeg"></el-image>
    </div>
    <div class="content">
      <p>{{ props.content }}</p>
    </div>
    <div class="slot">
      <slot></slot>
    </div>
    <div class="notify">
      <p>程序猿顶级秘籍：搞不定就重启电脑</p>
    </div>
  </div>
</template>

<script setup>
const props = defineProps({
  content: {
    type: String,
    default: '没有找到符合条件的文章哦!'
  }
});
</script>

<style lang="scss" scoped>
.empty-result {
  flex-direction: column;
  width: 100%;
  margin-top: 30px;

  .icon {
    :deep(img) {
      width: 400px;
      border-radius: 5px;
      border: 2px dashed var(--color-cyan);
    }
  }

  .content {
    margin-top: 20px;

    p {
      font-size: 18px;
      font-weight: 500;
      letter-spacing: 1px;
    }
  }

  .slot {
    margin-top: 20px;
  }

  .notify {
    margin-top: 20px;

    p {
      color: #008cff;
      letter-spacing: 1px;
      background: linear-gradient(90deg, rgba(131, 58, 180, 1) 0%, rgba(253, 29, 29, 1) 33.3%, rgba(252, 176, 69, 1) 66.6%, rgba(131, 58, 180, 1) 100%);
      background-clip: text;
      -webkit-background-clip: text;
      color: transparent;
      background-size: 300% 100%;
      animation: text 4s infinite linear;
      font-size: 16px;
      font-weight: 500;
    }

    @keyframes text {
      0% {
        background-position: 0 0;
      }

      100% {
        background-position: -150% 0;
      }
    }
  }
}
</style>
