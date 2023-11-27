<template>
  <div class="post-category card-border-radius" ref="postRef" :style="{ height: model.collapse ? `${model.categoryScrollHeight}px` : '72px' }">
    <div class="category-grid">
      <ul class="flex-start-center">
        <li v-for="(item, index) in model.category" :key="item.name">
          <div class="category flex-center-center" :class="{ active: item.active }" @click="categoryChange(index)">
            <span class="name">{{ item.name }}</span>
            <span class="category-post-count" v-if="item.count">{{ item.count }}</span>
          </div>
        </li>
      </ul>
    </div>
    <div class="collapse" :class="{ show: model.collapse }" v-if="model.categoryScrollHeight > 72">
      <el-icon :size="24" @click="collapse">
        <component :is="'d-arrow-left'"></component>
      </el-icon>
    </div>
  </div>
</template>

<script setup>
import { onMounted, reactive, ref, nextTick } from 'vue';
import { getCategoryHeaders } from '@/api/category';

const props = defineProps({
  loadIndex: {
    type: Number,
    default: void 0
  }
});

const postRef = ref();
const model = reactive({
  categoryScrollHeight: 0,
  collapse: false,
  category: [],
  selectedCategory: ''
});

const emit = defineEmits(['change', 'loadComplete']);

const categoryChange = index => {
  for (let i = 0; i < model.category.length; i++) {
    if (i == index) {
      model.selectedCategory = model.category[i].id;
      if (model.category[i].active) {
        return;
      }

      model.category[i].active = true;
    } else {
      model.category[i].active = false;
    }
  }

  emit('change', model.selectedCategory);
};

const getCategorys = async () => {
  try {
    let res = await getCategoryHeaders();
    if (res && res.resCode == 0) {
      if (res.data.list && res.data.list.length) {
        model.category = res.data.list.map(x => {
          return {
            id: x.id,
            name: x.name,
            active: false,
            count: x.count
          };
        });
      }
    }
  } catch (error) {}

  model.category.unshift({
    name: '近期文章',
    active: true
  });
};

onMounted(async () => {
  try {
    await getCategorys();
    nextTick(() => {
      model.categoryScrollHeight = postRef.value.scrollHeight;
    });
  } finally {
    emit('loadComplete', props.loadIndex);
  }
});

const collapse = () => {
  model.collapse = !model.collapse;
};
</script>

<style lang="scss" scoped>
.post-category {
  overflow: hidden;
  background-color: var(--main-background-light-color);
  transition: height 0.4s;
  z-index: 2;
  box-shadow: 0 0 8px 8px var(--color-secondary-light);

  .category-grid {
    padding: 20px;
    width: 95%;

    ul {
      flex-flow: row wrap;
      gap: 20px;

      li {
        list-style: none;

        .category {
          padding: 4px 5px;
          background: var(--color-secondary);
          border-radius: 8px;
          cursor: pointer;
          transition: all 0.4s;

          .name {
            padding: 0px 10px;
          }

          .category-post-count {
            padding: 0 8px 0 5px;
            color: var(--color-danger);
            transition: all 0.4s;
          }
        }

        .category:hover {
          opacity: 0.8;

          .category-post-count {
            opacity: 0.8;
          }
        }

        .category.active {
          background: var(--main-linear-gradient);
          color: #fff;

          .category-post-count {
            color: var(--color-yellow);
          }
        }
      }
    }
  }

  .collapse {
    position: absolute;
    top: 24px;
    right: 30px;

    .el-icon {
      cursor: pointer;
      transform: scale(1) rotate(-90deg);
      transition: all 0.3s;
    }

    .el-icon:hover {
      transform: scale(1.2) rotate(-90deg);
    }
  }

  .collapse.show {
    .el-icon {
      transform: scale(1) rotate(90deg);
    }

    .el-icon:hover {
      transform: scale(1.2) rotate(90deg);
    }
  }
}

@media (max-width: 1920px) {
}
</style>
