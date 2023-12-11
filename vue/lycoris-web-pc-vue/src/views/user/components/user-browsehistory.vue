<template>
  <div class="browse">
    <div class="header">
      <span class="info-text">浏览记录只保留180天</span>
      <div class="date-picker">
        <el-date-picker
          v-model="model.datePicker"
          type="daterange"
          range-separator="至"
          start-placeholder="开始时间"
          end-placeholder="结束时间"
          format="YYYY-MM-DD"
          value-format="YYYY-MM-DD"
          :disabled-date="handleDisabledDate"
        />
      </div>
      <el-button type="primary" plain @click="handleSearch">查询</el-button>
    </div>
    <ul class="browse-ul" v-infinite-scroll="load" :infinite-scroll-immediate="false">
      <li v-for="item in props.list" :key="item.id" @click="routeToPage(item.postId)">
        <div class="list-card" v-if="item.postId && item.postId != '0'">
          <div class="icon">
            <img :src="item.icon" :alt="item.title" />
          </div>
          <div class="post">
            <p class="title">{{ item.title }}</p>
            <p class="info">
              {{ item.info }}
            </p>
          </div>
          <div class="browse-time">
            {{ item.lastTime }}
          </div>
        </div>

        <div v-else class="post-now-found flex-center-center">
          <span>文章已删除</span>
        </div>
      </li>
      <li class="loading">
        <loading-line v-show="props.loading" :loading="props.loading"></loading-line>
        <div v-show="!props.loading && props.list.length == 0" class="flex-center-center empty">
          <span class="empty-text">您在当前时段内没有浏览任务文章</span>
        </div>
      </li>
    </ul>
  </div>
</template>

<script setup>
import { onMounted, reactive } from 'vue';
import loadingLine from '../../../components/loadings/loading-line.vue';

const props = defineProps({
  list: {
    type: Array,
    default: void 0
  },
  pageIndex: {
    type: Number,
    require: true
  },
  pageSize: {
    type: Number,
    require: true
  },
  'infinite-scroll-disabled': {
    type: Boolean,
    require: true
  },
  loading: {
    type: Boolean,
    require: true
  }
});

const model = reactive({
  datePicker: []
});

const emit = defineEmits(['load', 'search']);

onMounted(() => {
  if (props.pageIndex == 0) {
    load();
  }
});

const handleSearch = () => {
  if (model.datePicker && model.datePicker.length == 2) {
    emit('search', {
      pageIndex: 1,
      beginTime: model.datePicker[0],
      endTime: model.datePicker[1]
    });
  } else {
    emit('search', {
      pageIndex: 1
    });
  }
};

const load = () => {
  emit('load', props.pageIndex + 1);
};

const routeToPage = postId => {
  if (postId && postId != '0') {
    window.open(`/post/${postId}`);
  }
};

const handleDisabledDate = date => {
  const now = new Date();
  if (now < date) {
    return true;
  }

  const earlyDate = new Date(now.addDays(-180));
  if (date < earlyDate) {
    return true;
  }

  return false;
};
</script>

<style lang="scss" scoped>
.browse {
  height: calc(100vh - 355px);

  .header {
    display: flex;
    justify-content: flex-end;
    align-items: center;
    padding: 10px 5px;

    .info-text {
      padding-right: 10px;
      color: var(--color-danger);
      &::before {
        content: '*';
      }
    }

    .date-picker {
      width: 350px;
      margin-right: 10px;
    }
  }

  .browse-ul {
    position: relative;
    height: calc(100vh - 400px);
    overflow-x: hidden;

    li {
      margin-right: 5px;
      margin-bottom: 25px;
      cursor: pointer;
    }

    .post-now-found {
      position: relative;
      height: 120px;
      width: 100%;
      background-color: var(--color-secondary);
      padding: 10px;
      border-radius: 5px;
      cursor: default;

      span {
        font-size: 22px;
        color: var(--color-danger);
      }
    }

    .loading {
      position: relative;
      height: 120px;
      width: 100%;

      &:first-child {
        height: 100%;
        margin-bottom: 0px !important;
      }

      .empty {
        height: 120px;

        .empty-text {
          font-size: 22px;
        }
      }
    }
  }
}

.list-card {
  display: grid;
  grid-template-columns: 145px 1fr 180px;
  grid-gap: 15px;
  padding: 10px;
  border-radius: 5px;
  transition: all 0.4s;

  .icon {
    border-radius: 5px;
    border: 1px solid var(--color-secondary);

    > img {
      height: 90px;
      width: 145px;
      object-fit: cover;
    }
  }

  .post {
    display: flex;
    justify-content: flex-start;
    align-items: flex-start;
    flex-direction: column;

    .title {
      font-size: 18px;
      letter-spacing: 2px;
      font-weight: 500;
      padding-bottom: 5px;
    }

    .info {
      font-size: 12px;
      color: var(--color-dark-light);
      overflow: hidden;
      word-break: break-all;
      text-overflow: ellipsis;
      display: -webkit-box;
      -webkit-box-orient: vertical;
      -webkit-line-clamp: 2;
    }
  }

  .browse-time {
    display: flex;
    justify-content: flex-end;
    align-items: center;
  }

  &:hover {
    background-color: var(--color-secondary);

    .icon {
      border-color: #fff;
    }
  }
}
</style>
