<template>
  <pageLayout title="个人中心" icon="chat-dot-round">
    <div class="user-container card">
      <div class="user-tabs">
        <div class="tabs">
          <ul>
            <li v-for="item in tabs" :key="item.key">
              <p :class="{ active: model.tabActive == item.key }" @click="model.tabActive = item.key">{{ item.label }}</p>
            </li>
          </ul>
        </div>
        <div class="tabpanels">
          <transition name="fade" mode="out-in">
            <user-info v-if="model.tabActive == 'user-info'"></user-info>
            <user-safe v-else-if="model.tabActive == 'user-safe'"></user-safe>
            <user-browsehistory
              v-else-if="model.tabActive == 'user-browse'"
              :list="model.userBrowsehistory.list"
              :page-index="model.userBrowsehistory.pageIndex"
              :page-size="model.userBrowsehistory.pageSize"
              :infinite-scroll-disabled="model.userBrowsehistory.scrollDisabled"
              :loading="model.userBrowsehistory.loading"
              @search="searchBrowseList"
              @load="loadBrowseList"
            ></user-browsehistory>
          </transition>
        </div>
      </div>
    </div>
  </pageLayout>
</template>

<script setup name="user">
import { onMounted, reactive, nextTick } from 'vue';
import pageLayout from '../layout/page-layout.vue';
import userInfo from './components/user-info.vue';
import userSafe from './components/user-safe.vue';
import userBrowsehistory from './components/user-browsehistory.vue';
import { userPostBrowseList } from '@/api/user';
import { stores } from '@/stores';

const emit = defineEmits(['loading', 'browse']);

const tabs = [
  {
    key: 'user-info',
    label: '基本信息'
  },
  {
    key: 'user-safe',
    label: '帐号安全'
  },
  {
    key: 'user-browse',
    label: '浏览历史'
  },
  {
    key: 'user-other',
    label: '其他'
  }
];

const model = reactive({
  tabActive: 'user-info',
  userBrowsehistory: {
    list: [],
    beginTime: '',
    endTime: '',
    pageIndex: 0,
    pageSize: 10,
    scrollDisabled: false,
    loading: false
  }
});

onMounted(async () => {
  setInterval(() => {
    if (stores.user.state) {
      emit('loading', false);
    }
  }, 500);

  emit('browse');
});

const searchBrowseList = ({ beginTime, endTime }) => {
  if (model.userBrowsehistory.beginTime != beginTime || model.userBrowsehistory.endTime != endTime) {
    model.userBrowsehistory.beginTime = beginTime;
    model.userBrowsehistory.endTime = endTime;
  }

  model.userBrowsehistory.pageIndex = 1;
  model.userBrowsehistory.list = [];
  nextTick(() => {
    GetBrowseList();
  });
};

const loadBrowseList = pageIndex => {
  if (model.userBrowsehistory.pageIndex < pageIndex) {
    model.userBrowsehistory.pageIndex = pageIndex;
    GetBrowseList();
  }
};

const GetBrowseList = async () => {
  model.userBrowsehistory.loading = true;
  try {
    let res = await userPostBrowseList({ ...model.userBrowsehistory });
    if (res && res.resCode == 0) {
      if (model.userBrowsehistory.pageIndex == 1) {
        model.userBrowsehistory.list = res.data.list;
      } else {
        res.data.list.forEach(x => model.userBrowsehistory.list.push(x));
      }
    }
  } finally {
    model.userBrowsehistory.loading = false;
  }
};
</script>

<style lang="scss" scoped>
.user-container {
  width: 100%;

  .user-tabs {
    min-height: calc(100vh - 335px);
    width: 100%;
    display: grid;
    grid-template-columns: 200px auto;

    .tabs {
      padding: 10px 25px 10px 10px;
      border-right: 1px solid var(--color-secondary);
      li {
        list-style: none;
        padding: 5px 5px;
        cursor: pointer;

        > p {
          font-size: 18px;
          font-weight: 500;
          padding: 8px 8px;
          border-radius: 5px;
          transition: all 0.4s;

          &.active,
          &:hover {
            background-color: #ecf5ff;
            color: var(--color-primary);
          }
        }
      }
    }

    .tabpanels {
      height: 100%;
      width: 100%;
      padding: 10px 40px;

      .tab-panel {
        margin-top: 25px;
      }
    }
  }
}
</style>

<style lang="scss">
.tabpanels {
  .form-group {
    margin-bottom: 18px;
    display: flex;
    justify-content: flex-start;
    align-items: center;
    width: 300px;

    label:first-child {
      font-size: 16px;
      font-weight: 500;
      width: 120px;
      padding: 5px;
    }

    &:last-child.sumit-btn {
      padding-top: 18px;
    }
  }
}
</style>
