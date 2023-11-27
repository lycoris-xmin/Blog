<template>
  <div>
    <feature :loadIndex="0" @load-complete="pageLoadComplete"></feature>

    <div class="post-line flex-start-center">
      <el-icon :size="24">
        <component :is="'notebook'"></component>
      </el-icon>
      <span>博客文章</span>
    </div>

    <div class="category-header" ref="categoryHeader">
      <post-category :loadIndex="1" @load-complete="pageLoadComplete" @change="categoryChange"></post-category>
    </div>

    <div class="card post-list" ref="post">
      <transition-list tag="ul">
        <li v-for="(item, index) in model.list" :key="item.id" :data-index="index"><post-data :post="item"></post-data></li>
      </transition-list>
      <div class="pagination flex-center-center" v-if="model.scroll > 3">
        <el-pagination v-model:current-page="model.pageIndex" :page-size="model.pageSize" :pager-count="5" layout="prev, pager, next" :total="model.count" :hide-on-single-page="true" @current-change="pageChange" />
      </div>
    </div>

    <navigator-tool></navigator-tool>
  </div>
</template>

<script setup name="home">
import { nextTick, onMounted, reactive, ref } from 'vue';
import feature from './components/feature.vue';
import postCategory from './components/post-category.vue';
import postData from './components/post-data.vue';
import navigatorTool from '@/components/navigator-tool/index.vue';
import transitionList from '@/components/transitions/list-transition.vue';
import { getPostList } from '@/api/post';

const categoryHeader = ref();
const post = ref();

const model = reactive({
  categoryFilter: '',
  count: 0,
  list: [],
  pageIndex: 1,
  pageSize: 10
});

const componentsLoad = [false, false, false];

const emit = defineEmits(['loading', 'browse']);

onMounted(async () => {
  try {
    await getList();
  } finally {
    pageLoadComplete(2);
    emit('browse');
  }
});

const getList = async () => {
  try {
    let res = await getPostList({
      category: model.categoryFilter,
      pageIndex: model.pageIndex,
      pageSize: model.pageSize
    });
    if (res && res.resCode == 0) {
      model.count = res.data.count;
      model.list = res.data.list;
    }
  } catch (error) {}
};

const pageLoadComplete = index => {
  if (index >= 0 && index < componentsLoad.length) {
    componentsLoad[index] = true;
    if (componentsLoad.filter(x => x == false).length == 0) {
      emit('loading', false);
    }
  }
};

const categoryChange = categoryFilter => {
  model.categoryFilter = categoryFilter;
  getList();

  if (categoryHeader.value.getBoundingClientRect().top <= 10) {
    nextTick(() => {
      window.scrollTo(0, post.value.offsetTop);
    });
  }
};

const pageChange = async index => {
  model.pageIndex = index;
  await getList();
};
</script>

<style lang="scss" scoped>
.post-line {
  margin: 20px 10px;
  width: 140px;
  gap: 10px;
  background: var(--main-linear-gradient) no-repeat left bottom;
  background-size: 100% 3px;

  .el-icon {
    color: var(--main-text-color);
  }

  span {
    font-size: 24px;
    color: var(--main-text-color);
  }
}

.category-header {
  padding: 0;
  position: sticky;
  top: 0;
  z-index: 2;
  // background-color: var(--main-background-color);
}

.post-list {
  margin: 20px 0;
  min-height: 100px;
  padding: 20px;
  > ul {
    > li {
      list-style: none;
    }
  }

  .pagination {
    padding: 20px;

    :deep(ul.el-pager) {
      li {
        font-size: 16px;
      }
    }
  }
}
</style>
