<template>
  <div class="comments-container">
    <div class="flex-start-center">
      <div class="comment-title">
        <el-icon>
          <component :is="'chat-line-square'"></component>
        </el-icon>
        <span>文章评论</span>
      </div>
    </div>
    <div class="comment-body">
      <div class="comment-group" ref="groupRef">
        <transition-list class="comment">
          <li v-for="item in comments.list" :key="item.id">
            <div class="header">
              <div class="user flex-start-center">
                <div class="avatar">
                  <popover-user-info-card :user-id="item.user.id" :is-owner="item.isOwner">
                    <template #reference>
                      <el-image :src="item.user.avatar" lazy></el-image>
                    </template>
                    <div class="user-info-card">123</div>
                  </popover-user-info-card>
                </div>
                <span>{{ item.user.nickName }}<el-tag style="margin-left: 5px" v-if="item.isOwner">站长</el-tag></span>
                <span class="time">{{ item.createTime }}</span>
              </div>
              <div class="other flex-end-center">
                <span class="ip">{{ item.ipAddress == '局域网' ? webSettings.privateIpAddress : item.ipAddress }}</span>
                <el-tooltip effect="dark" :content="`他/她 使用 ${userAgentName(item.agentFlag)} 发布了这条评论`" placement="top">
                  <el-image :src="userAgentIcon(item.agentFlag)" lazy></el-image>
                </el-tooltip>

                <el-dropdown v-if="stores.user.id != item.user.id">
                  <el-icon :size="20">
                    <component :is="'more'"></component>
                  </el-icon>
                  <template #dropdown>
                    <el-dropdown-menu>
                      <el-dropdown-item>
                        <p class="dropdown-action" @click="replyTo(item)">@他/她</p>
                      </el-dropdown-item>
                      <el-dropdown-item v-if="!item.isOwner">
                        <p class="dropdown-action" @click="commentReport(item)">举报</p>
                      </el-dropdown-item>
                    </el-dropdown-menu>
                  </template>
                </el-dropdown>
              </div>
            </div>
            <div class="content">
              <span class="reply-user" v-if="item.repliedUser">
                <popover-user-info-card :user-id="item.repliedUserId" trigger="click">
                  <span class="reply-user-text">@{{ item.repliedUser }}</span>
                </popover-user-info-card>
              </span>
              {{ item.content }}
            </div>
          </li>
        </transition-list>
        <div class="empty flex-center-center" v-if="comments.list.length == 0">该文章还没有评论！</div>
        <loading-line :loading="comments.loading" :text="'文章评论加载中...'" :show-text="true"></loading-line>
      </div>

      <el-pagination
        layout="prev, pager, next"
        :page-size="comments.pageSize"
        :total="comments.count"
        :current-change="comments.pageIndex"
        style="justify-content: center"
        @current-change="pageChange"
        hide-on-single-page
      />

      <div class="publich-comment">
        <span style="display: block; height: 35px; min-width: 1px">
          <el-tag v-if="model.repliedUserName" closable @close="handleCloseReply">@{{ model.repliedUserName }}</el-tag>
        </span>
        <div style="position: relative">
          <el-input
            ref="inputRef"
            v-model="model.content"
            type="textarea"
            :autosize="{ minRows: 5 }"
            maxlength="100"
            show-word-limit
            :placeholder="model.repliedUserName ? `回复 ${model.repliedUserName} 的评论` : model.placeholder"
          ></el-input>
          <div v-if="!stores.user.state" class="to-login-wrap">
            <el-button type="info" @click="toLogin">请先登录</el-button>
          </div>
        </div>
        <div class="publich-comment-action flex-end-center" v-if="stores.user.state">
          <el-button type="primary" @click="publish" :loading="model.loading">发布评论</el-button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { reactive, onMounted, ref, nextTick, inject } from 'vue';
import popoverUserInfoCard from '../../../components/popover-user-card/index.vue';
import loadingLine from '../../../components/loadings/loading-line.vue';
import transitionList from '../../../components/transitions/list-transition.vue';
import { getCommentList, publishComment } from '../../../api/comment';
import { getUserAgentIconByEnum as userAgentIcon, getUserAgentNameByEnum as userAgentName } from '../../../utils/user-agent';
import toast from '../../../utils/toast';
import { webSettings } from '../../../config.json';
import { stores } from '../../../stores';

const inputRef = ref();
const groupRef = ref();
const loginModalRef = inject('$loginModal');

const model = reactive({
  content: '',
  placeholder: '站长由于没钱，所以数据库买的比较小，字数要做限制，不然数据太多，钱包撑不住',
  repliedUserId: '',
  repliedUserName: '',
  loading: false
});

const props = defineProps({
  id: {
    type: String,
    required: true,
    default: ''
  }
});

const comments = reactive({
  count: 0,
  list: [],
  pageIndex: 1,
  pageSize: 5,
  loading: false
});

onMounted(async () => {
  await getList();
});

const getList = async () => {
  comments.loading = true;
  try {
    let res = await getCommentList({
      postId: props.id,
      pageIndex: comments.pageIndex,
      pageSize: comments.pageSize
    });

    if (res && res.resCode == 0) {
      comments.count = res.data.count;
      comments.list = res.data.list;
    }
  } finally {
    comments.loading = false;
  }
};

const pageChange = index => {
  comments.pageIndex = index;
  getList();
};

const replyTo = item => {
  model.repliedUserId = item.user.id;
  model.repliedUserName = item.user.nickName;
  nextTick(() => {
    inputRef.value.focus();
  });
};

const handleCloseReply = () => {
  model.repliedUserId = '';
  model.repliedUserName = '';
};

const publish = async () => {
  if (model.content) {
    if (!stores.user.state) {
      toast.info('请先登录');
      loginModalRef.value.show();
      return;
    }

    model.loading = true;
    try {
      let data = {
        postId: props.id,
        content: model.content
      };

      if (model.repliedUserId) {
        data.repliedUserId = model.repliedUserId;
      }

      let res = await publishComment(data);
      if (res && res.resCode == 0) {
        //
        if (comments.list.length >= comments.pageSize) {
          comments.list.pop();
        }

        comments.list.unshift(res.data);

        model.content = '';
        toast.success('发布成功');
      }
    } finally {
      model.loading = false;
    }
  }
};

const commentReport = item => {
  //
  console.log(item);
  toast.warn('功能还未开发');
};

const toLogin = () => {
  loginModalRef.value.show();
};
</script>

<style lang="scss" scoped>
.comments-container {
  padding: 20px 10px 50px 10px;
  //

  .comment-title {
    font-size: 20px;
    background: var(--main-linear-gradient) no-repeat left bottom;
    background-size: 78% 3px;
    cursor: default;

    .el-icon {
      margin-right: 10px;
    }

    span {
      letter-spacing: 3px;
    }

    margin-bottom: 20px;
  }

  .comment-group {
    position: relative;
    transition: all 0.5s;

    .comment {
      padding: 8px 0;

      > li {
        list-style: none;
        padding-bottom: 25px;
      }

      .header {
        display: flex;
        justify-content: space-between;
        align-items: center;
        margin: 0 10px;
        padding-bottom: 8px;
        border-bottom: 2px dashed var(--color-secondary);

        .user {
          gap: 15px;

          cursor: default;

          $avatar-size: 50px;
          .avatar {
            height: $avatar-size;
            width: $avatar-size;
            border-radius: 50%;
            border: 1px solid var(--color-secondary);

            .el-image {
              height: $avatar-size;
              width: $avatar-size;
              border-radius: 50%;
              cursor: pointer;

              :deep(img) {
                height: $avatar-size;
                width: $avatar-size;
                object-fit: cover;
                border-radius: 50%;
              }
            }
          }

          .time {
            font-size: 12px;
            color: var(--color-dark-light);
          }
        }

        .other {
          cursor: default;

          $us-size: 30px;

          > span {
            font-size: 14px;
            padding-right: 20px;
            color: var(--color-dark-light);
          }

          .el-image {
            height: $us-size;
            width: $us-size;

            :deep(img) {
              height: $us-size;
              width: $us-size;
            }
          }

          .el-icon {
            margin-left: 15px;
            transition: all 0.4s;
            cursor: pointer;
          }

          .el-icon:hover {
            color: var(--color-info);
          }

          .ip {
            color: var(--color-dark-light);
          }
        }
      }

      .content {
        padding: 10px 15px;
        letter-spacing: 2.5px;
        font-size: 16px;
        line-height: 25px;
        color: var(--color-dark);
        overflow: hidden;
        text-overflow: ellipsis;
        word-break: break-all;

        .reply-user {
          position: relative;
          span.reply-user-text {
            color: var(--color-primary);
            padding-right: 8px;
            transition: all 0.4s;
            cursor: pointer;
          }

          span.reply-user-text:hover {
            text-decoration: underline;
          }

          .reply-user-card {
            position: absolute;
            top: -100%;
            left: 50%;
            transform: translate(-50%, -100%);
            background-color: #fff;
            border-radius: 8px;
            padding: 10px;
            box-shadow: 0 0 10px 8px var(--color-secondary);
          }
        }
      }
    }

    .empty {
      height: 150px;

      font-size: 20px;
      letter-spacing: 2.5px;
      cursor: default;
    }
  }

  .publich-comment {
    position: relative;
    padding: 5px 20px 10px 20px;
    display: flex;
    flex-direction: column;

    .el-tag {
      margin: 5px 0;
      font-size: 14px;
      padding-top: 5px;
      padding-bottom: 5px;
    }

    .publich-comment-action {
      padding: 10px 0;
    }

    .to-login-wrap {
      position: absolute;
      left: 0;
      right: 0;
      top: 0;
      bottom: 0;
      background-color: rgb(228 231 234 / 50%);
      display: flex;
      justify-content: center;
      align-items: center;
    }
  }
}

p.dropdown-action {
  padding: 3px 5px;
  transition: all 0.3s;
}

p.dropdown-action:hover {
  color: var(--color-info);
}
</style>
