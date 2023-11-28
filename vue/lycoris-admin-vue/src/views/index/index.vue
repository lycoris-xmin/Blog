<template>
  <page-layout :show-header="false">
    <statistics-card></statistics-card>
    <server-monitor ref="serverMonitorRef"></server-monitor>
    <web-charts ref="webChartsRef"></web-charts>
    <world-map></world-map>
  </page-layout>
</template>

<script setup name="dashboard">
import { reactive, onMounted, onUnmounted, ref } from 'vue';
import pageLayout from '../layout/page-layout.vue';
import statisticsCard from './components/statistics-card.vue';
import serverMonitor from './components/server-monitor.vue';
import webCharts from './components/web-charts.vue';
import worldMap from './components/world-map.vue';

const serverMonitorRef = ref();
const webChartsRef = ref();

const model = reactive({
  loading: false
});

onMounted(() => {
  try {
    window.addEventListener('resize', chartResize);
  } finally {
    model.loading = false;
  }
});

onUnmounted(() => {
  window.removeEventListener('resize', chartResize);
});

function chartResize() {
  serverMonitorRef.value.chartResize();
  webChartsRef.value.chartResize();
}
</script>

<style lang="scss" scoped>
.statistic-group {
  padding: 10px 0;
  display: grid;
  grid-template-columns: repeat(6, 342px);
  grid-gap: 20px;

  @media (max-width: 1920px) {
    grid-gap: 10px;
    grid-template-columns: repeat(6, 1fr);
  }

  @media (max-width: 1440px) {
    grid-gap: 25px;
    grid-template-columns: repeat(3, 1fr);
  }

  .statistic-item {
    --statistic-bg-color: #fff;
    --statistic-border-color: #fff;
    background-color: var(--statistic-bg-color);
    border: 1px solid var(--statistic-border-color);
    color: #fff;

    :deep(.el-statistic__number) {
      color: #fff;
    }
  }

  .statistic-item:hover {
    box-shadow: 0 0 8px 4px var(--statistic-border-color) !important;
  }

  .statistic-item:nth-child(1) {
    --statistic-bg-color: var(--color-info);
    --statistic-border-color: var(--color-info-light);
  }

  .statistic-item:nth-child(2) {
    --statistic-bg-color: var(--color-success);
    --statistic-border-color: var(--color-success-light);
  }

  .statistic-item:nth-child(3) {
    --statistic-bg-color: var(--color-danger);
    --statistic-border-color: var(--color-danger-light);
  }

  .statistic-item:nth-child(4) {
    --statistic-bg-color: var(--color-purple);
    --statistic-border-color: var(--color-purple-light);
  }

  .statistic-item:nth-child(5) {
    --statistic-bg-color: var(--color-primary);
    --statistic-border-color: var(--color-primary-light);
  }

  .statistic-item:nth-child(6) {
    --statistic-bg-color: var(--color-warning);
    --statistic-border-color: var(--color-warning-light);
  }
}
</style>
