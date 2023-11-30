<template>
  <div class="world-map-container" :class="{ 'hide-world-map': !model.showWorldMap }">
    <div class="world-map flex-center-center">
      <div class="border">
        <div id="echart-world-map"></div>
      </div>
    </div>

    <div class="map-list-panel">
      <div class="map-header border-bottom">
        <div class="flex-start-center">
          <el-icon>
            <component :is="'paperclip'"></component>
          </el-icon>
          <span class="title"> 浏览地区排行 </span>
        </div>
        <div class="flex-start-center more" @click="handleMoreList">
          <el-button link>{{ model.showWorldMap ? '查看更多' : '返回' }}</el-button>
          <el-icon :size="14">
            <component :is="'d-arrow-right'"></component>
          </el-icon>
        </div>
      </div>
      <ul class="list-body">
        <li v-for="item in model.worldMapList" :key="item.country" class="map-item">
          <div class="map-country flex-start-center">
            <img class="border" :src="`/flags/${item.flag}.png`" />
            <p>{{ item.country }}</p>
          </div>
          <div class="map-value flex-center-center">
            <div class="border-right border-dark">
              <span>{{ model.showWorldMap ? item.count : item.count }}</span>
            </div>
            <div class="percent border-left border-dark">
              <div class="process-bar" :style="{ width: `${item.percent}%` }"></div>
              <span>{{ item.percent }}</span>
            </div>
          </div>
        </li>
      </ul>
    </div>

    <loading-line v-if="model.loading" :show-text="true" text="浏览地区分布数据加载中..."></loading-line>
  </div>
</template>

<script setup name="world-map">
import { onMounted, onUnmounted, reactive } from 'vue';
import * as echarts from 'echarts';
import loadingLine from '@/components/loadings/loading-line.vue';
import request from '@/utils/request';
import { getWorldMapList } from '@/api/webStatistics.js';
import toast from '@/utils/toast';

const model = reactive({
  worldMapList: [],
  showWorldMap: true,
  loading: true,
  mapPanelwidth: 0,
  worldMapWidth: 0
});

const echartModel = {
  instance: void 0,
  option: {
    tooltip: {
      trigger: 'item',
      showDelay: 0,
      transitionDuration: 0.2,
      borderColor: '#666',
      formatter: function (params) {
        if (params.name) {
          return params.name + ' : ' + (isNaN(params.value) ? 0 : parseInt(params.value));
        }
      }
    },
    visualMap: {
      show: false,
      min: 0,
      max: 1000000,
      inRange: {
        color: ['#1BA3E8', '#0082C4', '#0062A1', '#004580', '#00295F']
      },
      text: ['High', 'Low'],
      calculable: true
    },
    series: [
      {
        name: 'World',
        type: 'map',
        map: 'world',
        emphasis: {
          label: {
            show: true
          }
        },
        zoom: 1.2, //地图大小
        roam: false, //禁止拖拽
        data: []
      }
    ]
  }
};

onMounted(async () => {
  try {
    const chartDom = document.getElementById('echart-world-map');
    echartModel.instance = echarts.init(chartDom, null, {
      renderer: 'canvas',
      useDirtyRect: false
    });

    const { geo, nameMap, flag } = await getWorldJson();

    if (!geo && !nameMap && !flag) {
      toast.error('加载地图数据失败');
      return;
    }

    echarts.registerMap('world', geo);
    echartModel.option.series[0].nameMap = nameMap;
    echartModel.instance.setOption(echartModel.option);

    const mapList = await getMapList(flag);
    if (!mapList) {
      return;
    }

    const total = mapList.map(x => x.count).reduce((total, value) => total + value);

    model.worldMapList = mapList.map(x => {
      return {
        country: x.country,
        count: x.count,
        flag: x.flag,
        percent: ((x.count / total) * 100).toFixed(2)
      };
    });

    echartModel.instance.setOption({
      series: [
        {
          data: mapList.map(x => {
            return {
              name: x.country,
              value: x.count
            };
          })
        }
      ]
    });

    window.addEventListener('resize', echartModel.instance.resize);
  } finally {
    model.loading = false;
  }
});

onUnmounted(() => {
  echartModel.instance.dispose();
});

const getWorldJson = async () => {
  try {
    let res = await request.axios.get('/world/world.json');

    if (res && res.data) {
      return res.data;
    } else {
      return { geo: void 0, nameMap: void 0, flag: void 0 };
    }
  } catch (error) {
    return { geo: void 0, nameMap: void 0, flag: void 0 };
  }
};

const getMapList = async jsonFlag => {
  try {
    let res = await getWorldMapList();
    if (res && res.resCode == 0) {
      const list = [];
      for (let item of res.data.list) {
        const data = {
          country: changeCountry(item.country),
          count: item.count,
          flag: ''
        };

        data.flag = jsonFlag[data.country];
        const index = list.findIndex(x => x.country == data.country);
        if (index > -1) {
          list[index].count += data.count;
          continue;
        }

        list.push(data);
      }

      list.sort((a, b) => b - a);

      return list;
    }
  } catch (error) {
    return void 0;
  }
};

const changeCountry = country => {
  if (country.indexOf('香港') > -1 || country.indexOf('澳门') > -1 || country.indexOf('台湾') > -1) {
    return '中国';
  }

  return country;
};

const handleMoreList = () => {
  model.showWorldMap = !model.showWorldMap;
};
</script>

<style lang="scss" scoped>
.world-map-container {
  --wold-map-height: 800px;
  --world-map-width: 1400px;
  min-height: var(--wold-map-height);
  padding: 40px 0;

  @media (max-width: 1920px) {
    --wold-map-height: 550px;
    --world-map-width: 1000px;
  }

  &.hide-world-map {
    .world-map {
      width: 0px;

      #echart-world-map {
        display: none;
      }
    }

    .map-list-panel {
      margin-left: 0px;
      height: 100%;
    }
  }

  .world-map {
    position: absolute;
    left: 0;
    top: 0;
    height: 100%;
    width: var(--world-map-width);
    transition: width 0.75s cubic-bezier(0.22, 0.61, 0.36, 1);

    .border {
      position: relative;
      border-radius: 5px;
    }

    #echart-world-map {
      height: var(--wold-map-height);
      width: var(--world-map-width);
    }
  }

  .map-list-panel {
    position: relative;
    margin-left: var(--world-map-width);
    padding-left: 20px;
    height: var(--wold-map-height);
    overflow: hidden;
    display: flex;
    flex-direction: column;
    transition: margin-left 0.75s cubic-bezier(0.22, 0.61, 0.36, 1);

    .map-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      font-size: 18px;
      padding-bottom: 8px;

      span.title {
        padding-left: 10px;
      }

      div.more {
        cursor: pointer;
        transition: all 0.4;

        .el-button {
          color: var(--color-dark);

          &:hover {
            color: var(--color-info);
          }
        }

        &:hover {
          color: var(--color-info);
        }
      }
    }

    .list-body {
      overflow: hidden;

      .map-item {
        display: flex;
        justify-content: space-between;
        align-items: center;
        padding: 10px 5px;
        font-size: 16px;

        .map-country {
          cursor: default;

          p {
            padding-left: 5px;
            overflow: hidden;
            text-overflow: ellipsis;
            word-break: break-all;
            white-space: nowrap;
          }
        }
      }
    }
  }
}
</style>
