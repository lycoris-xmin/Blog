<template>
  <div class="statistics-map">
    <div class="map-list-panel">
      <ul>
        <li class="header">
          <span class="borwse-show-change flex-start-center">
            {{ model.showRoute ? '访问地址' : '访问页面' }}
            <el-tooltip effect="dark" content="切换显示" placement="top">
              <el-icon @click="model.showRoute = !model.showRoute">
                <component :is="'switch'"></component>
              </el-icon>
            </el-tooltip>
          </span>
          <span>浏览量</span>
        </li>

        <li v-for="item in props.browseList" :key="item.route" class="li-value" @click="routeToPage(item.route)">
          <p>{{ model.showRoute ? item.route : item.pageName }}</p>
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
        <span @click="showMorePanel(props.browseKey)" class="flex-center-center">
          查看更多
          <el-icon style="padding-left: 5px">
            <component :is="'d-arrow-right'"></component>
          </el-icon>
        </span>
      </div>
      <loading-line v-if="props.browseLoading" :loading="props.browseLoading" :show-text="true" text="访问数据加载中..."></loading-line>
    </div>

    <div class="map-list-panel border-left">
      <ul>
        <li class="header">
          <span>来源域名</span>
          <span>统计量</span>
        </li>

        <li v-for="item in props.refererList" :key="item.referer" class="li-value">
          <p>{{ item.domain }}</p>
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
        <span @click="showMorePanel(props.refererKey)" class="flex-center-center">
          查看更多
          <el-icon style="padding-left: 5px">
            <component :is="'d-arrow-right'"></component>
          </el-icon>
        </span>
      </div>
      <loading-line v-if="props.refererLoading" :loading="props.refererLoading" :show-text="true" text="来源数据加载中..."></loading-line>
    </div>
  </div>
</template>

<script setup>
import { reactive } from 'vue';
import loadingLine from '@/components/loadings/loading-line.vue';

const emit = defineEmits(['showPanel', 'routeToPage']);

const props = defineProps({
  browseKey: {
    type: String,
    require: true
  },
  browseList: {
    type: Array,
    require: true
  },
  browseLoading: {
    type: Boolean,
    require: true
  },
  refererKey: {
    type: String,
    require: true
  },
  refererList: {
    type: Array,
    require: true
  },
  refererLoading: {
    type: Boolean,
    require: true
  }
});

const model = reactive({
  showRoute: false
});

const routeToPage = path => {
  emit('routeToPage', path);
};

const showMorePanel = panel => {
  emit('showPanel', panel);
};
</script>

<style lang="scss" scoped>
.statistics-map {
  display: grid;
  grid-template-columns: repeat(2, 50%);
  grid-auto-rows: var(--panel-group-height);

  .map-list-panel {
    padding: 0 20px;
    position: relative;
  }
}

.map-list-panel {
  ul {
    overflow: hidden;
    height: 35px * 16;

    @media (max-width: 1920px) {
      height: 35px * 11;
    }
  }

  li {
    display: flex;
    justify-content: space-between;
    align-items: center;
    height: 35px;
    padding: 5px 10px;

    &.header {
      padding-left: 0;
      padding-right: 0;
      padding-bottom: 15px;
      cursor: default;

      .borwse-show-change {
        cursor: default;

        &.borwse-show-change {
          .el-icon:last-child {
            margin-left: 6px;
            cursor: pointer;
            transition: color 0.4s;

            &:hover {
              color: var(--color-primary);
            }
          }
        }
      }

      span {
        font-size: 16px;
        font-weight: 600;
        letter-spacing: 1.5px;

        &:last-child {
          padding-right: 80px;
        }
      }
    }
  }

  .li-value {
    cursor: default;

    > p:first-child {
      overflow: hidden;
      text-overflow: ellipsis;
      word-break: break-all;
      white-space: nowrap;
      padding: 0 10px 0 0;
      transition: color 0.25s;
      max-width: calc(100% - var(--map-value-width));
      cursor: pointer;

      &:hover {
        color: var(--color-primary);
        font-weight: 600;
      }
    }

    > div:last-child {
      flex-shrink: 1;
    }
  }
}
</style>
