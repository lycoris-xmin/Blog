<template>
  <div class="feature">
    <div class="main">
      <el-carousel :interval="6000">
        <el-carousel-item v-for="item in carousel" :key="item.id">
          <div class="top-post-item">
            <div class="post-bg">
              <img :src="item.icon" />
            </div>
            <div class="top-post">
              <router-link :to="`/post/${item.id}`">
                <div class="top-post-title flex-start-center">
                  <el-tag v-if="item.recommend">推荐</el-tag>
                  <el-tag v-else class="primary">最新</el-tag>
                  <span>{{ item.title }}</span>
                </div>
              </router-link>
              <div class="top-post-info">
                <div class="info">
                  {{ item.info }}
                </div>
              </div>
            </div>
          </div>
        </el-carousel-item>
      </el-carousel>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import { getRecommendPostList } from '@/api/post';
import { stores } from '@/stores';

const carousel = ref([]);

const props = defineProps({
  loadIndex: {
    type: Number,
    required: true
  }
});

const emit = defineEmits(['loadComplete']);

onMounted(async () => {
  try {
    await getCarousel();
  } finally {
    emit('loadComplete', props.loadIndex);
  }
});

const getCarousel = async () => {
  let res = await getRecommendPostList();
  if (res && res.resCode == 0) {
    if (res.data && res.data.list && res.data.list.length) {
      carousel.value = res.data.list;
      carousel.value.forEach(x => (x.icon = stores.postIcon.getRandomPostIcon()));
    } else {
      carousel.value = [
        {
          id: '0',
          info: '',
          recommend: false,
          title: '网站初始化'
        }
      ];
    }

    Object.freeze(carousel);
  }
};
</script>

<style lang="scss" scoped>
.feature {
  padding-top: 40px;

  .main {
    width: 100%;

    $carousel-height: 430px;
    .el-carousel {
      border-color: transparent;
      box-shadow: 0 8px 8px 0 rgba(48, 55, 66, 0.15);
      overflow: hidden;
      border-radius: 10px;

      :deep(.el-carousel__container) {
        height: $carousel-height;

        .el-carousel__item {
          border-radius: 10px;
        }
      }

      :deep(.el-carousel__indicators) {
        .el-carousel__button {
          background-color: var(--color-dark);
        }
      }

      :deep(.is-active) {
        .el-carousel__button {
          background-color: var(--color-danger);
        }
      }
    }

    .top-post-item {
      position: relative;
      height: $carousel-height;
      display: flex;
      align-items: center;

      .post-bg {
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

      .post-bg::after {
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

      .top-post {
        flex: 1;
        height: 100%;
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

          .top-post-title {
            font-size: 26px;
            margin-bottom: 15px;

            .el-tag {
              font-size: 16px;
              background-color: var(--color-danger);
              margin: 0 10px;
              color: #fff;
              border: 1px solid var(--color-danger-light);
            }

            .el-tag.primary {
              background-color: var(--color-info);
              border: 1px solid var(--color-info-light);
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

        .top-post-info {
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

        .top-post-time {
          flex: 1;
          width: 100%;
          gap: 20px;
          padding-right: 40px;
          cursor: default;

          span {
            color: var(--post-info-color);
          }

          img {
            height: 35px;
            width: 35px;
            border-radius: 50%;
            object-fit: cover;
            border: 1px solid var(--post-info-color);
          }

          .el-icon {
            color: var(--post-info-color);
          }
        }
      }
    }
  }
}

@media (max-width: 1920px) {
  .feature {
    .main {
      $carousel-height: 350px;
      .el-carousel {
        :deep(.el-carousel__container) {
          height: $carousel-height;
        }
      }

      .top-post-item {
        height: $carousel-height;

        .top-post {
          .top-post-info {
            .info {
              line-height: 35px;
              letter-spacing: 2.5px;

              -webkit-line-clamp: 5;
            }
          }
        }
      }
    }
  }
}
</style>
