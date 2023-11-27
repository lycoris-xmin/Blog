<template>
  <div>
    <div class="card post-topic" v-if="props.topics && props.topics.length > 0">
      <div class="topic-title">
        <el-icon>
          <component :is="'guide'"></component>
        </el-icon>
        <span>文章目录</span>
      </div>
      <ul>
        <li v-for="item in props.topics" :key="item.id">
          <div class="topic-item">
            <span @click="goTopic(item.id)" class="main" :title="item.text">{{ item.text }}</span>
            <div class="topic-item-child" v-for="child in item.child" :key="child.id">
              <span @click="goTopic(child.id)" class="child" :title="child.text">{{ child.text }}</span>
            </div>
          </div>
        </li>
      </ul>
    </div>

    <div class="card post-bottom-tool flex-start-center">
      <div class="go go-back flex-center-center" @click="goBack">
        <el-tooltip effect="dark" content="返回上一页" placement="bottom">
          <el-icon>
            <component :is="'back'"></component>
          </el-icon>
        </el-tooltip>
      </div>
      <div class="go go-top flex-center-center" @click="() => scrollPageTop()">
        <el-tooltip effect="dark" content="返回顶部" placement="bottom">
          <el-icon>
            <component :is="'promotion'"></component>
          </el-icon>
        </el-tooltip>
      </div>
      <div class="go go-home flex-center-center">
        <router-link to="/" class="flex-center-center">
          <el-tooltip effect="dark" content="返回首页" placement="bottom">
            <el-icon>
              <component :is="'home-filled'"></component>
            </el-icon>
          </el-tooltip>
        </router-link>
      </div>
      <div class="go go-share flex-center-center" @click="shareLinkCopy()">
        <el-tooltip effect="dark" content="文章分享" placement="bottom">
          <el-icon>
            <component :is="'share'"></component>
          </el-icon>
        </el-tooltip>
      </div>
    </div>
  </div>
</template>

<script setup>
import { useRouter } from 'vue-router';
import useClipboard from 'vue-clipboard3';
import toast from '@/utils/toast';
import { scrollPageTop } from '@/utils/tool';

const router = useRouter();
const { toClipboard } = useClipboard();

const props = defineProps({
  title: {
    type: String,
    required: true,
    default: ''
  },
  topics: {
    type: Array,
    default: () => {
      return [];
    }
  }
});
const goBack = () => {
  router.back();
};

const goTopic = id => {
  let dom = document.getElementById(id);
  dom.scrollIntoView({ block: 'start', behavior: 'smooth' });

  let domInterval = setInterval(() => {
    if (dom.scrollHeight - dom.scrollTop == dom.clientHeight) {
      dom.classList.add('topic-active');
      setTimeout(() => {
        dom.classList.remove('topic-active');
      }, 1000);

      clearInterval(domInterval);
    }
  }, 200);
};

const shareLinkCopy = async () => {
  try {
    let content = `${props.title}
${window.location.href}
作者：Lycoris - 程序猿的小破站`;
    await toClipboard(content);
    toast.success(`文章链接复制成功,感谢您的分享`);
  } catch (error) {}
};
</script>

<style lang="scss" scoped>
.post-topic {
  margin-bottom: 15px;
  padding-top: 20px;

  .topic-title {
    font-size: 20px;
    background: var(--main-linear-gradient) no-repeat left bottom;
    background-size: 38% 3px;
    cursor: default;

    .el-icon {
      margin-right: 10px;
    }

    span {
      letter-spacing: 3px;
    }

    margin-bottom: 20px;
  }

  li {
    list-style: none;

    .topic-item {
      display: flex;
      flex-direction: column;

      .topic-item-child {
        display: flex;
        flex-direction: column;
      }

      span {
        padding: 5px;
        border-radius: 4px;
        cursor: pointer;
        transition: all 0.4s;

        font-size: 15px;
        line-height: 25px;
        overflow: hidden;
        text-overflow: ellipsis;
        word-wrap: break-word;
        white-space: nowrap;
      }

      span:hover {
        color: var(--color-danger);
        background-color: var(--color-secondary);
      }

      span.child {
        padding-left: 15px;
      }

      span::before {
        content: '&';
        color: var(--color-danger);
        padding-right: 3px;
      }
    }
  }
}

.post-bottom-tool {
  height: 45px;
  width: 100%;

  .go {
    height: 100%;
    width: 100%;
    cursor: pointer;

    .el-icon {
      font-size: 20px;
      transition: all 0.3s;
    }
  }

  .go:hover {
    :deep(.el-icon) {
      color: var(--color-info);
      transform: scale(1.2);
    }
  }

  .go-back {
    flex: 1;
    border-right: 1px solid var(--color-secondary);
  }

  .go-top {
    flex: 1;
    border-right: 1px solid var(--color-secondary);
    align-items: center;
    color: var(--color-danger);
  }

  .go-home {
    flex: 1;
    border-right: 1px solid var(--color-secondary);

    a {
      height: 100%;
      width: 100%;
      color: var(--color-primary);
    }
  }

  .go-share {
    flex: 1;
    align-items: center;
    color: var(--color-warning);
  }
}

@media (max-width: 1920px) {
  .post-topic {
    li {
      .topic-item {
        span {
          font-size: 14px;
          line-height: 20px;
        }

        span.child {
          padding-left: 15px;
        }
      }
    }
  }
}
</style>

<style lang="scss">
.topic-active {
  animation: topic-scale 0.5s linear forwards;
}

@keyframes topic-scale {
  0% {
    background-color: var(--color-danger);
  }

  75% {
    background-color: var(--color-danger-light);
  }

  100% {
    background-color: transparent;
  }
}
</style>
