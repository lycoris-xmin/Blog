<template>
  <div class="statistic-group">
    <div class="statistic-card" v-for="item in model.statistics" :key="item.key" @click="routeTo(item.key)">
      <div class="statistic-header">
        <el-icon>
          <component :is="item.icon"></component>
        </el-icon>
        <div class="statistic-value">
          <el-statistic v-if="!model.loading" :value="item.count" :formatter="countChange" />
          <el-icon v-else>
            <component :is="'loading'"></component>
          </el-icon>
        </div>
      </div>
      <div class="statistic-title">
        <span>{{ item.title }}</span>
      </div>
    </div>
  </div>
</template>

<script setup>
import { onMounted, reactive } from 'vue';
import { countChange } from '../../../utils/tool';
import { getWebStatistics } from '../../../api/dashboard';
import { useRouter } from 'vue-router';

const router = useRouter();

const model = reactive({
  loading: true,
  statistics: [
    {
      key: 'browse',
      title: '今日浏览',
      icon: 'chrome-filled',
      count: 0
    },
    {
      key: 'api',
      title: '今日接口调用',
      icon: 'monitor',
      count: 0
    },
    {
      key: 'errorApi',
      title: '今日接口异常调用',
      icon: 'warn-triangle-filled',
      count: 0
    },
    {
      key: 'totalBrowse',
      title: '网站浏览统计',
      icon: 'chrome-filled',
      count: 0
    },
    {
      key: 'message',
      title: '网站留言统计',
      icon: 'message',
      count: 0
    },
    {
      key: 'users',
      title: '网站用户统计',
      icon: 'user',
      count: 0
    }
  ]
});

onMounted(() => {
  getData();
});

const getData = async () => {
  try {
    let res = await getWebStatistics();
    if (!res || res.resCode != 0 || !res.data) {
      return;
    }

    for (let key in res.data) {
      for (let item of model.statistics) {
        if (key == item.key) {
          item.count = res.data[key];
          break;
        }
      }
    }
  } finally {
    model.loading = false;
  }
};

const routeTo = key => {
  let opt = {
    name: ''
  };
  if (key == 'browse' || key == 'totalBrowse') {
    opt.name = 'statistics-browse';
  } else if (key == 'api' || key == 'errorApi') {
    opt.name = 'statistics-request';
    opt.params = {
      key
    };
  } else if (key == 'message') {
    opt.name = 'message';
  } else {
    opt.name = 'user';
  }

  router.push(opt);
};
</script>

<style lang="scss" scoped>
.statistic-group {
  padding: 10px 0;
  display: grid;
  grid-template-columns: repeat(6, 342px);
  grid-gap: 20px;

  @media (max-width: 1920px) {
    grid-gap: 10px;
    grid-template-columns: repeat(6, 1fr);
  }

  @media (max-width: 1440px) {
    grid-gap: 25px;
    grid-template-columns: repeat(3, 1fr);
  }

  .statistic-card {
    padding: 20px 40px;
    border-radius: 10px;
    --statistic-bg-color: #fff;
    --statistic-border-color: #fff;
    background-color: var(--statistic-bg-color);
    border: 1px solid var(--statistic-border-color);
    color: #fff;
    cursor: pointer;
    transition: all 0.3s;

    .statistic-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      font-size: 28px;

      @media (max-width: 1920px) {
        font-size: 23px;
      }

      @media (max-width: 1440px) {
        font-size: 28px;
      }

      .statistic-value {
        height: 39px;
        display: flex;
        justify-content: center;
        align-items: center;

        @media (max-width: 1920px) {
          font-size: 32px;
        }

        @media (max-width: 1440px) {
          height: 39px;
        }

        .el-icon {
          font-size: 24px;
          animation: el-icon-rotate 4s linear infinite;

          @media (max-width: 1920px) {
            font-size: 18px;
          }

          @media (max-width: 1440px) {
            font-size: 24px;
          }
        }

        @keyframes el-icon-rotate {
          from {
            transform: rotate(0deg);
          }
          to {
            transform: rotate(360deg);
          }
        }

        :deep(.el-statistic__number) {
          transition: all 0.4s;
          font-weight: 600;
          color: #fff;
          font-size: 24px;

          @media (max-width: 1920px) {
            font-size: 18px;
          }

          @media (max-width: 1440px) {
            font-size: 24px;
          }
        }
      }
    }

    .statistic-title {
      text-align: right;
      padding-top: 10px;
      font-size: 16px;

      @media (max-width: 1920px) {
        font-size: 14px;
      }

      @media (max-width: 1440px) {
        font-size: 16px;
      }
    }

    &:hover {
      box-shadow: 0 0 8px 4px var(--statistic-border-color) !important;
    }

    &:nth-child(1) {
      --statistic-bg-color: var(--color-info);
      --statistic-border-color: var(--color-info-light);
    }

    &:nth-child(2) {
      --statistic-bg-color: var(--color-purple);
      --statistic-border-color: var(--color-purple-light);
    }

    &:nth-child(3) {
      --statistic-bg-color: var(--color-danger);
      --statistic-border-color: var(--color-danger-light);
    }

    &:nth-child(4) {
      --statistic-bg-color: var(--color-primary);
      --statistic-border-color: var(--color-primary-light);
    }

    &:nth-child(5) {
      --statistic-bg-color: var(--color-brown);
      --statistic-border-color: var(--color-brown-light);
    }

    &:nth-child(6) {
      --statistic-bg-color: var(--color-pink);
      --statistic-border-color: var(--color-pink-light);
    }
  }
}
</style>
