<template>
  <div class="card author">
    <div class="avatar flex-center-center">
      <el-image :src="stores.owner.avatar"></el-image>
    </div>
    <p class="name">{{ stores.owner.nickName }}</p>
    <div class="owner-link flex-center-center">
      <div v-if="stores.owner.github">
        <el-tooltip effect="dark" content="github" placement="bottom">
          <a :href="stores.owner.github" target="_blank"><el-image :src="'/icon/platform/github.png'"></el-image></a>
        </el-tooltip>
      </div>

      <div v-if="stores.owner.qq">
        <el-tooltip effect="dark" :content="stores.owner.qq" placement="bottom">
          <el-image :src="'/icon/platform/qq.png'" @click="copy(stores.owner.qq)"></el-image>
        </el-tooltip>
      </div>

      <div v-if="stores.owner.wechat">
        <el-tooltip effect="dark" :content="stores.owner.wechat" placement="bottom">
          <el-image :src="'/icon/platform/wechat.png'" @click="copy(stores.owner.wechat)"></el-image>
        </el-tooltip>
      </div>

      <div v-if="stores.owner.email">
        <el-tooltip effect="dark" :content="stores.owner.email" placement="bottom">
          <el-image :src="'/icon/platform/email.png'" @click="copy(stores.owner.email, true)"></el-image>
        </el-tooltip>
      </div>

      <div v-if="stores.owner.bilibili">
        <el-tooltip effect="dark" content="bilibili" placement="bottom">
          <el-image :src="'/icon/platform/bilibili.png'"></el-image>
        </el-tooltip>
      </div>

      <div v-if="stores.owner.music">
        <el-tooltip effect="dark" content="网易云" placement="bottom">
          <el-image :src="'/icon/platform/music.png'"></el-image>
        </el-tooltip>
      </div>
    </div>
    <div class="web-info">
      <div class="info-item">
        <p>{{ stores.owner.statistics.post }}</p>
        <p>文章</p>
      </div>
      <div class="info-item">
        <p>{{ stores.owner.statistics.talk }}</p>
        <p>说说</p>
      </div>
      <div class="info-item">
        <p>{{ stores.owner.statistics.category }}</p>
        <p>分类</p>
      </div>
    </div>
  </div>
</template>

<script setup>
import useClipboard from 'vue-clipboard3';
import toast from '@/utils/toast';
import { stores } from '@/stores';

const { toClipboard } = useClipboard();

const copy = async (content, isemail = false) => {
  try {
    await toClipboard(content);
    toast.info(`复制成功,${isemail ? '非必要请不要乱发邮件谢谢！' : '加好友请注明来意'}`);
  } catch (error) {}
};
</script>

<style lang="scss" scoped>
.author {
  margin-bottom: 10px;

  .avatar {
    padding: 15px;
    width: 100%;
  }

  .el-image {
    height: 108px;
    width: 108px;
    display: flex;
    justify-content: center;
    align-items: center;
    border-radius: 50%;

    :deep(img) {
      height: 100px;
      width: 100px;
      border-radius: 50%;
    }
  }

  p.name {
    text-align: center;
    margin: 0;
    padding: 10px 0;
    font-size: 20px;
    letter-spacing: 2px;
  }

  .owner-link {
    .el-image {
      cursor: pointer;
      height: 45px;
      width: 45px;

      :deep(img) {
        height: 45px;
        width: 45px;
      }
    }
  }

  .web-info {
    display: flex;
    padding-top: 15px;
    justify-content: space-around;
    align-items: center;

    .info-item {
      height: 70px;
      width: 60px;

      p {
        text-align: center;
        cursor: default;
      }
    }
  }
}

@media (max-width: 1920px) {
  .author {
    padding: 8px;

    .avatar {
      padding: 8px;
    }

    .el-image {
      height: 88px;
      width: 88px;

      :deep(img) {
        height: 80px;
        width: 80px;
      }
    }

    p.name {
      font-size: 18px;
    }

    .owner-link {
      $link-icon-size: 40px;
      .el-image {
        height: $link-icon-size;
        width: $link-icon-size;

        :deep(img) {
          height: $link-icon-size;
          width: $link-icon-size;
        }
      }
    }

    .web-info {
      padding-top: 10px;

      .info-item {
        height: 70px;
        width: 60px;

        p {
          text-align: center;
          cursor: default;
        }
      }
    }
  }
}
</style>
