<template>
  <div class="other-panel">
    <div class="map-list-panel">
      <ul>
        <li class="header">
          <p>浏览器</p>
          <p>统计量</p>
        </li>
        <li class="li-value" v-for="item in props.browserList" :key="item.name">
          <div class="flex-start-center">
            <el-image class="li-icon" :src="`/icon/browser/${item.icon}`" lazy></el-image>
            <p>{{ item.name }}</p>
          </div>
          <div class="map-value flex-end-center">
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

      <div class="fotter-more">
        <span @click="showMorePanel(props.browserKey)" class="flex-center-center">
          查看更多
          <el-icon style="padding-left: 5px">
            <component :is="'d-arrow-right'"></component>
          </el-icon>
        </span>
      </div>

      <loading-line v-if="props.loading" :loading="props.loading" :show-text="true" text="浏览器统计数据加载中..."></loading-line>
    </div>
    <div class="map-list-panel border-left border-right">
      <ul>
        <li class="header">
          <p>系统</p>
          <p>统计量</p>
        </li>
        <li class="li-value" v-for="item in props.osList" :key="item.name">
          <div class="flex-start-center">
            <el-image class="li-icon" :src="`/icon/os/${item.icon}`" lazy></el-image>
            <p>{{ item.name }}</p>
          </div>
          <div class="map-value flex-end-center">
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

      <div class="fotter-more">
        <span @click="showMorePanel(props.osKey)" class="flex-center-center">
          查看更多
          <el-icon style="padding-left: 5px">
            <component :is="'d-arrow-right'"></component>
          </el-icon>
        </span>
      </div>

      <loading-line v-if="props.loading" :loading="props.loading" :show-text="true" text="系统统计数据加载中..."></loading-line>
    </div>
    <div class="map-list-panel">
      <ul>
        <li class="header">
          <p>设备</p>
          <p>统计量</p>
        </li>
        <li class="li-value" v-for="item in props.deviceList" :key="item.name">
          <div class="browser flex-start-center">
            <el-image class="li-icon" :src="`/icon/device/${item.icon}`" lazy></el-image>
            <p>{{ item.name }}</p>
          </div>
          <div class="map-value flex-end-center">
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

      <div class="fotter-more">
        <span @click="showMorePanel(props.deviceKey)" class="flex-center-center">
          查看更多
          <el-icon style="padding-left: 5px">
            <component :is="'d-arrow-right'"></component>
          </el-icon>
        </span>
      </div>

      <loading-line v-if="props.loading" :loading="props.loading" :show-text="true" text="设备统计数据加载中..."></loading-line>
    </div>
  </div>
</template>

<script setup>
import loadingLine from '@/components/loadings/loading-line.vue';

const props = defineProps({
  loading: {
    type: Boolean,
    require: true
  },
  browserKey: {
    type: String,
    require: true
  },
  browserList: {
    type: Array,
    require: true
  },
  osKey: {
    type: String,
    require: true
  },
  osList: {
    type: Array,
    require: true
  },
  deviceKey: {
    type: String,
    require: true
  },
  deviceList: {
    type: Array,
    require: true
  }
});

const emit = defineEmits(['showPanel']);

const showMorePanel = panel => {
  emit('showPanel', panel);
};
</script>

<style lang="scss" scoped>
.other-panel {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  grid-template-rows: var(--panel-other-height);

  @media (max-width: 1920px) {
    --map-value-width: 150px;
    --map-process-width: 75px;
  }

  .map-list-panel {
    position: relative;
    height: 100%;
    width: 100%;
    padding: 10px 15px;

    &:nth-child(2) {
      padding: 10px 10px;
    }

    ul {
      height: 35px * 6;
      overflow: hidden;

      li {
        display: flex;
        justify-content: space-between;
        align-items: center;
        height: 35px;
        padding: 5px 10px;
      }
    }

    .header {
      cursor: default;
      padding-bottom: 10px;

      > p:last-child {
        padding-right: 70px;

        @media (max-width: 1920px) {
          padding-right: 50px;
        }
      }
    }

    .li-value {
      cursor: default;

      > div:first-child {
        p {
          padding-left: 5px;
        }
      }
    }
  }
}
</style>
