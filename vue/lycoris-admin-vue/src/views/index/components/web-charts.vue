<template>
  <div class="chart-group">
    <div>
      <div class="chart" id="browse-statistics-chart"></div>
      <p v-show="!model.chartLoading">近七天访问统计</p>
      <loading-line v-if="model.chartLoading" :loading="model.chartLoading" :show-text="true" text="图表数据加载中..."></loading-line>
    </div>
    <div>
      <div class="chart" id="api-statistics-chart"></div>
      <p v-show="!model.chartLoading">近七天接口统计</p>
      <loading-line v-if="model.chartLoading" :loading="model.chartLoading" :show-text="true" text="图表数据加载中..."></loading-line>
    </div>
  </div>
</template>

<script setup>
import { onMounted, onUnmounted, reactive } from 'vue';
import * as echarts from 'echarts';
import loadingLine from '../../../components/loadings/loading-line.vue';
import { getNearlyDaysWebStatistics } from '../../../api/dashboard';

const model = reactive({
  chartLoading: true
});

const charts = {
  xAxis: [],
  tooltip: {
    trigger: 'axis',
    position: function (point, params, dom, rect, size) {
      //  size为当前窗口大小
      if (size.viewSize[0] / 2 >= point[0]) {
        //其中point为当前鼠标的位置
        return [point[0] + 50, '10%'];
      } else {
        //其中point为当前鼠标的位置
        return [point[0] - 200, '10%'];
      }
    }
  },
  browseStatistics: {
    instance: void 0,
    color: ['#926dde', '#f96197'],
    legendData: ['浏览量(PV)', '独立访客(UV)']
  },
  apiStatistics: {
    instance: void 0,
    color: ['#15c377', '#fa8181'],
    legendData: ['接口调用量', '异常调用']
  }
};

onMounted(async () => {
  browseStatisticsLineInit();
  apiStatisticsLineInit();

  let list = await getStatistics();

  if (!list || list.length == 0) {
    return;
  }

  charts.xAxis = list.map(x => {
    return x.day;
  });

  browseStatisticsLineLoad(list);
  apiStatisticsChartLoad(list);
});

onUnmounted(() => {
  charts.browseStatistics.instance.dispose();
  charts.apiStatistics.instance.dispose();
});

const chartResize = () => {
  charts.browseStatistics.instance.resize();
  charts.apiStatistics.instance.resize();
};

const getStatistics = async () => {
  let list = [];
  try {
    let res = await getNearlyDaysWebStatistics();
    if (res && res.resCode == 0) {
      list = res.data.list;
    }

    if (!list || list.length == 0) {
      let day = new Date(new Date().addDays(-6).format('yyyy-MM-dd 00:00:00'));
      for (let i = 0; i < 7; i++) {
        list.push({
          day: day.addDays(i).format('yyyy-MM-dd'),
          pvBrowse: 0,
          uvBrowse: 0,
          api: 0,
          errorApi: 0
        });
      }
    }

    return list;
  } catch (error) {
    //
  } finally {
    model.chartLoading = false;
  }
};

const browseStatisticsLineInit = () => {
  // 基于准备好的dom，初始化echarts实例
  charts.browseStatistics.instance = echarts.init(document.getElementById('browse-statistics-chart'));
  // 绘制图表
  charts.browseStatistics.instance.setOption({
    tooltip: charts.tooltip,
    color: charts.browseStatistics.color,
    legend: {
      itemGap: 30,
      data: charts.browseStatistics.legendData
    },
    xAxis: {
      data: []
    },
    yAxis: {},
    series: charts.browseStatistics.legendData.map((x, index) => {
      return {
        name: x,
        type: 'line',
        data: [],
        smooth: true,
        areaStyle: {
          color: charts.browseStatistics.color[index],
          opacity: 0.5
        }
      };
    }),
    animationEasing: 'sinusoidalIn',
    animationDelayUpdate: function (idx) {
      return idx * 5;
    }
  });
};

const browseStatisticsLineLoad = list => {
  charts.browseStatistics.instance.setOption({
    xAxis: {
      data: charts.xAxis
    },
    series: [
      {
        name: '浏览量(PV)',
        data: list.map(x => {
          return x.pvBrowse;
        })
      },
      {
        name: '独立访客(UV)',
        data: list.map(x => {
          return x.uvBrowse;
        })
      }
    ]
  });
};

const apiStatisticsLineInit = () => {
  // 基于准备好的dom，初始化echarts实例
  charts.apiStatistics.instance = echarts.init(document.getElementById('api-statistics-chart'));
  // 绘制图表
  charts.apiStatistics.instance.setOption({
    tooltip: charts.tooltip,
    color: charts.apiStatistics.color,
    legend: {
      itemGap: 30,
      data: charts.apiStatistics.legendData
    },
    xAxis: {
      data: []
    },
    yAxis: {},
    series: charts.apiStatistics.legendData.map((x, index) => {
      return {
        name: x,
        type: 'line',
        data: [],
        smooth: true,
        areaStyle: {
          color: charts.apiStatistics.color[index],
          opacity: 0.5
        }
      };
    }),
    animationEasing: 'sinusoidalIn',
    animationDelayUpdate: function (idx) {
      return idx * 5;
    }
  });
};

const apiStatisticsChartLoad = list => {
  charts.apiStatistics.instance.setOption({
    xAxis: {
      data: charts.xAxis
    },
    series: [
      {
        name: '接口调用量',
        data: list.map(x => {
          return x.api;
        })
      },
      {
        name: '异常调用',
        data: list.map(x => {
          return x.errorApi;
        })
      }
    ]
  });
};

defineExpose({
  chartResize
});
</script>

<style lang="scss" scoped>
.chart-group {
  padding-top: 35px;
  display: grid;
  grid-template-columns: 50% 50%;

  @media (max-width: 1440px) {
    grid-template-columns: 1fr;
  }

  > div {
    position: relative;
    height: 450px;

    .chart {
      height: 420px;

      @media (max-width: 1920px) {
        height: 330px;
      }
    }

    > p {
      text-align: center;
      font-size: 17px;
      letter-spacing: 2.5px;
    }
  }
}
</style>
