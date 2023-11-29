<template>
  <div class="map-panel" :class="{ more: !model.showWorldMap }">
    <div class="world-map border" v-show="model.showWorldMap">
      <div id="echart-world-map"></div>
    </div>
    <div class="map-list-panel">
      <div class="map-header border-bottom">
        <div class="flex-start-center">
          <el-icon>
            <component :is="'paperclip'"></component>
          </el-icon>
          <span class="title"> 浏览地区排行 </span>
        </div>
        <div class="flex-start-center more" @click="model.showWorldMap = !model.showWorldMap">
          <small>{{ model.showWorldMap ? '查看更多' : '返回' }}</small>
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
              <span>{{ item.count }}</span>
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
  loading: true
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
    console.log(mapList);

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
</script>

<style lang="scss" scoped>
.map-panel {
  position: relative;
  display: grid;
  grid-template-columns: 2fr 1fr;
  grid-gap: 20px;
  grid-template-rows: 800px;
  padding: 20px 0;

  &.more {
    grid-template-columns: 1fr;
    grid-template-rows: auto;
  }

  .world-map {
    height: 100%;
    width: 100%;
    padding: 10px;
    border-radius: 5px;

    #echart-world-map {
      height: 100%;
      width: 100%;
    }
  }

  .map-list-panel {
    position: relative;
    height: 100%;
    width: 100%;
    overflow: hidden;
    display: flex;
    flex-direction: column;

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

        .map-value {
          width: 150px;

          div {
            &:first-child {
              width: 100px;
              padding-right: 5px;
              text-align: right;
            }

            &:last-child {
              width: 150px;
              padding-left: 5px;
              text-align: left;
            }
          }

          .percent {
            position: relative;

            .process-bar {
              position: absolute;
              margin: 0;
              padding: 0;
              left: 0;
              top: 0;
              height: 100%;
              background-color: var(--color-info-light);
              border: 0;
              opacity: 0.6;
            }

            &::after {
              content: '%';
            }
          }
        }
      }
    }

    .more-footer {
      flex-shrink: 1;
      height: 35px;
      text-align: center;
    }
  }
}
</style>
