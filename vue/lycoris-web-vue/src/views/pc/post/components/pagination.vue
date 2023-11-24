<template>
  <div class="post-pagination" v-if="model.previous || model.next">
    <div class="card card-border-radius">
      <a :href="model.previous ? `/post/${model.previous.id}` : '/'">
        <div class="card-body">
          <div class="card-box">
            <el-image :src="model.previous?.icon || '/images/home-post-bg.jpg'" lazy></el-image>
            <div class="wrap"></div>
            <div class="frame">
              <div class="flex-center-center">
                <p>{{ model.previous?.title || '没有更早的文章了' }}</p>
                <span class="flex-start-center previous"> {{ model.previous ? 'Previous' : '返回首页' }}</span>
              </div>
            </div>
          </div>
        </div>
      </a>
    </div>
    <div class="card">
      <a :href="model.next ? `/post/${model.next.id}` : '/'">
        <div class="card-body">
          <div class="card-box">
            <el-image :src="model.next?.icon || '/images/home-post-bg.jpg'" lazy></el-image>
            <div class="wrap"></div>
            <div class="frame">
              <div class="flex-center-center">
                <p>{{ model.next?.title || '没有更新的文章了' }}</p>
                <span class="flex-start-center next"> {{ model.next ? 'Next' : '返回首页' }} </span>
              </div>
            </div>
          </div>
        </div>
      </a>
    </div>
  </div>
</template>

<script setup>
import { onMounted, reactive } from 'vue';
import { getPostPreviousAndNext } from '@/api/post';

const model = reactive({
  previous: void 0,
  next: void 0
});

const props = defineProps({
  id: {
    type: String,
    required: true,
    default: ''
  }
});

onMounted(async () => {
  if (props.id) {
    //
    let res = await getPostPreviousAndNext(props.id);
    if (res && res.resCode == 0) {
      model.previous = res.data.previous;
      model.next = res.data.next;
    }
  }
});
</script>

<style lang="scss" scoped>
.post-pagination {
  width: 100%;
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  grid-gap: 15px;
  margin: 20px 0;

  .card {
    position: relative;
    height: 300px;
    padding: 0px;
    overflow: hidden;

    @media (max-width: 1920px) {
      height: 260px;
    }

    .card-body {
      position: relative;
      height: 100%;
      width: 100%;

      padding: 0;

      .card-box {
        position: relative;
        height: 100%;
        width: 100%;
        background-color: #fff;
        overflow: hidden;
        cursor: pointer;

        .el-image {
          height: 100%;
          width: 100%;
          z-index: 1;

          :deep(img) {
            height: 100%;
            width: 100%;
            object-fit: cover;
            transition: transform 0.4s;
          }
        }

        .wrap {
          position: absolute;
          left: 0;
          top: 0;
          height: 100%;
          width: 100%;
          background-color: transparent;
          transition: all 0.4s;
          z-index: 2;
        }

        .frame {
          position: absolute;
          top: 50%;
          left: 50%;
          transform: translate(-50%, -50%);
          width: calc(100% - 150px);
          height: calc(100% - 150px);
          border-radius: 5px;
          transition: all 0.4s;
          z-index: 3;

          > div {
            position: relative;
            height: 100%;
            width: 100%;
            p {
              font-size: 24px;
              letter-spacing: 2px;
              padding: 10px 20px;
              background-color: #302f2fa1;
              border-radius: 5px;
              color: #fff;
              transition: all 0.4s;
            }

            > span {
              position: absolute;
              top: 0;
              display: none;
              font-size: 18px;
              color: #000;
              padding: 4px 3px;
            }

            span.previous {
              left: 0;
              background: var(--main-linear-gradient) no-repeat left bottom;
              background-size: 68% 3px;
            }

            span.next {
              right: 0;
              background: var(--main-linear-gradient) no-repeat right bottom;
              background-size: 68% 3px;
            }
          }
        }
      }

      .card-box:hover {
        opacity: 0.8;

        .el-image {
          :deep(img) {
            transform: scale(1.1);
          }
        }

        .wrap {
          backdrop-filter: blur(3.5px);
        }

        .frame {
          width: calc(100% - 60px);
          height: calc(100% - 60px);
          padding: 10px 15px;
          background-color: rgb(255 255 255 / 45%);

          > div {
            p {
              color: var(--color-purple);
              padding: 0;
              background-color: transparent;
              border-radius: 0px;
              color: #000;
            }

            > span {
              display: flex;
            }
          }
        }
      }
    }
  }
}
</style>
