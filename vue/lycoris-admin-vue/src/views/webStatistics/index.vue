<template>
  <page-layout :loading="model.modelloading">
    <browse-map class="map-item"></browse-map>
    <world-map class="map-item"></world-map>
  </page-layout>
</template>

<script setup name="web-statistics">
import { onMounted, reactive } from 'vue';
import PageLayout from '../layout/page-layout.vue';
import browseMap from './components/browse-map.vue';
import worldMap from './components/world-map.vue';

const model = reactive({
  loading: false
});

onMounted(() => {
  //
});
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
}
</style>
