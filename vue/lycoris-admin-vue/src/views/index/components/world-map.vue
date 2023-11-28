<template>
  <div>
    <div id="word-map"></div>
  </div>
</template>

<script setup>
import { onMounted } from 'vue';
import * as echarts from 'echarts';
import request from '../../../utils/request';

onMounted(() => {
  var chartDom = document.getElementById('word-map');
  var myChart = echarts.init(chartDom);
  var option;

  myChart.showLoading();

  request.axios.get('/world-map/world.json').then(json => {
    myChart.hideLoading();
    echarts.registerMap('world', json);
    option = {
      // 鼠标悬浮提示框
      tooltip: {
        trigger: 'item',
        borderColor: '#666', //区域边框颜色
        formatter: function (params) {
          if (params.name) {
            return params.name + ' : ' + (isNaN(params.value) ? 0 : parseInt(params.value));
          }
        }
      },
      visualMap: {
        min: 0, //最小值
        max: 1000, //最大值
        orient: 'horizontal', //图例排列方向
        // orient: "vertical", //图例模式
        left: 26,
        bottom: 20,
        showLabel: true, //显示图例文本
        precision: 0, //数据展示无小数点
        itemWidth: 12, //图例宽度
        itemHeight: 12, //图例高度
        textGap: 10, //图例间距
        inverse: false, //数据反向展示
        hoverLink: true, //鼠标悬浮
        inRange: {
          //选中图例后背景半透明
          color: ['rgba(3,4,5,0.4)'],
          symbol: 'rect' //更改图元样式
        },
        pieces: [
          {
            gt: 1001,
            label: '>1000',
            color: '#004bbc'
          },
          {
            gte: 500,
            lte: 1000,
            label: '500-1000',
            color: '#237bff'
          },
          {
            gte: 100,
            lte: 499,
            label: '100-499',
            color: '#35a9ff'
          },
          {
            gte: 10,
            lte: 99,
            label: '10-99',
            color: '#73c1ff'
          },
          {
            gte: 1,
            lte: 9,
            label: '1-9',
            color: '#b4deff'
          },
          {
            lte: 0,
            label: '0',
            color: '#d2ecf1'
          }
        ],
        textStyle: {
          color: '#fff',
          fontSize: 14, //图元字体大小
          fontWeight: 500
        }
      },
      series: [
        {
          name: 'World',
          type: 'map',
          map: 'world',
          zoom: 1.2, //地图大小
          roam: false, //禁止拖拽
          data: []
        }
      ]
    };
    myChart.setOption(option);
  });
});
</script>

<style lang="scss" scoped>
#word-map {
  height: 1140px;
  width: 100%;
}
</style>
