<template>
  <div class="message-box">
    <div class="message-header">
      <div class="flex-start-center">
        <popover-user-info-card :user-id="props.data.user.id" :is-owner="props.data.isOwner">
          <template #reference>
            <el-image :src="props.data.user.avatar" :title="props.data.user.nickName" lazy>
              <template #error>
                <img :src="stores.webSetting.defaultAvatar" />
              </template>
            </el-image>
          </template>
        </popover-user-info-card>

        <span class="name">
          {{ props.data.user.nickName }}
          <el-tag v-if="props.data.isOwner" size="small">博主</el-tag>
        </span>

        <span class="time" v-if="props.data.createTime == '刚刚'">刚刚</span>
        <span class="time" v-else>发布于 {{ props.data.createTime }}</span>
      </div>
      <div class="flex-end-center">
        <span class="ip">{{ props.data.ipAddress == '未知' || props.data.ipAddress == '局域网' ? webSettings.privateIpAddress : props.data.ipAddress }}</span>
        <el-tooltip effect="dark" :content="`他/她 使用 ${userAgentName(props.data.agentFlag)} 发布了这条留言`" placement="top">
          <el-image class="agent" :src="userAgentIcon(props.data.agentFlag)" lazy></el-image>
        </el-tooltip>
      </div>
    </div>
    <div class="content">
      <p v-if="props.data.status == 0">{{ props.data.content }}</p>
      <span class="status-message status-danger" v-else-if="props.data.status == 1">该留言含有不良信息,可能涉及违规,为避免影响浏览,暂时不予展示</span>
      <span class="status-message status-default" v-else-if="props.data.status == 2">用户已删除留言</span>
      <span class="status-message status-default" v-else>该留言含有不良信息,可能涉及违规,为避免影响浏览,暂时不予展示</span>
    </div>
    <div class="redundancy" :class="{ 'empty-redundancy': !props.data.replyCount }" v-if="props.data.status == 0 || props.data.status == 2">
      <div :style="{ height: model.replidHeight }" v-if="props.data.redundancy && props.data.redundancy.length">
        <ul ref="redundancyRef">
          <!-- <transition-list> -->
          <li v-for="item in replidList" :key="item.id">
            <reply-message-data :data="item" @show-reply="showReply(props.data, item)"></reply-message-data>
          </li>
          <!-- </transition-list> -->
        </ul>
      </div>

      <div v-show="model.showMoreList" class="flex-center-center li-pagination">
        <el-pagination
          style="background-color: transparent"
          :current-page="reply.pageIndex"
          :page-size="reply.pageSize"
          layout="prev, pager, next"
          :total="props.data.replyCount"
          hide-on-single-page
          @current-change="pageChange"
        />
      </div>

      <div class="bottom-reply">
        <div>
          <div v-if="props.data.replyCount > 2">
            <div class="flex-start-center more-reply" @click="showMoreReplyComment">
              <el-icon>
                <component :is="model.showMoreList ? 'caretTop' : 'more-filled'"></component>
              </el-icon>
              <span>{{ model.showMoreList ? '收起回复' : `共 ${props.data.replyCount} 条回复` }}</span>
            </div>
          </div>
        </div>
        <div class="flex-center-center" @click="showReply(props.data)">
          <el-icon>
            <component :is="'chat-dot-round'"></component>
          </el-icon>
          <span>我也说一句</span>
        </div>
      </div>

      <div class="reply-body" :style="{ height: model.showReply ? '185px' : '0px' }">
        <div>
          <div style="height: 35px">
            <el-tag v-if="model.replyToName" style="margin: 5px 0">{{ model.replyToName }}</el-tag>
          </div>
          <el-input type="textarea" v-model="model.content" ref="inputRef" :autosize="{ minRows: 4, maxRows: 6 }" maxlength="100" show-word-limit> </el-input>
          <div style="text-align: right; padding: 8px 0">
            <el-button type="info" plain @click="model.showReply = false">不说了</el-button>
            <el-button type="warning" plain @click="clear">清空</el-button>
            <el-button type="primary" plain @click="replyMessage">发布</el-button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { reactive, ref, nextTick, computed, onMounted, inject } from 'vue';
import replyMessageData from './reply-message-data.vue';
import popoverUserInfoCard from '@/components/popover-user-card/index.vue';
import { getReplyMessageLsit, publishReplyMessage } from '@/api/message';
import { getUserAgentIconByEnum as userAgentIcon, getUserAgentNameByEnum as userAgentName } from '@/utils/user-agent';
import toast from '@/utils/toast';
import { stores } from '@/stores';
import { webSettings } from '@/config.json';

const redundancyRef = ref();
const inputRef = ref();
const loginModalRef = inject('$loginModal');

const model = reactive({
  showReply: false,
  replyToName: '',
  content: '',
  replyId: '',
  showMoreList: false,
  defaultHeight: '',
  replidHeight: ''
});

const reply = reactive({
  list: [],
  pageIndex: 1,
  pageSize: 5
});

const replidList = computed(() => {
  return model.showMoreList ? reply.list : props.data.redundancy;
});

const props = defineProps({
  data: {
    type: Object,
    required: true
  },
  frequency: {
    type: Number,
    required: true
  },
  lastPublish: {
    type: Number,
    required: true
  }
});

const emit = defineEmits(['reply', 'deleteMessage', 'updateRedundancy']);

onMounted(() => {
  if (redundancyRef.value) {
    model.defaultHeight = `${redundancyRef.value.scrollHeight}px`;
    model.replidHeight = model.defaultHeight;
  }
});

const getReplyList = async () => {
  try {
    let res = await getReplyMessageLsit({
      messageId: props.data.id,
      pageIndex: reply.pageIndex,
      pageSize: reply.pageSize
    });

    if (res && res.resCode == 0) {
      reply.list = res.data.list;
      if (reply.pageIndex == 1) {
        emit('updateRedundancy', res.data.list.slice(0, 2));
      }
    }
  } catch (error) {}
};

const showMoreReplyComment = async () => {
  if (!model.showMoreList && reply.list.length == 0) {
    await getReplyList();
  }

  model.showMoreList = !model.showMoreList;

  if (!model.showMoreList && model.showReply) {
    model.showReply = false;
  }

  nextTick(() => {
    if (model.showMoreList) {
      model.replidHeight = `${redundancyRef.value.scrollHeight}px`;
    } else {
      model.replidHeight = model.defaultHeight;
    }
  });
};

const pageChange = async pageIndex => {
  reply.pageIndex = pageIndex;
  await getReplyList();
  replidHeightcalc();
};

const showReply = (data, reply) => {
  if (reply) {
    model.replyId = reply.id;
    model.replyToName = `@ ${reply.user.nickName}`;
  } else {
    model.replyId = '';
    model.replyToName = '';
  }

  if (!model.showReply) {
    model.showReply = true;
  }

  nextTick(() => {
    inputRef.value.focus();
  });
};

const clear = () => {
  model.replyId = '';
  model.replyToName = '';
  model.content = '';
  model.inputMaxLength = 100;

  nextTick(() => {
    inputRef.value.focus();
  });
};

const replidHeightcalc = () => {
  nextTick(() => {
    if (!model.showMoreList) {
      model.defaultHeight = `${redundancyRef.value.scrollHeight}px`;
      model.replidHeight = model.defaultHeight;
    } else {
      model.replidHeight = `${redundancyRef.value.scrollHeight}px`;
    }
  });
};

const replyMessage = async () => {
  if (model.content) {
    if (!model.replyToName) {
      model.replyId = '';
    }

    if (!stores.user.state) {
      userLogin();
      return;
    }

    if (!stores.user.isAdmin) {
      let now = new Date().getTime();
      const lastTime = props.lastPublish + props.frequency;
      if (lastTime > now) {
        let time = Math.ceil((lastTime - now) / 1000);
        toast.warn(`留言发布有时间限制,请${time.toFixed(0)}秒后再发布留言`);
        return;
      }
    }

    let repliedUserId = '';
    if (model.replyId) {
      let item = [...(model.showMoreList ? reply.list : props.data.redundancy)].filter(x => x.id == model.replyId);
      if (item && item.length) {
        repliedUserId = item[0].user.id;
      }
    }

    try {
      let res = await publishReplyMessage(model.replyId || props.data.id, model.content, repliedUserId);
      if (res && res.resCode == 0) {
        if (reply.list.length > 0) {
          if (reply.list.length >= reply.pageSize) {
            reply.list.pop();
          }

          reply.list.unshift(res.data);
        }

        emit('reply', props.data.id, res.data, replidHeightcalc);

        clear();

        if (reply.pageIndex != 1) {
          reply.pageIndex = 1;
        }

        console.log(model.showMoreList);
        console.log(replidList);
      }
    } catch (error) {
      if (error && error.statusCode == 401) {
        userLogin();
      }
    }
  }
};

const userLogin = () => {
  toast.info('请先登录');
  loginModalRef.value.show();
};
</script>

<style lang="scss" scoped>
.message-box {
  .message-header {
    display: flex;
    justify-content: space-between;
    align-items: center;

    .flex-start-center {
      gap: 20px;
      height: 60px;

      $avatar-size: 55px;
      .el-image {
        height: $avatar-size;
        width: $avatar-size;
        display: flex;
        justify-content: center;
        align-items: center;

        :deep(img) {
          height: $avatar-size;
          width: $avatar-size;
          object-fit: cover;
          border-radius: 50%;
          border: 1px solid var(--color-secondary);
          cursor: pointer;
        }
      }

      span.name {
        font-size: 16px;
      }

      span.time {
        font-size: 12px;
        color: var(--color-dark-light);
      }

      > span {
        cursor: default;
      }
    }

    .flex-end-center {
      gap: 15px;

      $us-size: 30px;
      .agent {
        height: $us-size;
        width: $us-size;

        :deep(img) {
          height: $us-size;
          width: $us-size;
          object-fit: cover;
        }
      }

      .el-icon {
        cursor: pointer;
      }

      .ip {
        color: var(--color-dark-light);
      }

      .delete-message {
        color: var(--color-danger);
        transform: all 0.3s;
      }
      .delete-message:hover {
        color: var(--color-danger-light);
      }
    }
  }

  .content {
    padding: 20px 35px;
    > p {
      letter-spacing: 2px;
      line-height: 26px;
      color: var(--color-dark);
    }

    span.status-message {
      padding: 10px 20px;
      border-radius: 4px;
      letter-spacing: 2.5px;
    }

    .status-danger {
      background-color: var(--color-danger);
      color: #fff;
    }

    .status-default {
      background-color: var(--color-secondary);
      color: var(--color-dark-light);
    }
  }

  .redundancy {
    margin: 10px 0 10px 220px;
    background-color: #f4f5f8;
    border-radius: 11px;
    padding: 10px 15px;

    div {
      list-style: none;
    }

    div:first-child {
      overflow: hidden;
      transition: all 0.4s;
    }

    .bottom-reply {
      padding: 8px;
      display: flex;
      justify-content: space-between;
      align-items: center;

      .more-reply {
        padding-left: 20px;
        gap: 10px;
        color: var(--color-dark-light);
        cursor: pointer;
        transition: all 0.4s;
      }

      .more-reply:hover {
        color: var(--color-info);
      }

      > div:last-child {
        gap: 10px;
        color: var(--color-dark-light);
        font-size: 14px;
        cursor: pointer;
        transition: color 0.4s;
      }

      > div:last-child:hover {
        color: var(--color-info);
      }
    }

    .li-pagination {
      :deep(button),
      :deep(li) {
        background-color: transparent;
      }
    }
  }

  .redundancy.empty-redundancy {
    background-color: #fff;
  }

  .reply-body {
    padding: 0 !important;
    transition: all 0.3s;
    overflow: hidden;

    > div {
      padding: 8px;
    }

    .el-button {
      width: 100px;
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
