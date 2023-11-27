<template>
  <div class="chat-windows">
    <div class="chat-windows-title">
      <span>{{ props.chat.chatUserName }}</span>
      <span class="badge badge-purple" v-if="props.chat.isOwner">博主</span>
    </div>
    <ul ref="messageWindowsRef" @scroll="messageScroll">
      <li v-if="chatMessage.count > chatMessage.list.length">
        <div class="more-message"><span @click="loadMoreMessage">查看更多</span></div>
      </li>
      <li v-for="item in chatMessage.list" :key="item.id">
        <div v-if="stores.chatMessage.messages[props.chat.id].times[item.id]" class="message-time">
          <span>{{ stores.chatMessage.messages[props.chat.id].times[item.id] }}</span>
        </div>
        <div class="message-item" :class="{ self: item.user.id == stores.user.id }">
          <img :src="item.user.avatar" />
          <div class="message-body">
            <div class="message" v-html="item.content"></div>
          </div>
          <el-tooltip v-if="item.user.id == stores.user.id && item.ackError" class="box-item" effect="dark" :content="item.ackError" placement="top">
            <el-icon :size="22" class="message-error">
              <component :is="'warn-triangle-filled'"></component>
            </el-icon>
          </el-tooltip>
        </div>
      </li>
    </ul>
    <div class="new-message" v-show="model.newMessage > 0">
      <span @click="scrollBottom()">{{ model.newMessage > 1 ? `有${model.newMessage}条未读消息` : '有新消息' }}</span>
    </div>
    <loading-line v-if="model.listLoading" :loading="model.listLoading" :show-text="true" text="聊天记录加载中"></loading-line>
  </div>
</template>

<script setup>
import { reactive, nextTick, watch, ref, computed, onBeforeMount, onMounted } from 'vue';
import loadingLine from '../../../../components/loadings/loading-line.vue';
import { getChatMessageList } from '../../../../api/chat';
import { stores } from '../../../../stores';

const messageWindowsRef = ref();

const model = reactive({
  html: '',
  listLoading: false,
  submitType: 0,
  newMessage: 0
});

const chatMessage = computed(() => {
  return stores.chatMessage.messages[props.chat.id];
});

const props = defineProps({
  chat: {
    type: Object,
    required: true
  }
});

const emit = defineEmits(['init']);

watch(
  () => props.chat,
  async (value, oldValue) => {
    if (value.id === oldValue.id) {
      return;
    }

    if (!value || !value.id) {
      return;
    }

    stores.chatMessage._initRoomMessages(value.id);

    initRoomMessages();
  }
);

onBeforeMount(() => {
  if (!props.chat || !props.chat.id) {
    return;
  }
  stores.chatMessage._initRoomMessages(props.chat.id);
});

onMounted(() => {
  if (props.chat?.id) {
    initRoomMessages();
  }
});

const page = {
  pageIndex: 1,
  pageSize: 10
};

const getMessageList = async () => {
  const data = {
    count: 0,
    list: []
  };

  try {
    let res = await getChatMessageList({ roomId: props.chat.id, ...page });
    if (res && res.resCode == 0) {
      data.count = res.data.count;
      data.list = res.data.list;
    }
  } catch {}

  return data;
};

const initRoomMessages = async () => {
  // 清空未读消息数据
  stores.chat.clearUnreadMessage(props.chat.id);

  // 初始化页码
  page.pageIndex = 1;
  model.listLoading = true;

  try {
    if (chatMessage.value.pageIndex < page.pageIndex) {
      const result = await getMessageList();
      stores.chatMessage.setRoomMessageList(props.chat.id, result, page);

      emit('init', props.chat.id, result.list[result.list.length - 1].content || '');
    }
  } finally {
    model.listLoading = false;
  }

  showScroll();
};

const loadMoreMessage = async () => {
  page.pageIndex++;
  let result = {};

  if (chatMessage.value.pageIndex < page.pageIndex) {
    result = await getMessageList(false);
    console.log('接口中获取');
    stores.chatMessage.setRoomMessageList(props.chat.id, result, page);
  }

  if (result && result.list.length) {
    const scrollHeightBefore = messageWindowsRef.value.scrollHeight;

    nextTick(() => {
      const scrollHeightAfter = messageWindowsRef.value.scrollHeight;
      const scrollHeightDifference = scrollHeightAfter - scrollHeightBefore;
      messageWindowsRef.value.scrollTop += scrollHeightDifference;
    });
  }
};

const subscribeMessage = data => {
  if (data.roomId == props.chat.id) {
    if (messageWindowsRef.value.scrollHeight) {
      if (messageWindowsRef.value.scrollHeight === messageWindowsRef.value.scrollTop + messageWindowsRef.value.clientHeight) {
        scrollBottom();
      } else {
        model.newMessage++;
      }
    }
  }
};

const messageAckTimer = {};
const sendMessage = data => {
  stores.chatMessage.addRoomMessage(props.chat.id, data);

  scrollBottom();

  messageAckTimer[data.id] = setTimeout(() => {
    stores.chatMessage.updateErrorMessage(props.chat.id, data.id, '消息发送失败');
  }, 5000);
};

const messageAck = res => {
  if (res && res.data?.messageId && messageAckTimer[res.data.messageId]) {
    clearTimeout(messageAckTimer[res.data.messageId]);
  }

  if (res && res.resCode == 0) {
    stores.chatMessage.updateRoomMessage(res.data.roomId, res.data);
  } else {
    setTimeout(() => {
      stores.chatMessage.updateErrorMessage(props.chat.id, res.data.messageId, res.resMsg || '消息发送失败');
    }, 1500);
  }
  // scrollBottom();
};

const showScroll = rightNow => {
  if (!rightNow) {
    nextTick(() => {
      messageWindowsRef.value.scrollTop = messageWindowsRef.value.scrollHeight;
    });
  } else {
    messageWindowsRef.value.scrollTop = messageWindowsRef.value.scrollHeight;
  }
};

const messageScroll = () => {
  if (model.newMessage > 0 && messageWindowsRef.value.scrollHeight == messageWindowsRef.value.scrollTop + messageWindowsRef.value.clientHeight) {
    model.newMessage = 0;
  }
};

const scrollBottom = () => {
  nextTick(() => {
    messageWindowsRef.value.scrollTo({
      top: messageWindowsRef.value.scrollHeight,
      behavior: 'smooth'
    });
  });
};

defineExpose({
  subscribeMessage,
  sendMessage,
  messageAck,
  showScroll
});
</script>

<style lang="scss" scoped>
.chat-windows {
  height: var(--room-message-height);
  position: relative;
  overflow: hidden;

  .chat-windows-title {
    position: relative;
    height: 40px;
    padding: 5px 10px;
    font-size: 18px;
    background-color: #fff;
    box-shadow: 0px 0px 10px 2px #cacacb;

    .badge {
      margin-left: 5px;
      font-size: 12px;
    }
  }

  ul {
    height: calc(var(--room-message-height) - 60px);
    overflow-x: hidden;
    overflow-y: auto;
    padding-bottom: 15px;
    margin: 2px 0 10px 0;

    li {
      list-style: none;
      padding: 10px 15px;
      overflow: hidden;

      .more-message {
        text-align: center;
        > span {
          color: var(--color-primary);
          cursor: pointer;
        }

        > span:hover {
          color: var(--color-primary-light);
        }
      }

      .message-item {
        display: flex;
        justify-content: flex-start;
        align-items: flex-start;

        > img {
          height: 40px;
          width: 40px;
          border-radius: 10%;
          object-fit: cover;
          cursor: pointer;
        }

        .message-body {
          margin: 0 10px;
          --chat-message-width: 550px;
          :deep(.message) {
            position: relative;
            background-color: var(--color-secondary);
            padding: 10px 10px;
            border-radius: 8px;
            font-size: 16px;
            max-width: var(--chat-message-width);

            @media (max-width: 1920px) {
              --chat-message-width: 500px;
            }

            @media (max-width: 1440px) {
              --chat-message-width: 400px;
            }

            p {
              word-wrap: break-word;
            }

            img {
              max-width: calc(var(--chat-message-width) - 50px);
              object-fit: cover;
              border-radius: 5px;
            }
          }
        }

        .message-error {
          color: var(--color-danger);
          padding-top: 15px;
          cursor: pointer;
        }

        &.self {
          flex-direction: row-reverse;

          .message-user {
            justify-content: flex-end;

            span:nth-child(1) {
              padding-right: 10px;
            }
          }

          .message {
            background-color: var(--color-primary-light);
            color: #fff;
          }
        }
      }

      .message-time {
        margin: 15px 0;
        text-align: center;

        span {
          background-color: var(--color-secondary);
          padding: 5px 10px;
          border-radius: 5px;
        }
      }
    }
  }

  .new-message {
    position: absolute;
    bottom: 10px;
    left: 30%;
    span {
      background-color: var(--color-secondary);
      padding: 8px 10px;
      border-radius: 5px;
      cursor: pointer;
    }
  }
}
</style>
