<template>
  <div class="post">
    <div class="icon flex-center-center">
      <el-image :src="props.post.icon" lazy>
        <template #error>
          <img src="/images/404.png" />
        </template>
      </el-image>
    </div>
    <div class="content">
      <div class="flex-start-center title">
        <span class="badge badge-success" v-if="props.post.new">最新</span>
        <span class="badge badge-purple" v-if="props.post.type == 0">原创</span>
        <span class="badge badge-info" v-else>转载</span>

        <router-link :to="`/post/${props.post.id}`" :title="props.post.title">
          {{ props.post.title }}
        </router-link>
      </div>
      <div class="info">
        <div class="info-content">
          {{ props.post.info }}
        </div>
      </div>
      <div class="attr">
        <div class="attr-item">
          <span class="post-category" :class="{ 'none-category': !props.post.categoryName }">{{ props.post.categoryName ? props.post.categoryName : '未分类' }}</span>
        </div>

        <div class="attr-item" v-if="props.post.tags && props.post.tags.length">
          <span class="post-tags" v-for="item in props.post.tags" :key="item">{{ item }}</span>
        </div>

        <div class="attr-item publish-time">
          <el-icon>
            <component :is="'stopwatch'"></component>
          </el-icon>
          <span>{{ props.post.publishTime }}</span>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
const props = defineProps({
  post: {
    type: Object,
    required: true,
    default: void 0
  }
});
</script>

<style lang="scss" scoped>
.post {
  padding: 15px;
  border-radius: 13px;
  cursor: default;
  display: grid;
  grid-template-columns: 210px 1fr;
  transition: all 0.3s;

  .icon {
    height: 140px;
    width: 210px;
    flex-shrink: 0;

    .el-image {
      height: 140px;
      width: 210px;
      border: 1px solid var(--color-secondary);
      border-radius: 8px;
      overflow: hidden;
      transition: all 0.3s;

      :deep(img) {
        height: 140px;
        width: 210px;
      }
    }
  }

  .content {
    position: relative;
    padding: 5px 0px 5px 20px;
    overflow: hidden;

    .title {
      font-size: 20px;
      padding-bottom: 10px;

      a {
        color: var(--post-title-color);
        transition: all 0.4s;
        display: block;
        overflow: hidden;
        text-overflow: ellipsis;
        white-space: nowrap;
      }

      .badge {
        padding: 0px 8px;
        flex-shrink: 0;
        font-size: 13px;
        margin-right: 6px;
      }
    }

    .info {
      min-height: 68px;

      .info-content {
        font-size: 14px;
        color: var(--post-info-color);

        overflow: hidden;
        word-break: break-all;
        text-overflow: ellipsis;
        display: -webkit-box;
        -webkit-box-orient: vertical;
        -webkit-line-clamp: 3;
      }
    }

    .attr {
      width: 100%;
      padding: 10px 0 0 0;
      color: var(--post-info-color);
      font-size: 14px;
      display: flex;
      align-items: center;
      $hover-color: var(--color-primary);

      .attr-item {
        .post-category {
          cursor: pointer;
          color: var(--post-info-color);
          transition: all 0.5s;
        }

        .post-category:hover {
          color: $hover-color;
        }

        .none-category {
          color: var(--color-danger);
        }

        .post-tags {
          cursor: pointer;
          margin-right: 4px;
          transition: all 0.5s;
        }

        .post-tags:last-child {
          margin-right: 0px;
        }

        .post-tags::after {
          content: ',';
          padding: 0 2px;
        }

        .post-tags:last-child::after {
          content: '';
        }

        .post-tags:hover {
          color: $hover-color;
        }
      }

      .attr-item::after {
        content: '/';
        padding: 0 10px;
      }

      .attr-item:last-child::after {
        content: '';
      }

      .attr-item.publish-time {
        display: flex;
        align-items: center;

        .el-icon {
          margin-right: 8px;
        }
      }
    }
  }
}

.post:hover {
  background-color: var(--color-secondary);

  .icon {
    .el-image {
      border: 1px solid var(--color-info-light);
    }
  }

  .content {
    .title {
      a {
        color: var(--post-title-hover-color);
      }
    }
  }
}

@media (max-width: 1920px) {
  .post {
    .content {
      .info {
        min-height: 48px;
        font-size: 14px;
        -webkit-line-clamp: 2; /** 显示的行数 **/
      }
    }
  }
}
</style>
