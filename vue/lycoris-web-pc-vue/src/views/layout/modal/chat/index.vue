<template>
  <el-dialog v-model="model.visible" title="消息中心(Beta测试，若有bug,请各位大佬担待)" append-to-body :close-on-click-modal="false" class="chat-modal" @open="handleDialogOpen" @opened="handleDialogOpened">
    <div class="chat-container">
      <transition-list class="chat-room-list">
        <li v-for="item in model.list" :key="item.id" @click="selectRoom(item)">
          <div class="room" :class="{ active: item.id == model.active.id }">
            <div class="flex-start-center">
              <img class="room-icon" :src="item.chatUserAvatar" />
              <div class="room-info">
                <span class="room-name">{{ item.chatUserName }}</span>
                <div class="room-last-message">
                  <div v-if="item.lastMessage">
                    <span>{{ item.lastMessage }}</span>
                  </div>
                </div>
              </div>
            </div>
            <div class="flex-center-center">
              <el-icon size="20" @click.stop="handleDelete(item)">
                <component :is="'close'"></component>
              </el-icon>
              <span class="room-active-time">{{ changeRoomLastActiveTime(item.lastActiveTime) }}</span>
              <p class="room-active-message">
                <span v-if="stores.chat.unreadMessage[item.id]">{{ stores.chat.unreadMessage[item.id] > 99 ? '99+' : stores.chat.unreadMessage[item.id] }}</span>
              </p>
            </div>
          </div>
        </li>
      </transition-list>
      <div class="chat-room">
        <chat-message ref="chatMessageRef" :chat="model.active" @init="chatMessageInit" @read="messageRead"></chat-message>
        <chat-edit-bar ref="chatEditBarRef" :room-id="model.active?.id" @submit-message="content => hanldeSubmitMessage(content)"></chat-edit-bar>
      </div>
    </div>
  </el-dialog>
</template>

<script setup>
import { onMounted, onUnmounted, reactive, ref, inject } from 'vue';
import transitionList from '../../../../components/transitions/list-transition.vue';
import chatMessage from './chat-message.vue';
import chatEditBar from './chat-edit-bar.vue';
import { stores } from '../../../../stores';
import { refreshToken } from '../../../../api/authentication';
import { getChatRoomList } from '../../../../api/chat';
import notification from '../../../../utils/notification';
import { uuid } from '../../../../utils/tool';

const chatMessageRef = ref();
const chatEditBarRef = ref();
const signalR = inject('$chat-signalR');

const model = reactive({
  visible: false,
  count: 0,
  list: [],
  active: null,
  unreadCount: {}
});

onMounted(async () => {
  getRoomList();

  try {
    // 初始化signalRl连接
    await signalR.setupSignalR('/hub/chat');

    // 登录状态
    subscribeAuthroization();
    // 刷新登录状态
    subscribeRefreshToken();
    // 聊天室
    subscribeChatRoom();
    // 聊天消息
    subscribeMessage();
    // 发送的聊天消息确认
    subscribeMessageAck();
    // 登出
    subscribeLogout();
  } catch (error) {
    console.error('signalR connect failed：', error);
  }
});

onUnmounted(() => {
  signalR.stop();
});

const roomPage = {
  pageIndex: 1,
  pageSize: 10
};

const getRoomList = async () => {
  try {
    let res = await getChatRoomList({ ...roomPage });
    if (res && res.resCode == 0) {
      model.count = res.data.count;
      model.list = res.data.list;

      for (let item of model.list) {
        stores.chat.additionUnreadMessage(item.id, item.unreadCount);
      }
    }
  } finally {
    if (model.list.length) {
      selectRoom(model.list[0]);
    }
  }
};

const subscribeAuthroization = () => {
  signalR.subscribe('authroization', () => {
    signalR.invoke('userAuthroization', stores.authorize.token);
  });
};

const subscribeRefreshToken = () => {
  signalR.subscribe('refreshToken', async () => {
    let data = await refreshToken(stores.authorize.refreshToken);
    if (data) {
      stores.authorize.setUserLoginState(data);
    } else {
      signalR.stop();
      stores.authorize.setUserLogoutState();
    }
  });
};

const subscribeChatRoom = () => {
  signalR.subscribe('chatRoom', data => {
    console.log(data);
    if (model.list.findIndex(x => x.id == data.id) == -1) {
      model.list.unshift(data);
      selectRoom(data);
    }
  });
};

const subscribeMessage = () => {
  signalR.subscribe('message', res => {
    if (!res || res.resCode != 0) {
      //
      return;
    }

    const index = model.list.findIndex(x => x.id == res.data.roomId);
    if (!model.visible) {
      showNotice(res.data);
      selectRoom(model.list[index]);
    }

    if (chatMessageRef.value) {
      stores.chatMessage.addRoomMessage(res.data.roomId, res.data);
      chatMessageRef.value.subscribeMessage(res.data);
    }

    stores.chat.additionUnreadMessage(res.data.roomId, 1);

    if (index > -1) {
      const item = model.list.splice(index, 1)[0];
      item.lastActiveTime = res.data.createTime;

      item.lastMessage = extractTextFromHTML(res.data.content);

      model.list.unshift(item);
    }
  });
};

const subscribeMessageAck = () => {
  signalR.subscribe('messageAck', res => {
    chatMessageRef.value.messageAck(res);

    const index = model.list.findIndex(x => x.id == res.data.roomId);
    if (index > -1) {
      const item = model.list.splice(index, 1)[0];

      item.lastActiveTime = res.data.createTime;
      item.lastMessage = extractTextFromHTML(res.data.content);

      model.list.unshift(item);
    }
  });
};

const subscribeLogout = () => {
  signalR.subscribe('logout', () => {
    stores.user.setLogoutState();
    stores.authorize.setUserLogoutState();
    stores.chat.resetChatStore();
    stores.chatMessage.resetChatMessageStore();

    location.reload();
  });
};

const notice = new notification.instance({
  dangerouslyUseHTMLString: true,
  customClass: 'chat-notice',
  offset: 100,
  callback: notickCallback
});

const showNotice = data => {
  let html = `
  <div>
    <div class="flex-start-center">
      <img src="${data.user.avatar}" />
      <span>${data.user.nickName}</span>
      ${data.isOwner ? `<span class="badge badge-purple">博主</span>` : ''}
    </div>
    <div class="chat-notice-content">
      ${extractTextFromHTML(data.content)}
    </div>
  </div>
  `;

  notice.show(html);
};

const selectRoom = item => {
  model.active = item;

  const index = model.list.findIndex(x => x.id == item.id);
  if (index > -1) {
    model.list[index].messageCount = 0;
  }

  if (model.visible) {
    stores.chat.clearUnreadMessage(item.id);
  }
};

const handleDelete = async item => {
  const index = model.list.findIndex(x => x.id == item.id);
  if (index > -1) {
    model.list.splice(index, 1);
  }
};

const hanldeSubmitMessage = async content => {
  let data = {
    id: uuid(),
    user: {
      id: stores.user.id,
      nickName: stores.user.nickName,
      avatar: stores.user.avatar
    },
    content: content,
    createTime: new Date().format('yyyy-MM-dd HH:mm:ss')
  };

  try {
    await signalR.invoke('sendMessage', {
      roomId: model.active.id,
      messageId: data.id,
      content,
      createTime: data.createTime
    });
  } catch (error) {
    // 发送失败
  } finally {
    chatMessageRef.value.sendMessage(data);
  }

  const index = model.list.findIndex(x => x.id === model.active.id);
  if (!model.list[index].lastActiveTime) {
    model.list[index].lastActiveTime = data.createTime;
  }
};

const show = () => {
  model.visible = true;
};

const messageRead = roomId => {
  const item = model.list.filter(x => x.id == roomId);
  if (item && item.length && item[0].unreadCount > 0) {
    signalR.invoke('UserUnreadMessageCount', {
      roomId,
      count: 0
    });
  }
};

const handleDialogOpen = () => {
  stores.chat.clearUnreadMessage(model.active.id);
};

const handleDialogOpened = () => {
  chatMessageRef.value.showScroll(true);
  chatEditBarRef.value.focus();
};

const chatMessageInit = (roomId, content) => {
  //
  const index = model.list.findIndex(x => x.id == roomId);
  if (index > -1 && !model.list[index].lastMessage) {
    model.list[index].lastMessage = extractTextFromHTML(content);
  }
};

function notickCallback(e) {
  show();
  e.close();
}

function changeRoomLastActiveTime(dateTime) {
  if (!dateTime) {
    return '';
  }

  try {
    dateTime = new Date(dateTime);
    const now = new Date();

    if (dateTime.getFullYear() !== now.getFullYear()) {
      return dateTime.format('yyyy/M/d');
    } else if (dateTime.getMonth() !== now.getMonth()) {
      return dateTime.format('M/d');
    } else if (dateTime.getDate() !== now.getDate()) {
      const timeDifferenceInDays = (now.getTime() - dateTime.getTime()) / (1000 * 60 * 60 * 24);
      return timeDifferenceInDays === 1 ? '昨天' : dateTime.format('M/d');
    } else {
      return dateTime.format('HH:mm');
    }
  } catch {
    return '';
  }
}

function extractTextFromHTML(htmlString) {
  // 图片
  let textContent = htmlString.replace(/<img.*?>/g, '[图片]');
  // 链接
  textContent = textContent.replace(/<a.*?>/g, '[链接]');
  // 音频
  textContent = textContent.replace(/<audio.*?>/g, '[音频]');
  // 视频
  textContent = textContent.replace(/<video.*?>/g, '[视频]');
  // 换行
  textContent = textContent.replace(/<br.*?>/g, ' ');
  // 提取纯文本
  return textContent.replace(/<[^>]+>/g, '');
}

defineExpose({
  show
});
</script>

<style lang="scss" scoped>
.chat-container {
  --chat-container-height: 850px;
  height: var(--chat-container-height);
  border: 1px solid var(--color-secondary);
  display: grid;
  grid-template-columns: 300px minmax(300px, 1fr);

  @media (max-width: 1920px) {
    --chat-container-height: 700px;
  }

  @media (max-width: 1440px) {
    --chat-container-height: 550px;
  }
}

.chat-room-list {
  height: var(--chat-container-height);
  overflow-y: auto;

  li {
    list-style: none;
    cursor: pointer;
  }

  .room {
    display: flex;
    justify-content: space-between;
    align-items: center;
    height: 70px;
    padding: 8px 10px;
    transition: all 0.3s;

    &:hover {
      background-color: #f2f2f2;
    }

    &.active {
      background-color: #ebebeb;
    }

    .flex-start-center {
      gap: 5px;

      .room-icon {
        height: 40px;
        width: 40px;
        border-radius: 10%;
        object-fit: cover;
        border: 1px solid var(--color-secondary);

        @media (max-width: 1440px) {
          height: 40px;
          width: 40px;
        }
      }

      .room-info {
        width: 165px;
        padding-left: 5px;

        .room-name {
          width: 110px;
          font-size: 14px;
          overflow: hidden;
          text-overflow: ellipsis;
          white-space: nowrap;
        }

        :deep(.room-last-message) {
          height: 22px;
          padding-top: 2px;

          > div {
            height: 20px;
          }

          div,
          p {
            overflow: hidden;
            text-overflow: ellipsis;
            white-space: nowrap;
          }

          * {
            font-size: 12px !important;
            color: var(--color-dark-light);
          }
        }
      }
    }

    .flex-center-center {
      min-width: 32px;
      height: 70px;
      flex-direction: column;

      .el-icon {
        display: none;
        transition: all 0.4s ease-in-out;

        &:hover {
          color: var(--color-danger);
        }
      }

      .room-active-time,
      .room-active-message {
        display: block;
        font-size: 12px;
        letter-spacing: 0.5px;
        color: var(--color-dark-light);
        height: 20px;
      }

      .room-active-message {
        margin: 0;
        > span {
          padding: 2px 4px;
          background-color: var(--color-danger);
          color: #fff;
          border-radius: 3px;
        }
      }

      &:hover {
        .el-icon {
          display: block;
        }

        .room-active-time,
        .room-active-message {
          display: none;
        }
      }
    }
  }
}

.chat-room {
  --room-message-height: 630px;
  --chat-bar-height: calc(var(--chat-container-height) - var(--room-message-height));
  height: var(--chat-container-height);
  border-left: 1px solid var(--color-secondary);

  @media (max-width: 1920px) {
    --room-message-height: 500px;
  }

  @media (max-width: 1440px) {
    --room-message-height: 380px;
  }
}
</style>

<style lang="scss">
.chat-modal {
  --el-dialog-width: 1200px;

  @media (max-width: 1920px) {
    --el-dialog-width: 1100px;
  }

  @media (max-width: 1440px) {
    --el-dialog-width: 1000px;
  }
}

.chat-notice {
  cursor: pointer;

  .flex-start-center {
    gap: 10px;
    > img {
      height: 40px;
      width: 40px;
      border-radius: 8px;
      object-fit: cover;
      border: 1px solid var(--color-secondary);
    }
  }

  .chat-notice-content {
    padding-top: 10px;
    padding-left: 5px;
    max-height: 110px;
    overflow: hidden;
    text-overflow: ellipsis;
    display: -webkit-box;
    -webkit-box-orient: vertical;
    -webkit-line-clamp: 4;
  }

  .badge {
    border-radius: 4px;
    color: #fff;
    font-size: 10px;
    padding: 0px 5px;
  }
}
</style>
