<template>
  <div class="card">
    <div class="talk-header">
      <div class="owner flex-start-center">
        <el-image :src="props.owner.avatar" lazy></el-image>
        <div class="talk-owner-info">
          <p class="talk-owner-name">{{ props.owner.nickName }}</p>
          <p class="talk-owner-time">{{ props.data.createTime }}</p>
        </div>
      </div>
      <div class="ua">
        <span class="ip">{{ props.data.ipAddress == '未知' || props.data.ipAddress == '局域网' ? webSettings.privateIpAddress : props.data.ipAddress }}</span>
        <el-image :src="userAgentIcon(props.data.agentFlag)"></el-image>
      </div>
    </div>
    <div class="talk-content">
      {{ props.data.content }}
    </div>
    <slot></slot>
  </div>
</template>

<script setup>
import { getUserAgentIconByEnum as userAgentIcon } from '@/utils/user-agent';
import { webSettings } from '@/config.json';

const props = defineProps({
  data: {
    type: Object,
    required: true,
    default: void 0
  },
  owner: {
    type: Object,
    required: true,
    default: void 0
  }
});
</script>

<style lang="scss" scoped>
.talk-header {
  width: 100%;
  padding-bottom: 10px;
  border-bottom: 2px dashed var(--color-primary);

  display: flex;
  justify-content: space-between;
  align-items: center;

  .owner {
    .el-image {
      height: 50px;
      width: 50px;
      border-radius: 50%;
    }

    .talk-owner-info {
      padding-left: 20px;
      height: 50px;

      .talk-owner-name {
        padding-bottom: 5px;
        margin-bottom: 0;
      }

      .talk-owner-time {
        font-size: 14px;
        color: var(--color-dark-light);
      }
    }
  }

  .ua {
    display: flex;
    align-items: center;
    .el-image {
      height: 25px;
      width: 25px;
    }

    .ip {
      font-size: 14px;
      color: var(--color-dark-light);
      letter-spacing: 2px;
      padding-right: 25px;
      cursor: default;
    }
  }
}

.talk-content {
  padding: 10px 20px;
  color: var(--color-dark);
  line-height: 25px;
}
</style>
