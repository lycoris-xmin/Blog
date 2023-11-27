<template>
  <el-popover :placement="props.placement" :width="props.width" :trigger="props.trigger" @before-enter="beforeEnter" :persistent="props.persistent">
    <template #reference> <slot name="reference"></slot> </template>

    <div class="flex-center-center popover-user--info" v-if="props.userId && props.userId != '0'">
      <el-image class="avatar" :src="model.avatar" lazy>
        <template #error>
          <img :src="stores.webSetting.defaultAvatar" />
        </template>
      </el-image>
      <span class="name">{{ model.nickName }}</span>
      <div class="flex-center-center platform">
        <div v-if="model.github">
          <el-tooltip effect="dark" content="github" placement="bottom">
            <a :href="model.github" target="_blank"><el-image :src="'/icon/platform/github.png'"></el-image></a>
          </el-tooltip>
        </div>

        <div v-if="model.qq">
          <el-tooltip effect="dark" :content="model.qq" placement="bottom">
            <el-image :src="'/icon/platform/qq.png'"></el-image>
          </el-tooltip>
        </div>

        <div v-if="model.wechat">
          <el-tooltip effect="dark" :content="model.wechat" placement="bottom">
            <el-image :src="'/icon/platform/wechat.png'"></el-image>
          </el-tooltip>
        </div>

        <div v-if="model.email">
          <el-tooltip effect="dark" :content="model.email" placement="bottom">
            <el-image :src="'/icon/platform/email.png'"></el-image>
          </el-tooltip>
        </div>

        <div v-if="model.bilibili">
          <el-tooltip effect="dark" content="bilibili" placement="bottom">
            <el-image :src="'/icon/platform/bilibili.png'"></el-image>
          </el-tooltip>
        </div>

        <div v-if="model.music">
          <el-tooltip effect="dark" content="网易云" placement="bottom">
            <el-image :src="'/icon/platform/music.png'"></el-image>
          </el-tooltip>
        </div>
      </div>
      <slot name="action">
        <div class="action" v-show="props.showAction" v-if="props.userId != stores.user.id">
          <el-button @click="privateMessage">私信</el-button>
        </div>
      </slot>
    </div>
    <div v-else></div>
  </el-popover>
</template>

<script setup>
import { reactive, inject } from 'vue';
import { getUserBrief } from '../../api/user';
import { stores } from '../../stores';
import toast from '../../utils/toast';

const loginModalRef = inject('$loginModal');
const chatModalRef = inject('$chatModal');
const signalR = inject('$chat-signalR');

const model = reactive({
  nickName: '',
  avatar: '',
  email: '',
  qq: '',
  wechat: '',
  github: '',
  bilibili: '',
  music: ''
});

const props = defineProps({
  userId: {
    type: String,
    required: true
  },
  isOwner: {
    type: Boolean,
    default: false
  },
  placement: {
    type: String,
    default: 'left'
  },
  width: {
    type: Number,
    default: 250
  },
  trigger: {
    type: String,
    default: 'hover'
  },
  persistent: {
    type: Boolean,
    default: false
  },
  showAction: {
    type: Boolean,
    default: false
  }
});

const beforeEnter = async () => {
  if (getUserInfoBySotre()) {
    return;
  }

  try {
    let res = await getUserBrief(props.userId);

    if (res && res.resCode == 0) {
      model.nickName = res.data.nickName;
      model.avatar = res.data.avatar;
      model.email = res.data.email;
      model.qq = res.data.qq;
      model.wechat = res.data.wechat;
      model.github = res.data.github;
      model.bilibili = res.data.bilibili;
      model.music = res.data.music;

      stores.userInfo.addUserInfo(res.data);
    }
  } catch (error) {
    //
  }
};

const getUserInfoBySotre = () => {
  if (props.isOwner) {
    model.nickName = stores.owner.nickName;
    model.avatar = stores.owner.avatar;
    model.email = stores.owner.email;
    model.qq = stores.owner.qq;
    model.wechat = stores.owner.wechat;
    model.github = stores.owner.github;
    model.bilibili = stores.owner.bilibili;
    model.music = stores.owner.music;
    return true;
  }

  const userInfo = stores.userInfo.getUserInfo(props.userId);
  if (userInfo) {
    model.nickName = userInfo.nickName;
    model.avatar = userInfo.avatar;
    model.email = userInfo.email;
    model.qq = userInfo.qq;
    model.wechat = userInfo.wechat;
    model.github = userInfo.github;
    model.bilibili = userInfo.bilibili;
    model.music = userInfo.music;
    return true;
  }

  return false;
};

const privateMessage = () => {
  if (stores.user.state) {
    // 创建聊天室
    signalR.invoke('createChatRoom', props.userId);

    chatModalRef.value.show();
  } else {
    toast.info('请先登录');
    loginModalRef.value.show();
  }
};
</script>

<style lang="scss" scoped>
.popover-user--info {
  padding: 20px 10px;
  flex-direction: column;

  $avatar-size: 55px;
  .avatar {
    height: $avatar-size;
    width: $avatar-size;

    :deep(img) {
      height: $avatar-size;
      width: $avatar-size;
      border-radius: 50%;
      border: 1px solid var(--color-secondary);
    }
  }

  span.name {
    margin-top: 15px;
  }

  div.platform {
    padding: 10px 0;
    gap: 5px;

    $platform-icon-size: 35px;
    .el-image {
      height: $platform-icon-size;
      width: $platform-icon-size;
      cursor: pointer;

      :deep(img) {
        height: $platform-icon-size;
        width: $platform-icon-size;
      }
    }
  }
}
</style>
