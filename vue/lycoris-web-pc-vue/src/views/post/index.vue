<template>
  <div class="main">
    <transition enter-active-class="animate__animated animate__fadeIn" duration="200">
      <div v-show="post.show">
        <div class="post-header flex-start-center">
          <div class="title">
            <span class="text-showup">{{ post.title }}</span>
            <div class="update" v-if="stores.owner.email">
              <p v-if="post.days > 0">
                本文已超过 <span :style="{ color: post.days > 30 ? 'var(--color-danger)' : '' }">{{ post.days }}</span> 天没有更新。如图片等资源失效，请反馈至邮箱：
                <span @click="copyEmial">{{ stores.owner.email }}</span>
                谢谢！
              </p>
            </div>
          </div>
        </div>
        <div class="container">
          <div class="post" ref="postRef">
            <div class="card">
              <div class="detail-markdown">
                <markdown-container ref="markdown"></markdown-container>
              </div>

              <div class="detail-footer">
                <div class="detail-footer-left flex-start-center">
                  <div class="reward flex-center-center">
                    <span>赏</span>
                  </div>
                  <div class="tags flex-start-center" v-if="post.category || (post.tags && post.tags.length > 0)">
                    <el-icon style="margin-right: 5px">
                      <component :is="'sugar'"></component>
                    </el-icon>
                    <span>{{ post.category }}</span>
                    <span class="tag" v-for="tag in post.tags" :key="tag">{{ tag }}</span>
                  </div>
                </div>
                <div class="detail-footer-right flex-end-center">
                  <div class="read flex-center-center">
                    <el-icon :size="'16px'">
                      <component :is="'reading'"></component>
                    </el-icon>
                    <span class="read-count">{{ post.browse }}</span>
                  </div>
                  <div>发布于 {{ post.publishTime }}</div>
                </div>
              </div>

              <el-divider></el-divider>
              <copyright :title="post.title"></copyright>
            </div>

            <pagination :id="$route.params.postid"></pagination>

            <div class="card" v-if="post.comment">
              <comment v-if="post.comment" :id="$route.params.postid"></comment>
            </div>
          </div>

          <el-affix :offset="10" class="affix-width">
            <div>
              <author></author>
              <topic :title="post.title" :topics="post.topics"></topic>
            </div>
          </el-affix>
        </div>
      </div>
    </transition>

    <img-preview ref="imgPreviewRef"></img-preview>
  </div>
</template>

<script setup name="post">
import { onMounted, reactive, ref, defineAsyncComponent } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import markdownContainer from '@/components/markdown-editor/index.vue';

const copyright = defineAsyncComponent(() => import('./components/copyright.vue'));
const pagination = defineAsyncComponent(() => import('./components/pagination.vue'));
const comment = defineAsyncComponent(() => import('./components/comment.vue'));
const author = defineAsyncComponent(() => import('./components/author.vue'));
const topic = defineAsyncComponent(() => import('./components/topic.vue'));
const imgPreview = defineAsyncComponent(() => import('./components/img-preview.vue'));

import { stores } from '@/stores';
import { getPostDetail } from '@/api/post.js';
import $document from '@/utils/document';
import toast from '@/utils/toast';
import useClipboard from 'vue-clipboard3';

const router = useRouter();
const route = useRoute();

const { toClipboard } = useClipboard();

const postRef = ref();
const markdown = ref();
const imgPreviewRef = ref();

const post = reactive({
  title: '',
  category: '',
  tags: [],
  publishTime: '',
  days: 0,
  daysColor: '',
  browse: 0,
  comment: false,
  topics: [],
  show: false
});

const emit = defineEmits(['loading', 'browse']);

onMounted(async () => {
  try {
    let res = await getPostDetail(route.params.postid);
    if (res.resCode == 10) {
      toast.warn(res.resMsg);
      setTimeout(() => {
        router.back();
      }, 500);
      return;
    }

    if (res.resCode == 0) {
      $document.setTitle(res.data.title);

      post.title = res.data.title;
      post.category = res.data.category;
      post.tags = res.data.tags;
      post.publishTime = res.data.publishTime;
      post.days = res.data.days;
      post.browse = res.data.browse;
      post.comment = res.data.comment;

      markdown.value.init(res.data.markdown);
      post.topics = markdown.value.getTopic();

      markdown.value.setImgPreview(function () {
        console.log(this.src);
        imgPreviewRef.value.show(this.src);
      });
    }
  } finally {
    emit('loading', false);
    post.show = true;
    if (post.title) {
      emit('browse', { pageName: post.title, isPost: true });
    }
  }
});

const copyEmial = async () => {
  try {
    await toClipboard(stores.owner.email);
    toast.info(`复制成功,感谢您的反馈`);
  } catch (error) {}
};
</script>

<style lang="scss" scoped>
.main {
  height: 100%;
}

.post-header {
  padding: 40px 330px 40px 0;
  margin-bottom: 5px;

  .title {
    padding: 10px 10px;
    overflow: hidden;
    filter: contrast(30);

    > span {
      font-size: 30px;
      font-weight: 600;
      max-width: 800px;
      color: #fff;
      cursor: default;
    }

    > span::before {
      content: '#';
      padding-right: 5px;
      color: var(--color-danger);
    }
  }

  .update {
    text-align: center;
    padding-top: 10px;

    p {
      font-size: 16px;
      font-weight: 400;
      color: #fff;
      letter-spacing: 1.5px;
    }

    span {
      transition: color 0.3s;
      cursor: pointer;
    }

    span:hover {
      color: var(--color-purple);
    }
  }
}

$right-width: 320px;
.container {
  position: relative;
  display: grid;
  gap: 20px;
  grid-template-columns: minmax(0, 1fr) $right-width;

  .post {
    border-radius: 15px;
    margin-bottom: 20px;
    position: relative;
    overflow: hidden;

    .card {
      padding: 10px 30px;
      margin-bottom: 20px;
    }

    .card:first-child {
      padding-bottom: 25px;
    }

    .detail-markdown {
      position: relative;
    }

    .detail-footer {
      height: 60px;
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding: 0 5px;
      margin-top: 20px;
      font-size: 16px;
      font-weight: 500;

      .detail-footer-left {
        height: 100%;

        .reward {
          height: 45px;
          width: 45px;
          border-radius: 50%;
          border: 1px dashed var(--color-purple);
          cursor: pointer;

          span {
            font-size: 18px;
            font-weight: 500;
            transition: all 0.5s;
          }
        }

        .reward:hover {
          span {
            color: var(--color-danger);
          }
        }

        .tags {
          padding-left: 20px;

          .tag {
            font-size: 16px;
            font-weight: 500;
            color: var(--color-text);
            cursor: pointer;
            transition: all 0.5s;
          }

          .tag:after {
            content: '、';
            padding: 0 5px;
          }

          .tag:last-child:after {
            content: '';
            padding: 0;
          }

          a.tag:hover {
            color: var(--color-info);
          }
        }
      }

      .detail-footer-right {
        height: 100%;
        cursor: default;

        .read {
          .read-count {
            padding-left: 10px;
          }
        }

        .read::after {
          content: '/';
          padding: 0 10px;
        }
      }
    }
  }

  .affix-width {
    width: $right-width !important;
  }
}

@media (max-width: 1920px) {
  $right-width: 280px;
  .container {
    grid-template-columns: minmax(0, 1fr) $right-width;

    .affix-width {
      width: $right-width !important;
    }
  }
}
</style>
