<template>
  <page-layout title="关于本站" icon="chrome-filled">
    <div class="web-container">
      <div class="card markdown">
        <markdown-container ref="markdown"></markdown-container>
      </div>
      <div class="right">
        <div class="card">
          <div class="card-title">网站概况</div>
          <div class="web-info">
            <div class="flex-center-center">
              <span>{{ countChange(model.info.browse) }}</span>
              <el-tooltip effect="dark" content="浏览统计" placement="bottom">
                <el-icon :size="24">
                  <component :is="'chrome-filled'"></component>
                </el-icon>
              </el-tooltip>
            </div>
            <div class="flex-center-center">
              <span>{{ countChange(model.info.comment) }}</span>
              <el-tooltip effect="dark" content="评论统计" placement="bottom">
                <el-icon :size="24">
                  <component :is="'comment'"></component>
                </el-icon>
              </el-tooltip>
            </div>
            <div class="flex-center-center">
              <span>{{ countChange(model.info.message) }}</span>
              <el-tooltip effect="dark" content="留言统计" placement="bottom">
                <el-icon :size="24">
                  <component :is="'message'"></component>
                </el-icon>
              </el-tooltip>
            </div>
          </div>
        </div>

        <div class="card">
          <div class="card-title">文章分类统计</div>
          <div class="flex-center-center charts">
            <div id="echarts"></div>
            <loading-line v-if="model.statistics.chartLoading" :loading="model.statistics.chartLoading" :show-text="true" text="数据加载中..."></loading-line>
          </div>
        </div>

        <div class="card">
          <div class="card-title">网站勉强维持运行</div>
          <div class="flex-center-center run-time">
            <span v-if="model.runTime.day > 0">{{ model.runTime.day }}天</span>
            <span v-if="model.runTime.hour > 0">{{ model.runTime.hour }}小时</span>
            <span v-if="model.runTime.minute > 0">{{ model.runTime.minute }}分</span>
            <span v-if="model.runTime.second > 0">{{ model.runTime.second }}秒</span>
          </div>
        </div>
      </div>
    </div>
  </page-layout>
</template>

<script setup name="aboutweb">
import { ref, onMounted, reactive } from 'vue';
import pageLayout from '../layout/page-layout.vue';
import * as echarts from 'echarts';
import markdownContainer from '../../components/markdown-editor/index.vue';
import loadingLine from '../../components/loadings/loading-line.vue';
import { getAboutWeb, getWebStatistics, getPostStatistics } from '../../api/home';
import { countChange } from '../../utils/tool';

const markdown = ref();

const model = reactive({
  info: {
    browse: 100000,
    comment: 100,
    message: 10000000
  },
  runTime: {
    day: 0,
    hour: 0,
    minute: 0,
    second: 0
  },
  statistics: {
    chartLoading: true
  }
});

const emit = defineEmits(['loading', 'browse']);

onMounted(async () => {
  try {
    webRunTime('2020-01-01 00:00:00');

    await aboutwebInit();

    await webStatisticsInit();

    postStatisticsInit();
  } finally {
    emit('loading', false);
    emit('browse');
  }
});

const webRunTime = buildTtime => {
  const time = buildTtime ? new Date(buildTtime).getTime() : new Date().getTime();

  function _run() {
    let timeSpan = Math.floor((new Date().getTime() - time) / 1000);

    const daySecond = 60 * 60 * 24;
    const hourSecond = 60 * 60;
    const minuteSecond = 60;

    const day = Math.floor(timeSpan / daySecond);
    if (day > 0) {
      model.runTime.day = parseInt(day);
    }

    timeSpan -= day * daySecond;

    const hour = Math.floor(timeSpan / hourSecond);
    if (hour > 0) {
      model.runTime.hour = parseInt(hour);
    }

    timeSpan -= hour * hourSecond;

    const minute = Math.floor(timeSpan / minuteSecond);
    if (minute > 0) {
      model.runTime.minute = parseInt(minute);
    }

    model.runTime.second = parseInt(timeSpan - minute * minuteSecond);
  }

  _run();
  setInterval(_run, 1000);
};

const aboutwebInit = async () => {
  let res = await getAboutWeb();
  if (res && res.resCode == 0) {
    markdown.value.init(res.data || '## 博主太懒，还未编辑');
  }
};

const webStatisticsInit = async () => {
  //
  let res = await getWebStatistics();
  if (res && res.resCode == 0) {
    model.info.browse = res.data.browse || 0;
    model.info.comment = res.data.comment || 0;
    model.info.message = res.data.message || 0;
  }
};

const postStatisticsInit = async () => {
  let res = await getPostStatistics();
  if (res && res.resCode == 0) {
    //
    echartsInit(res.data.list || []);
  }
};

const echartsInit = data => {
  const options = {
    tooltip: {
      trigger: 'item'
    },
    series: [
      {
        type: 'pie',
        radius: ['40%', '70%'],
        avoidLabelOverlap: false,
        itemStyle: {
          borderRadius: 10,
          borderColor: '#fff',
          borderWidth: 2
        },
        label: {
          show: false,
          position: 'center'
        },
        emphasis: {
          label: {
            show: false
          }
        },
        labelLine: {
          show: false
        },
        data: [...data]
      }
    ]
  };

  const instance = echarts.init(document.getElementById('echarts'));

  model.statistics.chartLoading = false;
  instance.setOption(options);
};
</script>

<style lang="scss" scoped>
.web-container {
  display: grid;
  grid-template-columns: 1100px 1fr;
  grid-gap: 20px;

  @media (max-width: 1920px) {
    grid-template-columns: 950px 1fr;
  }

  @media (max-width: 1440px) {
    grid-template-columns: 800px 1fr;
  }

  .markdown {
    min-height: calc(100vh - 305px);
  }

  .right {
    .card {
      margin-bottom: 25px;

      .web-info {
        display: grid;
        grid-template-columns: repeat(3, 1fr);
        grid-gap: 15px;
        padding: 15px 0;

        .flex-center-center {
          flex-direction: column;

          span {
            max-width: 95px;
            text-overflow: ellipsis;
            white-space: nowrap;
            overflow: hidden;
            padding: 5px 0;

            @media (max-width: 1920px) {
              max-width: 85px;
            }

            @media (max-width: 1440px) {
              max-width: 70px;
            }
          }

          span,
          .el-icon {
            transition: all 0.4s;
            cursor: pointer;
            color: var(--color-dark);
          }

          &:hover {
            span,
            .el-icon {
              color: var(--color-danger);
            }
          }
        }
      }

      .run-time {
        padding: 15px 0;

        span {
          padding: 0 0.25rem 0 0;
          font-size: 14px;
          cursor: default;
        }
      }

      .charts {
        padding: 15px 0;
        --aboutweb-chart-size: 300px;
        min-height: var(--aboutweb-chart-size);

        #echarts {
          height: var(--aboutweb-chart-size);
          width: var(--aboutweb-chart-size);
        }
      }

      &:last-child {
        margin-bottom: 0px;
      }
    }
  }
}
</style>
../../api/home
