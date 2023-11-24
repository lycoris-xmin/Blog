<template>
  <el-dialog v-model="model.visible" width="800px" :lock-scroll="true" :append-to-body="true" class="el-layout-modal" top="100px" :before-close="beforeClose">
    <div class="search">
      <div class="dialog-title">
        <span>文章搜索</span>
      </div>

      <div class="input" :class="{ focus: computedSearch }">
        <input type="text" v-model="model.keyWord" class="input-el" @focus="model.focus = true" @blur="model.focus = false" autocomplete="off" />
        <span data-placeholder="你想搜点什么呢..."></span>
      </div>

      <transition name="fade">
        <div class="search-result" v-show="model.keyWord.length > 0">
          <ul v-if="model.list && model.list.length">
            <li class="search-post" v-for="(item, index) in model.list" :key="item.id" :data-index="index" @click="toBlog(item.id)">
              <span class="title">{{ item.title }}</span>
              <div class="info">{{ item.info }}</div>
            </li>
          </ul>
          <div v-else-if="model.loaded" class="empty flex-center-center">
            <span>没有找到符合的文章</span>
          </div>
          <loading-line :loading="model.loading" :show-text="true" text="正在快马加鞭查询中,请稍候..."></loading-line>
        </div>
      </transition>
    </div>
  </el-dialog>
</template>

<script setup>
import { reactive, computed, watch } from 'vue';
import { useRouter } from 'vue-router';
import loadingLine from '@/components/loadings/loading-line.vue';
import { searchPost } from '@/api/post';
import { debounce } from '@/utils/tool';

const router = useRouter();

const model = reactive({
  visible: false,
  keyWord: '',
  focus: false,
  list: [],
  loading: false,
  loaded: false
});

const computedSearch = computed(() => {
  return model.focus || model.keyWord.length > 0;
});

const show = () => {
  model.visible = true;
};

const close = () => {
  beforeClose(() => {
    model.visible = false;
  });
};

watch(
  () => model.keyWord,
  (val, oldVal) => {
    if (val == oldVal) {
      return;
    }
    search();
  }
);

const search = debounce(async () => {
  if (model.keyWord && model.keyWord.trim()) {
    model.loaded = false;
    model.loading = true;
    try {
      let res = await searchPost(model.keyWord.trim());
      if (res && res.resCode == 0) {
        model.list = res.data.list || [];
      }
    } finally {
      model.loading = false;
      model.loaded = true;
    }

    router.push({
      path: '/server/error'
    });
  }
}, 500);

const toBlog = id => {
  model.keyWord = '';
  model.list = [];
  model.visible = false;
  router.push({
    path: `/post/${id}`
  });
};

const beforeClose = done => {
  model.keyWord = '';
  model.focus = false;
  model.loading = false;
  model.list = [];
  done();
};

defineExpose({
  show,
  close
});
</script>

<style lang="scss" scoped>
.search {
  position: relative;
  display: flex;
  flex-direction: column;

  .dialog-title {
    span {
      font-size: 24px;
      letter-spacing: 2px;
      line-height: 2px;
      background: linear-gradient(to right, rgba(131, 58, 180, 1) 0%, rgba(253, 29, 29, 1) 33.3%, rgba(252, 176, 69, 1) 66.6%, rgba(131, 58, 180, 1) 100%) no-repeat left bottom;
      background-size: 100% 2px;
      padding: 10px 2px 5px 2px;
      cursor: default;
      margin-bottom: 10px;
    }

    span::before {
      content: '#';
      padding-right: 10px;
      color: var(--color-danger);
    }

    margin-bottom: 8px;
  }

  .input {
    border-bottom: 2px solid #adadad;
    position: relative;
    margin: 30px 0;

    .input-el {
      font-size: 25px;
      color: #333;
      border: none;
      width: 100%;
      outline: none;
      background: none;
      padding: 0 5px;
      height: 40px;
      position: relative;
      z-index: 2;
    }

    .input-el:-internal-autofill-selected {
      -webkit-appearance: menulist-button;
      appearance: menulist-button;
      background-image: none !important;
      background-color: transparent !important;
      color: fieldtext !important;
    }

    span {
      position: static;
    }

    span::before {
      content: attr(data-placeholder);
      position: absolute;
      top: 50%;
      left: 5px;
      font-size: 19px;
      color: var(--color-dark);
      transform: translateY(-50%);
      z-index: 0;
      transition: 0.5s;
    }

    span::after {
      content: '';
      position: absolute;
      left: 0%;
      top: 100%;
      width: 0%;
      height: 2px;
      background: linear-gradient(120deg, #3498db, #8e44ad);
      transition: 0.5s;
    }
  }

  .focus {
    span::before {
      top: -8px;
    }

    span::after {
      width: 100%;
    }
  }

  $search-min-height: 150px;

  .search-result {
    position: absolute;
    top: 108px;
    min-height: $search-min-height;
    max-height: 720px;
    overflow-x: auto;
    width: 750px;
    background-color: #fff;
    border-radius: 4px;
    border: 1px solid var(--color-secondary-light);

    .search-post {
      margin: 5px 10px;
      padding: 20px;
      border-radius: 6px;
      cursor: pointer;
      transition: all 0.3s;

      .title {
        color: var(--color-dark);
        font-size: 24px;
        margin-bottom: 10px;
        transition: all 0.3s;
      }

      .info {
        font-size: 16px;
        color: var(--color-dark-light);
        transition: all 0.3s;
        overflow: hidden;
        word-break: break-all;
        text-overflow: ellipsis;
        display: -webkit-box;
        -webkit-box-orient: vertical;
        -webkit-line-clamp: 2;
      }
    }

    .search-post:hover {
      background-color: var(--color-secondary);
      .title {
        color: var(--color-purple);
      }

      .info {
        color: var(--color-purple-light);
      }
    }

    .empty {
      height: $search-min-height;
      cursor: default;
      font-size: 16px;
    }
  }
}

.fade-enter-active {
  animation: fadeIn 0.5s;
}

.fade-leave-active {
  animation: fadeOut 0.3s;
}
</style>
