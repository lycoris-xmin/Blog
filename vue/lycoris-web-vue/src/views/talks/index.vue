<template>
  <page-layout title="说说" icon="chat-dot-round">
    <div class="talks-container">
      <transition-list class="talks-list">
        <li v-for="item in model.list" :key="item.id">
          <talk-data :data="item" :owner="stores.owner"></talk-data>
        </li>
        <li v-if="!model.list.length">
          <div class="card no-talk-card">博主太懒，没有发布过说说</div>
        </li>
      </transition-list>
    </div>
  </page-layout>
</template>

<script setup name="talks">
import { onMounted, reactive } from 'vue';
import pageLayout from '../layout/page-layout.vue';
import transitionList from '../../components/transitions/list-transition.vue';
import talkData from './components/talk-data.vue';
import { getTalkList } from '../../api/talk';
import { stores } from '../../stores';

const model = reactive({
  count: 0,
  list: [],
  pageIndex: 1,
  pageSize: 10
});

const emit = defineEmits(['loading', 'browse']);

onMounted(async () => {
  try {
    await getList();
  } finally {
    emit('loading', false);
    emit('browse');
  }
});

const getList = async () => {
  let res = await getTalkList({
    pageIndex: model.pageIndex,
    pageSize: model.pageSize
  });

  if (res && res.resCode == 0) {
    model.count = res.data.count;
    model.list = res.data.list;
  }
};
</script>

<style lang="scss" scoped>
.talks-container {
  width: 100%;
  padding: 10px;

  .talks-list {
    li {
      margin-bottom: 20px;
      list-style: none;
    }

    > li:last-child {
      margin-bottom: 0;
    }

    .no-talk-card {
      font-size: 16px;
      font-weight: 500;
    }
  }
}
</style>
