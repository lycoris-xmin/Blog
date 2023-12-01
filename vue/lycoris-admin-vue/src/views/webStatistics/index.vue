<template>
  <page-layout :loading="model.modelloading">
    <world-map class="map-item"></world-map>
    <transition name="router_animate" mode="out-in">
      <div v-if="!model.showMore">
        <browse-statistics
          class="map-item p-tb-20"
          :browse-key="panelKey.BROWSE"
          :browse-loading="browseTable.loading"
          :browse-list="browseTable.firstList"
          :referer-key="panelKey.REFERER"
          :referer-loading="refererTable.loading"
          :referer-list="refererTable.firstList"
          @route-to-page="routeToPage"
          @show-panel="showPanel"
        ></browse-statistics>
        <otherStatistics
          class="map-item p-tb-20"
          :loading="model.userAgentLoading"
          :browser-key="panelKey.BROWSECLIENT"
          :browser-list="model.userAgent.browseClient"
          :os-key="panelKey.OS"
          :os-list="model.userAgent.os"
          :device-key="panelKey.DEVICE"
          :device-list="model.userAgent.device"
          @show-panel="showPanel"
        ></otherStatistics>
      </div>
      <div class="map-item more-panel-body p-tb-20" v-else>
        <ul class="panel-menu border-right">
          <li v-for="item in model.panelMenu" :key="item.key" class="flex-center-center">
            <p @click="model.showPanel = item.key" :class="{ active: model.showPanel == item.key }">{{ item.name }}</p>
          </li>
        </ul>
        <transition name="fade" mode="out-in">
          <ul class="panel-list-body" v-if="model.showPanel == panelKey.BROWSE">
            <li class="browse-li">
              <p class="panel-header">访问地址</p>
              <p class="panel-header">访问页面</p>
              <p class="panel-header text-right">浏览量</p>
            </li>
            <li class="browse-li panel-item" v-for="item in browseTable.list" :key="item.route">
              <p class="route-to" @click="routeToPage(item.route)">{{ item.route }}</p>
              <p>{{ item.pageName }}</p>
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
            <loading-line v-if="browseTable.loading || browseTable.list.length < browseTable.count" :loading="browseTable.loading" :show-text="true" text="数据加载中..."></loading-line>
          </ul>
          <ul class="panel-list-body" v-else-if="model.showPanel == panelKey.REFERER">
            <li class="referer-li">
              <p class="panel-header">来源域名</p>
              <p class="panel-header text-right">统计量</p>
            </li>
            <li class="referer-li panel-item" v-for="item in refererTable.list" :key="item.domain">
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
            <loading-line :loading="browseTable.loading" :show-text="true" text="数据加载中..."></loading-line>
          </ul>
          <ul class="panel-list-body" v-else-if="model.showPanel == panelKey.BROWSECLIENT">
            <li class="other-li">
              <p class="panel-header">浏览器</p>
              <p class="panel-header text-right">统计量</p>
            </li>

            <li class="other-li panel-item" v-for="item in model.userAgent.browseClient" :key="item.name">
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
          <ul class="panel-list-body" v-else-if="model.showPanel == panelKey.OS">
            <li class="other-li">
              <p class="panel-header">系统</p>
              <p class="panel-header text-right">统计量</p>
            </li>

            <li class="other-li panel-item" v-for="item in model.userAgent.os" :key="item.name">
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
          <ul class="panel-list-body" v-else>
            <li class="other-li">
              <p class="panel-header">设备</p>
              <p class="panel-header text-right">统计量</p>
            </li>

            <li class="other-li panel-item" v-for="item in model.userAgent.device" :key="item.name">
              <div class="flex-start-center">
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
        </transition>
        <div class="flex-center-center">
          <el-button @click="model.showMore = false">返回</el-button>
        </div>
        <div class="flex-center-center">
          <el-pagination
            v-if="model.showPanel == panelKey.BROWSE && browseTable.count > browseTable.list.length"
            background
            :hide-on-single-page="true"
            v-model:current-page="browseTable.pageIndex"
            :total="browseTable.count"
            :page-size="browseTable.pageSize"
            :layout="'prev, pager, next'"
            @current-change="e => pageChange(panelKey.BROWSE, e)"
          />
          <el-pagination
            v-else-if="model.showPanel == panelKey.REFERER && refererTable.count > refererTable.list.length"
            background
            :hide-on-single-page="true"
            v-model:current-page="refererTable.pageIndex"
            :total="refererTable.count"
            :page-size="refererTable.pageSize"
            :layout="'prev, pager, next'"
            @current-change="e => pageChange(panelKey.REFERER, e)"
          />
        </div>
      </div>
    </transition>
  </page-layout>
</template>

<script setup name="web-statistics">
import { onMounted, reactive, ref, inject, onActivated, onDeactivated } from 'vue';
import pageLayout from '../layout/page-layout.vue';
import worldMap from './components/world-map.vue';
import browseStatistics from './components/browse-statistics.vue';
import otherStatistics from './components/other-statistics.vue';
import loadingLine from '@/components/loadings/loading-line.vue';
import { getBrowseStatisticsList, getRefererStatisticsList, getUserAgentStatisticsList } from '@/api/webStatistics';
import { debounce } from '../../utils/tool';

const panelKey = {
  BROWSE: 'browse',
  REFERER: 'referer',
  BROWSECLIENT: 'browser',
  OS: 'os',
  DEVICE: 'device'
};

const webPath = ref(inject('$webPath'));

const model = reactive({
  loading: false,
  showMore: false,
  showPanel: 'browse',
  panelMenu: [
    {
      key: panelKey.BROWSE,
      name: '访问统计'
    },
    {
      key: panelKey.REFERER,
      name: '来源统计'
    },
    {
      key: panelKey.BROWSECLIENT,
      name: '浏览器统计'
    },
    {
      key: panelKey.OS,
      name: '系统统计'
    },
    {
      key: panelKey.DEVICE,
      name: '设备统计'
    }
  ],
  userAgentLoading: true,
  userAgent: {
    browseClient: [],
    os: [],
    device: []
  }
});

const browseTable = reactive({
  pageIndex: 1,
  pageSize: 20,
  count: 0,
  firstList: [],
  list: [],
  summary: 0,
  sum: true,
  loading: true
});

const refererTable = reactive({
  pageIndex: 1,
  pageSize: 20,
  count: 0,
  firstList: [],
  list: [],
  summary: 0,
  sum: true,
  loading: true
});

onMounted(() => {
  //
  getBrowseMapList(true);

  getRefererMapList(true);

  getUserAgentMapList();
});

onActivated(() => {
  // signalr 监听
});

onDeactivated(() => {
  // signalr 移除监听
});

const getBrowseMapList = async (first = false) => {
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

      if (first) {
        browseTable.firstList = [...browseTable.list];
      }
    }
  } finally {
    browseTable.loading = false;
  }
};

const getRefererMapList = async (first = false) => {
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
          domain: x.domain,
          count: x.count,
          percent: ((x.count / refererTable.summary) * 100).toFixed(2)
        };
      });

      if (first) {
        refererTable.firstList = refererTable.list.map(x => {
          return {
            domain: x.domain,
            count: x.count,
            percent: x.percent
          };
        });
      }
    }
  } finally {
    refererTable.loading = false;
  }
};

const getUserAgentMapList = async () => {
  try {
    let res = await getUserAgentStatisticsList();
    if (res && res.resCode == 0) {
      if (res.data.browseClient && res.data.browseClient.length) {
        const clientTotal = res.data.browseClient.map(x => x.count).reduce((total, count) => (total += count));
        model.userAgent.browseClient = res.data.browseClient.map(x => {
          return {
            name: x.name,
            icon: x.icon,
            count: x.count,
            percent: ((x.count / clientTotal) * 100).toFixed(2)
          };
        });
      }

      if (res.data.os && res.data.os.length) {
        const osTotal = res.data.os.map(x => x.count).reduce((total, count) => (total += count));

        model.userAgent.os = res.data.os.map(x => {
          return {
            name: x.name,
            icon: x.icon,
            count: x.count,
            percent: ((x.count / osTotal) * 100).toFixed(2)
          };
        });
      }

      if (res.data.device && res.data.device.length) {
        const deviceTotal = res.data.device.map(x => x.count).reduce((total, count) => (total += count));

        model.userAgent.device = res.data.device.map(x => {
          return {
            name: x.name,
            icon: x.icon,
            count: x.count,
            percent: ((x.count / deviceTotal) * 100).toFixed(2)
          };
        });
      }
    }
  } finally {
    model.userAgentLoading = false;
  }
};

const showPanel = panelName => {
  model.showPanel = panelName;
  model.showMore = true;
};

const routeToPage = path => {
  if (path.startsWith('http')) {
    window.open(path);
  } else {
    const url = `${webPath.value}${path}`;
    window.open(url);
  }
};

const pageChange = debounce((key, index) => {
  if (key == panelKey.BROWSE) {
    //
    if (browseTable.pageIndex != index) {
      browseTable.pageIndex = index;
      getBrowseMapList();
    }
  } else {
    if (refererTable.pageIndex != index) {
      refererTable.pageIndex = index;
      getRefererMapList();
    }
  }
}, 500);
</script>

<style lang="scss" scoped>
.map-item {
  border-bottom: 1px solid var(--color-secondary);

  &:last-child {
    border: 0;
  }

  &.p-tb-20 {
    padding: 20px 0;
  }
}

.more-panel-body {
  display: grid;
  grid-template-columns: 200px 1fr;
  grid-template-rows: calc(var(--panel-group-height) + var(--panel-other-height));

  .panel-menu {
    > li {
      padding: 10px;
      > p {
        text-align: center;
        width: 100%;
        padding: 10px;
        border-radius: 8px;
        cursor: pointer;
        transition: all 0.4s;

        &.active,
        &:hover {
          color: #fff;
          background-color: var(--color-info);
        }
      }
    }
  }

  .panel-list-body {
    position: relative;
    padding: 10px 20px;
    overflow-y: auto;

    p.panel-header {
      font-size: 18px;
      padding-bottom: 10px;

      &.text-right {
        text-align: right;
        padding-right: 70px;
      }
    }

    .panel-item {
      padding: 8px 0;

      p {
        cursor: default;

        &.route-to {
          cursor: pointer;
          transition: all 0.4s;

          &:hover {
            color: var(--color-purple);
          }
        }
      }

      > div:first-child {
        p {
          padding-left: 5px;
        }
      }
    }

    .browse-li {
      display: grid;
      grid-template-columns: 2fr 3fr 250px;
      grid-gap: 20px;
    }

    .referer-li {
      display: grid;
      grid-template-columns: 5fr 250px;
      grid-gap: 20px;
    }

    .other-li {
      display: flex;
      justify-content: space-between;
      align-items: center;
    }
  }
}
</style>

<style lang="scss">
.map-item {
  position: relative;

  --panel-group-height: 610px;
  --panel-other-height: 435px;
  --panel-footer-height: 45px;
  --map-value-width: 250px;
  --map-process-width: 100px;

  @media (max-width: 1920px) {
    --panel-group-height: 430px;
    --panel-other-height: 270px;
  }

  .border {
    border: 1px solid var(--color-secondary);
  }

  .border-left {
    border-left: 1px solid var(--color-secondary);
  }

  .border-right {
    border-right: 1px solid var(--color-secondary);
  }

  .border-bottom {
    border-bottom: 1px solid var(--color-secondary);
  }

  .border-dark {
    border-color: var(--color-dark);
  }

  li {
    list-style: none;

    .map-value {
      width: var(--map-value-width);

      div:first-child {
        max-width: calc(var(--map-value-width) - var(--map-process-width));
        padding-right: 5px;
        text-align: right;
        overflow: hidden;
        text-overflow: ellipsis;
        word-break: break-all;
        white-space: nowrap;
      }

      .percent {
        position: relative;
        width: var(--map-process-width);
        padding-left: 5px;
        text-align: left;

        .process-bar {
          position: absolute;
          height: 100%;
          margin: 0;
          padding: 0;
          left: 0;
          border: 0;
          opacity: 0.6;
          background-color: var(--color-info-light);
        }

        &::after {
          content: '%';
        }
      }
    }

    .li-icon {
      display: flex;
      justify-content: center;
      align-items: center;
      min-height: 20px;
      min-width: 20px;

      img {
        height: 20px;
        width: 20px;
        object-fit: cover;
      }
    }
  }

  .fotter-more {
    text-align: center;
    padding: 10px;
    height: var(--panel-footer-height);

    > span {
      cursor: pointer;
      transition: color 0.3s;

      &:hover {
        color: var(--color-purple);
      }
    }
  }
}
</style>
