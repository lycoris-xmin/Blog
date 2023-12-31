<template>
  <div style="padding: 20px 0">
    <div class="title flex-start-center">
      <p class="domain flex-start-center">
        <img :src="`${stores.webSetting.webPath}/favicon.ico`" />
        <span @click="roteToPage">{{ stores.webSetting.webPath }}</span>
      </p>
      <p class="today">{{ new Date().format('yyyy-MM-dd') }}</p>
      <p class="online flex-start-center">
        <span class="icon" :class="{ success: model.onlineUsers > 0 }"></span>
        当前浏览
        <el-icon class="loading-icon" v-if="model.todayLoading">
          <component :is="'loading'"></component>
        </el-icon>
        <span class="number" v-show="!model.todayLoading">{{ model.onlineUsers }}</span>
        人
      </p>
    </div>
    <div class="statistics-count flex-start-center">
      <div class="count-statistics">
        <div class="flex-start-center">
          <div class="count flex-center-center">
            <el-icon class="loading-icon" v-if="model.todayLoading">
              <component :is="'loading'"></component>
            </el-icon>
            <span v-show="!model.todayLoading" class="number" :data-number="model.pvBrowse">0</span>
          </div>
          <p>
            <span class="lable"> 浏览量 </span>
            <span v-if="model.pvBrowsePercent != 0" class="percent flag" :class="{ up: model.pvBrowsePercent > 0, down: model.pvBrowsePercent < 0 }">
              <span>{{ model.pvBrowsePercent }}</span>
              %
            </span>
          </p>
        </div>
        <div class="flex-start-center">
          <div class="count flex-center-center">
            <el-icon class="loading-icon" v-if="model.todayLoading">
              <component :is="'loading'"></component>
            </el-icon>
            <span v-show="!model.todayLoading" class="number" :data-number="model.uvBrowse">0</span>
          </div>
          <p>
            <span class="lable">访客</span>
            <span v-if="model.uvBrowsePercent != 0" class="percent flag" :class="{ up: model.uvBrowsePercent > 0, down: model.uvBrowsePercent < 0 }">
              <span>{{ model.uvBrowsePercent }}</span>
              %
            </span>
          </p>
        </div>
        <div class="flex-start-center">
          <div class="count flex-center-center">
            <el-icon class="loading-icon" v-if="model.todayLoading">
              <component :is="'loading'"></component>
            </el-icon>
            <span v-show="!model.todayLoading" class="number" :data-number="model.commentMessage">0</span>
          </div>
          <p>
            <span class="lable">评论、留言</span>
            <span v-if="model.commentMessagePercent != 0" class="percent flag" :class="{ up: model.commentMessagePercent > 0, down: model.commentMessagePercent < 0 }">
              <span>{{ model.commentMessagePercent }}</span>
              %
            </span>
          </p>
        </div>
        <div class="flex-start-center">
          <label class="count flex-center-center">
            <el-icon class="loading-icon" v-if="model.todayLoading">
              <component :is="'loading'"></component>
            </el-icon>
            <span v-show="!model.todayLoading" :class="{ warning: model.elapsedMilliseconds > 1500, danger: model.elapsedMilliseconds > 5000 }">
              <span class="number" :data-number="model.elapsedMilliseconds">0</span>
              <small style="font-size: 14px; color: var(--color-dark)">ms</small>
            </span>
          </label>
          <p>
            <span class="lable">平均响应速度(ms)</span>
            <span v-if="model.elapsedMillisecondsDifference != 0" class="percent reverse-flag" :class="{ up: model.elapsedMillisecondsDifference < 0, down: model.elapsedMillisecondsDifference > 0 }">
              <span> {{ model.elapsedMillisecondsDifference }}</span>
              ms
            </span>
          </p>
        </div>
      </div>
    </div>

    <loading-line :loading="model.loading" :show-text="true" text="统计数据加载中..."></loading-line>
  </div>
</template>

<script setup>
import { onMounted, reactive, nextTick } from 'vue';
import loadingLine from '../../../components/loadings/loading-line.vue';
import { getTodayStatistics } from '../../../api/webStatistics';
import { animateNumber } from '../../../utils/tool';
import { stores } from '../../../stores';

const model = reactive({
  loading: true,
  todayLoading: true,
  onlineUsers: 0,
  pvBrowse: 0,
  pvBrowsePercent: 0,
  uvBrowse: 0,
  uvBrowsePercent: 0,
  commentMessage: 0,
  commentMessagePercent: 0,
  elapsedMilliseconds: 0,
  elapsedMillisecondsDifference: 0
});

const emit = defineEmits(['roteToPage']);

const animateNumberArray = [];

onMounted(async () => {
  const interval = setInterval(() => {
    if (stores.webSetting.webPath) {
      model.loading = false;
      clearInterval(interval);
      return;
    }
  }, 50);

  nextTick(() => {
    if (animateNumberArray.length == 0) {
      var numberEl = document.querySelectorAll('span.number');
      for (let item of [...numberEl]) {
        const animate = animateNumber(item);
        animate.minStep = 1;
        animate.targetNumberHandle = el => {
          return parseFloat(el.getAttribute('data-number') || 0);
        };
        animateNumberArray.push(animate);
      }
    }
  });

  getStatistics();
});

const getStatistics = async () => {
  try {
    let res = await getTodayStatistics();
    if (res && res.resCode == 0) {
      model.onlineUsers = res.data.onlineUsers;
      model.pvBrowse = res.data.pvBrowse;
      model.pvBrowsePercent = res.data.pvBrowsePercent.toFixed(2);
      model.uvBrowse = res.data.uvBrowse;
      model.uvBrowsePercent = res.data.uvBrowsePercent.toFixed(2);
      model.commentMessage = res.data.commentMessage;
      model.commentMessagePercent = res.data.commentMessagePercent;
      model.elapsedMilliseconds = res.data.elapsedMilliseconds;
      model.elapsedMillisecondsDifference = res.data.elapsedMillisecondsDifference;

      nextTick(() => {
        if (animateNumberArray && animateNumberArray.length > 0) {
          animateNumberArray.forEach(x => x.paly(1000));
        }
      });
    }
  } finally {
    model.todayLoading = false;
  }
};

const roteToPage = () => {
  emit('roteToPage', model.webDomain);
};
</script>

<style lang="scss" scoped>
.title {
  padding: 0 0 20px 10px;
  cursor: default;

  .domain {
    font-size: 26px;

    img {
      height: 30px;
      width: 30px;
      margin-right: 8px;
    }

    span {
      cursor: pointer;
      transition: all 0.4s;

      &:hover {
        color: var(--color-purple);
        text-decoration-line: underline;
        text-underline-offset: 10px;
      }
    }
  }

  .today {
    margin-left: 35px;
    padding: 0 20px;
  }

  .online {
    margin-left: 35px;

    .icon {
      height: 10px;
      width: 10px;
      border-radius: 50%;
      margin-right: 10px;
      background-color: var(--color-danger);

      &.success {
        background-color: var(--color-success);
      }
    }

    .number {
      padding: 0 5px;
    }

    .loading-icon {
      animation: el-icon-rotate 4s linear infinite;
      margin: 0 5px;
    }
  }
}

.statistics-count {
  padding: 20px;

  .count-statistics {
    width: 1400px;
    display: grid;
    grid-template-columns: repeat(4, 1fr);
    grid-gap: 20px;
    cursor: default;

    > .flex-start-center {
      flex-direction: column;
      align-items: start;

      .count {
        height: 58px;
        font-size: 36px;
        letter-spacing: 2px;
        padding-bottom: 10px;
        font-weight: 700;

        .loading-icon {
          font-size: 46px;
        }

        span {
          &.warning {
            color: var(--color-warning);
          }
          &.danger {
            color: var(--color-danger);
          }
        }
      }

      .label {
        font-size: 14px;
      }

      .percent {
        margin: 0 5px;
        font-size: 12px;
        font-weight: 600;
        padding: 1px 4px;
        border-radius: 3px;
        background-color: var(--color-secondary);

        &.up {
          color: #15a46e;
          background-color: #cef8e0;

          &.flag::before {
            content: '+';
          }
        }

        &.down {
          color: #f75c46;
          background-color: #ffebe7;

          &.reverse-flag::before {
            content: '+';
          }
        }
      }
    }
  }
}

.loading-icon {
  animation: el-icon-rotate 4s linear infinite;
}
</style>
