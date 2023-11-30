<template>
  <div class="browse-map">
    <div class="map-list-panel border-right">
      <ul>
        <li class="header">
          <span class="borwse-show-change flex-start-center">
            {{ browseTable.showRoute ? '访问地址' : '访问页面' }}
            <el-tooltip effect="dark" content="切换显示" placement="top">
              <el-icon @click="browseTable.showRoute = !browseTable.showRoute">
                <component :is="'switch'"></component>
              </el-icon>
            </el-tooltip>
          </span>
          <span>浏览量</span>
        </li>

        <li v-for="item in browseTable.list" :key="item.route" class="li-value" @click="routeToPage(item.route)">
          <p>{{ browseTable.showRoute ? item.route : item.pageName }}</p>
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

      <div class="fotter-more">
        <span @click="showMorePanel('browse')">查看更多</span>
      </div>
      <loading-line v-if="browseTable.loading" :loading="browseTable.loading" :show-text="true" text="访问数据加载中..."></loading-line>
    </div>

    <div class="map-list-panel border-left">
      <ul>
        <li class="header">
          <span>来源域名</span>
          <span>浏览量</span>
        </li>

        <li v-for="item in refererTable.list" :key="item.referer" class="li-value">
          <p>{{ item.referer }}</p>
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
      <div class="fotter-more">
        <span @click="showMorePanel('referer')">查看更多</span>
      </div>
      <loading-line v-if="refererTable.loading" :loading="refererTable.loading" :show-text="true" text="来源数据加载中..."></loading-line>
    </div>
  </div>
</template>

<script setup>
import { onMounted, reactive } from 'vue';
import loadingLine from '@/components/loadings/loading-line.vue';
import { getBrowseStatisticsList, getRefererStatisticsList } from '@/api/webStatistics';

const emit = defineEmits(['browseList', 'refererList', 'showPanel']);

const props = defineProps({
  webPath: {
    type: String,
    require: true
  }
});

const browseTable = reactive({
  pageIndex: 1,
  pageSize: 15,
  count: 0,
  list: [],
  summary: 0,
  sum: true,
  showRoute: false,
  loading: true
});

const refererTable = reactive({
  pageIndex: 1,
  pageSize: 15,
  count: 0,
  list: [],
  summary: 0,
  sum: true,
  loading: true
});

onMounted(() => {
  getbrowseTableList();
  getrefererTableList();
});

const getbrowseTableList = async () => {
  browseTable.loading = true;

  try {
    let res = await getBrowseStatisticsList({ ...browseTable });
    if (res && res.resCode == 0) {
      browseTable.count = res.data.count;

      if (browseTable.sum) {
        browseTable.summary = res.data.summary.count || 0;
        browseTable.sum = false;
      }

      browseTable.list = res.data.list.map(x => {
        return {
          route: x.route,
          pageName: x.pageName,
          count: x.count,
          percent: ((x.count / browseTable.summary) * 100).toFixed(2)
        };
      });
    }
  } finally {
    browseTable.loading = false;
    emit('browseList', { ...browseTable });
  }
};

const getrefererTableList = async () => {
  refererTable.loading = true;

  try {
    let res = await getRefererStatisticsList({ ...refererTable });
    if (res && res.resCode == 0) {
      refererTable.count = res.data.count;

      if (refererTable.sum) {
        refererTable.summary = res.data.summary.count || 0;
        refererTable.sum = false;
      }

      refererTable.list = res.data.list.map(x => {
        return {
          referer: x.referer,
          count: x.count,
          percent: ((x.count / refererTable.summary) * 100).toFixed(2)
        };
      });
    }
  } finally {
    refererTable.loading = false;
    emit('refererList', { ...refererTable });
  }
};

const routeToPage = path => {
  //
  const url = `${props.webPath}${path}`;
  window.open(url);
};

const showMorePanel = panel => {
  emit('showPanel', panel);
};
</script>

<style lang="scss" scoped>
$row-footer-heigth: 45px;

.browse-map {
  padding: 20px 0;
  display: grid;
  grid-template-columns: repeat(2, 50%);
  grid-auto-rows: var(--panel-group-height);

  .map-list-panel {
    padding: 0 20px;
    position: relative;
  }

  .fotter-more {
    text-align: center;
    padding: 10px 0;
    height: $row-footer-heigth;

    > span {
      cursor: pointer;
      transition: color 0.3s;

      &:hover {
        color: var(--color-purple);
      }
    }
  }
}

.map-list-panel {
  ul {
    height: calc(var(--panel-group-height) - $row-footer-heigth);
    overflow: hidden;
  }

  li {
    display: flex;
    justify-content: space-between;
    align-items: center;
    padding: 5px 10px;
    border-radius: 5px;

    &.header {
      padding-left: 0;
      padding-right: 0;
      padding-bottom: 15px;

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

      span:last-child {
        padding-right: 80px;
      }
    }
  }

  .li-value {
    cursor: pointer;

    > p:first-child {
      overflow: hidden;
      text-overflow: ellipsis;
      word-break: break-all;
      white-space: nowrap;
      padding: 0 10px 0 0;
      transition: color 0.25s;
      max-width: calc(100% - var(--map-value-width));

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
