<template>
  <div v-if="props.data" class="redundancy-box">
    <div class="redundancy-body">
      <div class="redundancy-header flex-start-center">
        <popover-user-info-card :user-id="props.data.user.id" :is-owner="props.data.isOwner">
          <template #reference>
            <el-image :src="props.data.user.avatar.startsWith('/avatar/') ? `${api.server}${props.data.user.avatar}` : props.data.user.avatar" lazy>
              <template #error>
                <img :src="stores.webSetting.defaultAvatar" />
              </template>
            </el-image>
          </template>
        </popover-user-info-card>
        <span>
          {{ props.data.user.nickName }}
          <el-tag v-if="props.data.isOwner" size="small">博主</el-tag>
        </span>
      </div>

      <div class="redundancy-content">
        <p>
          <span class="replied-user" v-if="props.data.repliedUser">
            <popover-user-info-card :user-id="props.data.repliedUser.id" :is-owner="props.data.repliedUser.isOwner" :trigger="'click'" :placement="'top'">
              <template #reference>
                <span class="reply-user-text"> @{{ props.data.repliedUser.nickName }} </span>
              </template>
            </popover-user-info-card>
          </span>

          <span v-if="props.data.status == 0">{{ props.data.content }}</span>
          <span class="status-message status-danger" v-else-if="props.data.status == 1">该留言含有不良信息,可能涉及违规,为避免影响浏览,暂时不予展示</span>
          <span class="status-message status-default" v-else-if="props.data.status == 2">用户已删除留言</span>
          <span class="status-message status-default" v-else>该留言可能涉及违规,为避免影响其他用户浏览,暂时不予展示</span>
        </p>
      </div>
    </div>
    <div class="action flex-end-center">
      <span>回复于 {{ props.data.createTime }}</span>
      <div class="ip">{{ props.data.ipAddress == '未知' || props.data.ipAddress == '局域网' ? webSettings.privateIpAddress : props.data.ipAddress }}</div>
      <div class="flex-center-center" style="gap: 8px" @click="showReply">
        <el-icon>
          <component :is="'chat-dot-round'"></component>
        </el-icon>
        <span>回复他/她</span>
      </div>
    </div>
  </div>
</template>

<script setup>
import popoverUserInfoCard from '@/components/popover-user-card/index.vue';
import { stores } from '@/stores';
import { webSettings, api } from '@/config.json';

const props = defineProps({
  data: {
    type: Object,
    default: void 0
  }
});

const emit = defineEmits(['showReply']);

const showReply = () => {
  emit('showReply');
};
</script>

<style lang="scss" scoped>
.redundancy-box {
  padding: 0 8px 15px 8px;

  .redundancy-body {
    display: flex;
    justify-content: flex-start;
    align-items: flex-start;

    .redundancy-header {
      flex-shrink: 0;
      gap: 8px;

      $avatar-size: 40px;
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
    }

    .redundancy-header::after {
      content: '：';
    }

    .redundancy-content {
      padding: 8px 10px;
      color: var(--color-dark);
      line-height: 26px;

      .replied-user {
        color: var(--color-primary);
        cursor: pointer;

        .reply-user-text {
          padding-right: 10px;
        }
      }

      p {
        span.status-message {
          padding: 4px 8px;
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
    }
  }

  .action {
    padding-top: 15px;
    font-size: 14px;
    color: var(--color-dark-light);
    gap: 15px;

    > div {
      cursor: pointer;

      .el-icon,
      span {
        transition: all 0.4s;
      }
    }

    > div:hover {
      .el-icon,
      span {
        color: var(--color-info);
      }
    }

    $us-size: 20px;
    .agent {
      height: $us-size;
      width: $us-size;

      :deep(img) {
        height: $us-size;
        width: $us-size;
        object-fit: cover;
      }
    }

    .delete-message {
      cursor: pointer;
      color: var(--color-danger);
      transform: all 0.3s;
    }
    .delete-message:hover {
      color: var(--color-danger-light);
    }
  }
}
</style>
