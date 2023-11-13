<template>
  <el-dialog v-model="model.visible" title="用户详细信息" class="user-detail">
    <div class="user-detail-grid">
      <div class="grid-col-all">
        <p>基本信息</p>
      </div>
      <div class="grid-col">
        <div class="grid-col-lable">
          <span>用户昵称</span>
        </div>
        <div class="grid-col-value">
          <div>
            <p>{{ model.user.nickName }}</p>
          </div>
        </div>
      </div>

      <div class="grid-col">
        <div class="grid-col-lable">
          <span>用户邮箱</span>
        </div>
        <div class="grid-col-value">
          <div>
            <p>{{ model.user.email }}</p>
          </div>
        </div>
      </div>

      <div class="grid-col">
        <div class="grid-col-lable">
          <span>状态</span>
        </div>
        <div class="grid-col-value">
          <el-tag type="info" v-if="model.user.status == 0">未审核</el-tag>
          <el-tag v-else-if="model.user.status == 1">已审核</el-tag>
          <el-tag type="danger" v-else-if="model.user.status == 100">帐号注销</el-tag>
        </div>
      </div>

      <div class="grid-col">
        <div class="grid-col-lable">
          <span>注册时间</span>
        </div>
        <div class="grid-col-value">
          {{ model.user.createTime }}
        </div>
      </div>

      <div class="grid-col-all">
        <p>绑定信息</p>
      </div>

      <div class="grid-col">
        <div class="grid-col-lable">
          <span>QQ</span>
        </div>
        <div class="grid-col-value">
          <div>
            <p>{{ model.link.qq || '' }}</p>
          </div>
        </div>
      </div>

      <div class="grid-col">
        <div class="grid-col-lable">
          <span>微信</span>
        </div>
        <div class="grid-col-value">
          <div>
            <p>{{ model.link.weChat || '' }}</p>
          </div>
        </div>
      </div>

      <div class="grid-col">
        <div class="grid-col-lable">
          <span>GitHub</span>
        </div>
        <div class="grid-col-value">
          <div>
            <a class="a-link" v-if="model.link.github" :href="model.link.github" target="_blank">{{ model.link.github }}</a>
            <span v-else></span>
          </div>
        </div>
      </div>

      <div class="grid-col">
        <div class="grid-col-lable">
          <span>网易云</span>
        </div>
        <div class="grid-col-value">
          <div>
            <a class="a-link" v-if="model.link.cloudMusic" :href="model.link.cloudMusic" target="_blank">{{ model.link.cloudMusic }}</a>
            <span v-else></span>
          </div>
        </div>
      </div>

      <div class="grid-col">
        <div class="grid-col-lable">
          <span>哔哩哔哩</span>
        </div>
        <div class="grid-col-value">
          <div>
            <a class="a-link" v-if="model.link.bilibili" :href="model.link.bilibili" target="_blank">{{ model.link.bilibili }}</a>
            <span v-else></span>
          </div>
        </div>
      </div>

      <div class="grid-col">
        <div class="grid-col-lable">
          <span>码云</span>
        </div>
        <div class="grid-col-value">
          <div>
            <a class="a-link" v-if="model.link.gitee" :href="model.link.gitee" target="_blank">{{ model.link.gitee }}</a>
            <span v-else></span>
          </div>
        </div>
      </div>

      <loading-line :loading="model.loading"></loading-line>
    </div>
  </el-dialog>
</template>

<script setup>
import { reactive } from 'vue';
import { getUserLink } from '../../../api/user';
import loadingLine from '../../../components/loadings/loading-line.vue';

const model = reactive({
  visible: false,
  loading: false,
  user: {},
  link: {}
});

const show = async row => {
  if (model.user.id == row.id) {
    model.visible = true;
    return;
  }

  model.user = {};
  model.link = {};

  model.visible = true;
  model.loading = true;

  model.user = row;
  await getLink();

  model.loading = false;
};

const close = () => {
  model.visible = false;
};

const getLink = async () => {
  try {
    let res = await getUserLink(model.user.id);
    if (res && res.resCode == 0) {
      model.link = res.data;
    }
  } catch (error) {}
};

defineExpose({
  show,
  close
});
</script>

<style lang="scss" scoped>
.user-detail {
  .user-detail-grid {
    position: relative;
    display: grid;
    grid-template-columns: repeat(2, minmax(300px, 1fr));

    .grid-col-all {
      padding: 7px 10px;
      grid-column-start: span 2;
      border: 1px solid var(--color-secondary);
      border-bottom: 0;

      &:last-child {
        border-bottom: 1px solid var(--color-secondary);
      }

      p {
        font-weight: 600;
        font-size: 16px;
      }
    }

    .grid-col {
      display: flex;
      border: 1px solid var(--color-secondary);
      font-size: 14px;
      border-bottom: 0px;

      .grid-col-lable {
        display: flex;
        justify-content: flex-start;
        align-items: center;
        background-color: #f5f7fa;
        min-width: 100px;
        padding: 7px 10px;
        border-right: 1px solid var(--color-secondary);
        font-weight: 600;
      }

      .grid-col-value {
        display: flex;
        justify-content: flex-start;
        align-items: center;
        padding: 7px 10px;
        width: 100%;
        overflow: hidden;

        > div {
          text-overflow: ellipsis;
          overflow: hidden;
          white-space: nowrap;

          .a-link {
            color: var(--color-primary);
            transition: all 0.3s;

            &:hover {
              color: var(--color-purple);
            }
          }
        }
      }

      &:nth-last-child(3),
      &:nth-last-child(2) {
        border-bottom: 1px solid var(--color-secondary);
      }
    }
  }
}
</style>

<style>
.user-detail {
  width: 800px;
}
</style>
