talks
<template>
  <page-layout title="网站留言板" icon="message">
    <div class="message-container">
      <div class="card message-premise">
        <h2>留言提醒</h2>
        <p v-for="(item, index) in model.premise" :key="index">{{ index + 1 }}、{{ item }}</p>
      </div>
      <div class="card publich-message">
        <el-input v-model="model.content" type="textarea" placeholder="发布你的留言" :autosize="{ minRows: 4, maxRows: 6 }" maxlength="100" show-word-limit></el-input>
        <div style="text-align: right; padding-top: 15px">
          <el-button style="width: 100px" type="primary" plain @click="publish" :loading="model.publishLoading"> 发布 </el-button>
        </div>
      </div>

      <div class="message-ul">
        <transition-list tag="ul" :style="{ height: model.dataLoading ? '450px' : 'auto' }">
          <li v-for="item in model.list" :key="item.id" class="card">
            <message-data :data="item" :frequency="model.frequency" :last-publish="model.lastPublish" :key="item.id" @reply="replyMessage" @updateRedundancy="updateRedundancy"></message-data>
          </li>

          <li style="padding: 0" :class="{ card: model.count == 0 }">
            <div class="flex-center-center card" v-if="model.count > 0 && model.count > model.pageSize">
              <el-pagination :page-size="model.pageSize" layout="prev, pager, next" :total="model.count" hide-on-single-page v-model:current-page="model.pageIndex" />
            </div>
            <div v-if="model.count == 0" class="emtpy-message flex-center-center card">
              <span v-if="!model.dataLoading">暂时没有留言</span>
            </div>
          </li>
        </transition-list>

        <loading-line :loading="model.dataLoading" :show-text="true" text="网站留言获取中..."></loading-line>
      </div>
    </div>
  </page-layout>
</template>

<script setup name="message">
import { reactive, onMounted, inject, watch, onBeforeMount } from 'vue';
import pageLayout from '../layout/page-layout.vue';
import transitionList from '@/components/transitions/list-transition.vue';
import messageData from './components/message-data.vue';
import loadingLine from '@/components/loadings/loading-line.vue';
import { getConfiguration, getMessageList, publishMessage } from '@/api/message';
import { stores } from '@/stores';
import toast from '@/utils/toast';
import { scrollPageTop } from '@/utils/tool';
import { webSettings } from '@/config.json';

const loginModalRef = inject('$loginModal');

const model = reactive({
  premise: [],
  frequency: 0,
  count: 0,
  list: [],
  pageIndex: 1,
  pageSize: 10,
  content: '',
  publishLoading: false,
  dataLoading: false,
  lastPublish: 0
});

const emit = defineEmits(['loading', 'browse']);

onBeforeMount(async () => {
  try {
    let res = await getConfiguration();
    if (res && res.resCode == 0) {
      //
      model.premise = res.data.messageRemind && res.data.messageRemind.length > 0 ? res.data.messageRemind : webSettings.premise;
      model.frequency = typeof res.data.frequencySecond == 'number' ? res.data.frequencySecond * 1000 : 0;
    }
  } catch (error) {
    model.premise = webSettings.premise;
  }
});

onMounted(async () => {
  await getList();
  emit('loading', false);
  model.loaded = true;
  emit('browse');
});

const getList = async () => {
  model.dataLoading = true;
  try {
    let res = await getMessageList(model);
    if (res && res.resCode == 0) {
      model.count = res.data.count || 0;
      model.list = res.data.list || [];
    }
  } finally {
    model.dataLoading = false;
  }
};

const publish = async () => {
  try {
    if (!stores.user.state) {
      userLogin();
      return;
    }

    let now = new Date().getTime();
    if (!stores.user.isAdmin) {
      const lastTime = model.lastPublish + model.frequency;
      if (lastTime > now) {
        let time = Math.ceil((lastTime - now) / 1000);
        toast.warn(`留言发布有时间限制,请${time.toFixed(0)}秒后再发布留言`);
        return;
      }
    }

    model.publishLoading = true;

    let res = await publishMessage(model.content);
    if (res && res.resCode == 0) {
      if (model.pageIndex != 1) {
        return;
      }

      if (model.list.length >= model.pageSize) {
        model.list.pop();
      }

      model.list.unshift(res.data);
      model.count++;
      model.content = '';

      model.lastPublish = now;
    }
  } catch (error) {
    if (error && error.statusCode == 401) {
      userLogin();
    }
  } finally {
    model.publishLoading = false;
  }
};

const replyMessage = (id, redundancy, done) => {
  for (let item of model.list) {
    if (item.id == id) {
      if (!item.redundancy) {
        item.redundancy = [];
      }

      if (item.redundancy.length >= 2) {
        item.redundancy.pop();
      }

      item.replyCount++;
      item.redundancy.unshift(redundancy);

      break;
    }
  }

  model.lastPublish = new Date().getTime();

  done();
};

const updateRedundancy = list => {
  if (list && list.length) {
    for (let reply of list) {
      for (let item of model.list) {
        if (item.redundancy && item.redundancy.length) {
          const index = item.redundancy.findIndex(x => x.id == reply.id);
          if (index > -1 && item.redundancy[index].createTime != reply.createTime) {
            item.redundancy[index].createTime = reply.createTime;
            break;
          }
        }
      }
    }
  }
};

const userLogin = () => {
  toast.info('请先登录');
  loginModalRef.value.show();
};

watch(
  () => model.pageIndex,
  async () => {
    await getList();
    scrollPageTop(200);
  }
);

watch(
  () => stores.state,
  value => {
    if (value) {
      location.reload();
    }
  }
);
</script>

<style lang="scss" scoped>
.message-container {
  width: 100%;
  padding: 10px;
  min-height: calc(100vh - 305px);

  .message-premise {
    padding: 25px;
    margin-bottom: 15px;

    > p {
      line-height: 28px;
    }

    > p:nth-child(2) {
      padding-top: 10px;
    }
  }

  .publich-message {
    padding: 25px 25px 20px 25px;
    margin-bottom: 15px;
  }

  .message-ul {
    position: relative;
    overflow: hidden;

    > ul {
      overflow: hidden;
      > li {
        list-style: none;
        padding: 20px 25px;
        color: var(--color-dark);
        margin-bottom: 15px;

        .emtpy-message {
          font-size: 25px;
          height: 350px;
          letter-spacing: 2.5px;
          position: relative;
        }
      }
    }
  }
}
</style>
