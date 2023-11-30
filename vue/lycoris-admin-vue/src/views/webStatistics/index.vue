<template>
  <page-layout :loading="model.modelloading">
    <world-map class="map-item"></world-map>
    <transition name="router_animate" mode="out-in">
      <div v-if="!model.showMore">
        <browse-map :web-path="webPath" class="map-item" @brwse-list="handleBrowseList" @referer-list="handleRefererList" @show-panel="showPanel"></browse-map>
        <div style="height: 800px"></div>
      </div>
      <div class="more-panel-body" v-else>
        <ul class="panel-menu border-right">
          <li v-for="item in model.panelMenu" :key="item.key">
            <p @click="model.showPanel = item.key" :class="{ active: model.showPanel == item.key }">{{ item.name }}</p>
          </li>
        </ul>
        <transition name="fade" mode="out-in">
          <ul class="panel-list-body" v-if="model.showPanel == 'browse'">
            <li>这是浏览</li>
          </ul>
          <ul class="panel-list-body" v-else>
            <li>这是来源</li>
          </ul>
        </transition>
        <div class="flex-center-center">
          <el-button @click="model.showMore = false">返回</el-button>
        </div>
        <div class="flex-center-center">
          <el-button @click="model.showMore = false">返回</el-button>
        </div>
      </div>
    </transition>
  </page-layout>
</template>

<script setup name="web-statistics">
import { onMounted, reactive, ref, inject } from 'vue';
import PageLayout from '../layout/page-layout.vue';
import browseMap from './components/browse-map.vue';
import worldMap from './components/world-map.vue';

const webPath = ref(inject('$webPath'));

const model = reactive({
  loading: false,
  showMore: false,
  showPanel: '',
  panelMenu: [
    {
      key: 'browse',
      name: '访问统计'
    },
    {
      key: 'referer',
      name: '来源统计'
    }
  ],
  browseList: {},
  refererList: {}
});

onMounted(() => {
  //
});

const handleBrowseList = data => {
  model.browseList = data;
};

const handleRefererList = data => {
  model.refererList = data;
};

const showPanel = panelName => {
  model.showPanel = panelName;
  model.showMore = true;
};
</script>

<style lang="scss" scoped>
.map-item {
  border-bottom: 1px solid var(--color-secondary);

  &:last-child {
    border: 0;
  }
}
</style>

<style lang="scss">
.map-item {
  position: relative;
  --map-value-width: 250px;

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
    .map-value {
      div:first-child {
        max-width: calc(var(--map-value-width) - 100px);
        padding-right: 5px;
        text-align: right;
        overflow: hidden;
        text-overflow: ellipsis;
        word-break: break-all;
        white-space: nowrap;
      }

      .percent {
        position: relative;
        width: 100px;
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
  }
}

.more-panel-body {
}
</style>

<style>
.map-item {
  --panel-group-height: 500px;
}
</style>
