<template>
  <div class="browse-map">
    <ul class="browse-route border-right">
      <li class="header">
        <span>网页</span>
        <span>浏览量</span>
      </li>

      <li v-for="item in model.browse" :key="item.route">
        <div>
          <p>{{ item.route }}</p>
        </div>
        <div>
          {{ item.count }}
        </div>
      </li>
    </ul>

    <ul class="browse-referer border-left">
      <li class="header">
        <span>来源域名</span>
        <span>浏览量</span>
      </li>

      <li v-for="item in model.referer" :key="item.referer">
        <div>
          <p>{{ item.referer }}</p>
        </div>
        <div>
          {{ item.count }}
        </div>
      </li>
    </ul>
  </div>
</template>

<script setup>
import { reactive } from 'vue';
import loadingLine from '@/components/loadings/loading-line.vue';

const model = reactive({
  browse: [
    {
      route: '/',
      count: 50
    },
    {
      route: '/Count',
      count: 50
    }
  ],
  referer: [
    {
      referer: 'github.com',
      count: 20
    }
  ]
});
</script>

<style lang="scss" scoped>
.browse-map {
  padding: 20px 0;
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  grid-auto-rows: 500px;

  .browse-route,
  .browse-referer {
    padding: 0 20px;
    position: relative;

    li {
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding: 5px 0;

      &.header {
        padding-bottom: 15px;
      }
    }
  }
}
</style>
