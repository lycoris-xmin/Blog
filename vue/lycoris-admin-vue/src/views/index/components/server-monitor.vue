<template>
  <div class="monitor">
    <div>
      <div class="chart" id="server-monitor-chart"></div>
      <p v-show="!model.serverMonitorChartLoading">性能监控</p>
      <loading-line v-if="model.serverMonitorChartLoading" :loading="model.serverMonitorChartLoading" :show-text="true" text="监控数据加载中..."></loading-line>
    </div>
    <div>
      <div class="chart" id="api-monitor-chart"></div>
      <p v-show="!model.requestMonitorChartLoading">服务监控</p>
      <loading-line v-if="model.requestMonitorChartLoading" :loading="model.requestMonitorChartLoading" :show-text="true" text="监控数据加载中..."></loading-line>
    </div>
  </div>
</template>

<script setup>
import { reactive, onMounted, onUnmounted, inject } from 'vue';
import * as echarts from 'echarts';
import loadingLine from '../../../components/loadings/loading-line.vue';

const signalR = inject('$signalR');

const model = reactive({
  serverMonitorChartLoading: true,
  requestMonitorChartLoading: true
});

const charts = {
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
  serverMonitor: {
    instance: void 0,
    color: ['#926dde', '#fbb264'],
    legendData: ['CPU使用率', '内存使用率'],
    list: []
  },
  requestMonitor: {
    instance: void 0,
    color: ['#515d70', '#15c377', '#fa8181', '#007be0', '#f96197'],
    legendData: ['请求量', '成功', '异常', 'PV(浏览量)', 'UV(独立用户)'],
    list: []
  }
};

onMounted(() => {
  serverMonitorLineInit();
  requestMonitorLineInit();

  signalR.connectdHadler(subscribeMonitor);
});

onUnmounted(() => {
  charts.serverMonitor.instance.dispose();
  charts.requestMonitor.instance.dispose();
});

const chartResize = () => {
  charts.serverMonitor.instance.resize();
  charts.requestMonitor.instance.resize();
};

const serverMonitorLineInit = () => {
  // 基于准备好的dom，初始化echarts实例
  charts.serverMonitor.instance = echarts.init(document.getElementById('server-monitor-chart'));
  // 绘制图表
  charts.serverMonitor.instance.setOption({
    tooltip: {
      ...charts.tooltip,
      formatter: function (params) {
        let html = `
        <div style="margin: 0px 0 0;padding: 5px; line-height: 1;">
          <div style="font-size: 14px; color: #666; font-weight: 600; line-height: 1">${new Date().format('yyyy-MM-dd')} ${params[0].name}</div>
            <div style="margin: 15px 0 0; line-height: 1">
                `;

        for (let item of params) {
          html += `
          <div style="margin: 10px 0; line-height: 1">
            <div style="margin: 0px 0 0; line-height: 1">
              <span style="display: inline-block; margin-right: 4px; border-radius: 10px; width: 10px; height: 10px; background-color: ${item.color}"></span>
              <span style="font-size: 14px; color: #666; font-weight: 400; margin-left: 2px">${item.seriesName}</span>
              <span style="float: right; margin-left: 20px; font-size: 14px; color: #666; font-weight: 900">${item.value}%</span>
              <div style="clear: both"></div>
            </div>
            <div style="clear: both"></div>
          </div>
          `;
        }

        html += `
            <div style="clear: both"></div>
            </div>
          <div style="clear: both"></div>
        </div>`;
        return html;
      }
    },
    color: charts.serverMonitor.color,
    legend: {
      itemGap: 30,
      data: charts.serverMonitor.legendData
    },
    xAxis: {
      data: []
    },
    yAxis: {
      type: 'value',
      axisLabel: {
        show: true,
        interval: 'auto',
        formatter: '{value} %'
      },
      show: true,
      min: 0,
      max: 100
    },
    series: charts.serverMonitor.legendData.map(x => {
      return {
        name: x,
        type: 'line',
        data: [],
        smooth: true
      };
    }),
    animationEasing: 'sinusoidalIn',
    animationDelayUpdate: function (idx) {
      return idx * 5;
    }
  });
};

const requestMonitorLineInit = () => {
  // 基于准备好的dom，初始化echarts实例
  charts.requestMonitor.instance = echarts.init(document.getElementById('api-monitor-chart'));
  // 绘制图表
  charts.requestMonitor.instance.setOption({
    tooltip: {
      ...charts.tooltip,
      formatter: function (params) {
        let html = `
        <div style="margin: 0px 0 0;padding: 5px; line-height: 1;">
          <div style="font-size: 14px; color: #666; font-weight: 600; line-height: 1">${new Date().format('yyyy-MM-dd')} ${params[0].name}:00</div>
            <div style="margin: 15px 0 0; line-height: 1">
                `;

        for (let item of params) {
          html += `
          <div style="margin: 10px 0; line-height: 1">
            <div style="margin: 0px 0 0; line-height: 1">
              <span style="display: inline-block; margin-right: 4px; border-radius: 10px; width: 10px; height: 10px; background-color: ${item.color}"></span>
              <span style="font-size: 14px; color: #666; font-weight: 400; margin-left: 2px">${item.seriesName}</span>
              <span style="float: right; margin-left: 20px; font-size: 14px; color: #666; font-weight: 900">${item.value}</span>
              <div style="clear: both"></div>
            </div>
            <div style="clear: both"></div>
          </div>
          `;
        }

        html += `
            <div style="clear: both"></div>
            </div>
          <div style="clear: both"></div>
        </div>`;
        return html;
      }
    },
    color: charts.requestMonitor.color,
    legend: {
      itemGap: 30,
      data: charts.requestMonitor.legendData
    },
    xAxis: {
      data: []
    },
    yAxis: {},
    series: charts.requestMonitor.legendData.map(x => {
      return {
        name: x,
        type: 'line',
        data: [],
        smooth: true
      };
    }),
    animationEasing: 'sinusoidalIn',
    animationDelayUpdate: function (idx) {
      return idx * 5;
    }
  });
};

const subscribeMonitor = () => {
  signalR.invoke('connectServerMonitor');
  signalR.reconnectedHanlder(() => signalR.invoke('connectServerMonitor'));

  signalR.subscribe('serverMonitor', list => {
    if (model.serverMonitorChartLoading) {
      model.serverMonitorChartLoading = false;
    }
    refreshServerMonitor(list);
  });

  signalR.subscribe('requestMonitor', list => {
    if (model.requestMonitorChartLoading) {
      model.requestMonitorChartLoading = false;
    }
    refreshRequestMonitor(list);
  });
};

const refreshServerMonitor = list => {
  let option = {
    xAxis: {
      data: list.map(x => {
        return x.time;
      })
    },
    series: [
      {
        name: 'CPU使用率',
        data: list.map(x => {
          return x.cpuRate == '0' ? '0.01' : x.cpuRate;
        })
      },
      {
        name: '内存使用率',
        data: list.map(x => {
          return x.ramRate;
        })
      }
    ]
  };

  charts.serverMonitor.instance.setOption(option);
};

var requestList = [];

const refreshRequestMonitor = list => {
  if (requestList.length == 0) {
    requestList = list;
  } else if (requestList[requestList.length - 1].time === list[list.length - 1].time) {
    return;
  }

  let option = {
    xAxis: {
      data: list.map(x => {
        return x.time;
      })
    },
    yAxis: {},
    series: [
      {
        name: '请求量',
        data: list.map(x => {
          return x.request;
        })
      },
      {
        name: '成功',
        data: list.map(x => {
          return x.success;
        })
      },
      {
        name: '异常',
        data: list.map(x => {
          return x.error;
        })
      },
      {
        name: 'PV(浏览量)',
        data: list.map(x => {
          return x.pv;
        })
      },
      {
        name: 'UV(独立用户)',
        data: list.map(x => {
          return x.uv;
        })
      }
    ]
  };

  if (list.filter(x => x.request > 0 || x.pv > 0 || x.uv > 0).length == 0) {
    option.yAxis = {
      min: 0,
      max: 5,
      axisLabel: {
        interval: 1
      }
    };
  }

  charts.requestMonitor.instance.setOption(option);
};

defineExpose({
  chartResize
});
</script>

<style lang="scss" scoped>
.monitor {
  padding-top: 35px;
  display: grid;
  grid-template-columns: 50% 50%;
  grid-gap: 20px;

  @media (max-width: 1920px) {
    grid-gap: 10px;
  }

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
