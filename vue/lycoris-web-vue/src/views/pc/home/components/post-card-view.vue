<template>
  <div class="view-card" :style="{ height: props.height }" :class="{ 'view-col': props.flexDirection == 'column' }" v-if="props.data">
    <div class="view-bg">
      <img :src="props.data.icon" />
    </div>
    <div class="view-card-container">
      <router-link :to="`/post/${props.data.id}`">
        <div class="post-title flex-center-center">
          <el-tag>推荐</el-tag>
          <span>{{ props.data.title }}</span>
        </div>
      </router-link>
      <div class="post-info">
        <div class="info">
          {{ props.data.info }}
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
const props = defineProps({
  height: {
    type: String,
    default: '100%'
  },
  width: {
    type: String,
    default: '100%'
  },
  flexDirection: {
    type: String,
    default: 'row'
  },
  data: {
    type: Object,
    required: true,
    default: void 0
  }
});
</script>

<style lang="scss" scoped>
.view-card {
  background-color: #fff;
  width: 100%;
  border-radius: 10px;
  overflow: hidden;

  position: relative;
  display: flex;
  align-items: stretch;

  .view-bg {
    flex: 1;
    position: relative;
    height: 100%;
    width: 100%;

    img {
      height: 100%;
      width: 120%;
      object-fit: cover;
      display: block;
      z-index: 20;
    }
  }

  .view-bg::after {
    pointer-events: none;
    content: '';
    position: absolute;
    z-index: 35;
    left: 50%;
    top: 0;
    height: 100%;
    width: 50%;
    background: var(--gradient-cover);
  }

  .view-card-container {
    flex: 1;
    width: 100%;
    background-color: var(--main-background-light-color);
    z-index: 40;
    padding: 40px 20px;
    display: flex;
    flex-direction: column;
    justify-content: center;
    align-items: flex-start;
    font-size: 16px;

    a {
      flex: 1.5;

      .post-title {
        font-size: 26px;
        margin-bottom: 15px;

        .el-tag {
          font-size: 16px;
          background-color: var(--color-danger);
          margin: 0 10px;
          color: #fff;
          border: 1px solid var(--color-danger-light);
        }

        span {
          word-wrap: break-word;
          text-overflow: ellipsis;
          overflow: hidden;
          color: var(--post-title-color);
          transition: all 0.3s;
        }

        span:hover {
          color: var(--post-title-hover-color);
        }
      }
    }

    .post-info {
      flex: 6;
      overflow: hidden;
      height: 60%;

      .info {
        color: var(--post-info-color);
        line-height: 40px;
        letter-spacing: 3.5px;

        overflow: hidden;
        word-break: break-all;
        text-overflow: ellipsis;
        display: -webkit-box;
        -webkit-box-orient: vertical;
        -webkit-line-clamp: 6;
      }
    }
  }
}

.view-card.view-col {
  flex-direction: column;

  .view-bg {
    flex-shrink: 0;
    height: 45%;

    img {
      height: 120%;
      width: 100%;
      display: block;
      z-index: 20;
    }
  }

  .view-bg::after {
    pointer-events: none;
    content: '';
    position: absolute;
    z-index: 35;
    left: 0;
    top: 50%;
    height: 50%;
    width: 100%;
    background: linear-gradient(
      180deg,
      hsla(0, 0%, 98%, 0) 0,
      hsla(0, 0%, 98%, 0.013) 8.1%,
      hsla(0, 0%, 98%, 0.049) 15.5%,
      hsla(0, 0%, 98%, 0.104) 22.5%,
      hsla(0, 0%, 98%, 0.175) 29%,
      hsla(0, 0%, 98%, 0.259) 35.3%,
      hsla(0, 0%, 98%, 0.352) 41.2%,
      hsla(0, 0%, 98%, 0.45) 47.1%,
      hsla(0, 0%, 98%, 0.55) 52.9%,
      hsla(0, 0%, 98%, 0.648) 58.8%,
      hsla(0, 0%, 98%, 0.741) 64.7%,
      hsla(0, 0%, 98%, 0.825) 71%,
      hsla(0, 0%, 98%, 0.896) 77.5%,
      hsla(0, 0%, 98%, 0.951) 84.5%,
      hsla(0, 0%, 98%, 0.987) 91.9%,
      #fff
    );
  }

  .view-card-container {
    flex-shrink: 0;
    height: 50%;
    padding: 10px 20px 20px 40px;

    .post-info {
      .info {
        -webkit-line-clamp: 3;
      }
    }
  }
}
</style>
