<template>
  <page-layout title="友情链接" icon="connection">
    <div class="friend-container card">
      <div>
        <p>友情链接申请的一些说明</p>
      </div>
      <el-divider content-position="right">
        <div class="flex-center-center">
          <el-icon :size="24" style="color: var(--color-danger)">
            <component :is="'star-filled'"></component>
          </el-icon>
          <span class="apply" @click="friendLinkApply">友链申请</span>
          <el-icon :size="24" style="color: var(--color-danger)">
            <component :is="'star-filled'"></component>
          </el-icon>
        </div>
      </el-divider>
      <transitionList class="friend-card-group" tag="div">
        <div class="card flex-start-center card-box-shadow" v-for="item in model.list" :key="item.link" @click="routeToLink(item)" :title="item.description">
          <div class="flex-start-center">
            <el-image :src="item.icon" lazy>
              <template #error>
                <img src="/images/404.png" title="图片加载失败" />
              </template>
            </el-image>
            <div>
              <span>{{ item.name }}</span>
              <div class="description">
                {{ item.description }}
              </div>
            </div>
          </div>
        </div>
      </transitionList>
    </div>

    <friend-apply ref="friendApplyRef"></friend-apply>
  </page-layout>
</template>

<script setup name="friends">
import { inject, onMounted, reactive, ref } from 'vue';
import pageLayout from '../layout/page-layout.vue';
import transitionList from '../../components/transitions/list-transition.vue';
import friendApply from './components/friend-apply-modal.vue';
import { getFriendLinkList } from '../../api/friend-link';
import { stores } from '../../stores';
import toast from '../../utils/toast';

const friendApplyRef = ref();
const loginModalRef = inject('$loginModal');

const model = reactive({
  list: []
});

const emit = defineEmits(['loading', 'browse']);

onMounted(async () => {
  await getList();

  emit('loading', false);
  emit('browse');
});

const page = {
  pageIndex: 1,
  pageSize: 20
};
const getList = async () => {
  try {
    let res = await getFriendLinkList({ ...page });
    if (res && res.resCode == 0) {
      model.list.push(...res.data.list);
    }
  } catch (error) {}
};

const routeToLink = item => {
  window.open(item.link);
};

const friendLinkApply = () => {
  if (!stores.user.state) {
    toast.info('请先登录');
    loginModalRef.value.show();
  } else {
    friendApplyRef.value.show();
  }
};
</script>

<style lang="scss" scoped>
.friend-container {
  width: 100%;
  padding: 20px;
  min-height: calc(100vh - 305px);

  span.apply {
    font-size: 18px;
    padding: 0 10px;
    color: var(--color-danger);
    cursor: pointer;
    transition: all 0.4s;

    &:hover {
      color: var(--color-purple);
    }
  }

  .friend-card-group {
    display: grid;
    grid-template-columns: repeat(4, minmax(350px, 1fr));
    grid-gap: 15px;

    @media (max-width: 1920px) {
      grid-template-columns: repeat(4, minmax(250px, 1fr));
    }

    @media (max-width: 1440px) {
      grid-template-columns: repeat(3, minmax(250px, 1fr));
    }

    .card {
      height: 140px;

      cursor: pointer;
      transition: all 0.4s;

      .flex-start-center {
        gap: 15px;

        $icon-size: 75px;
        :deep(.el-image) {
          flex-shrink: 0;
          height: $icon-size;
          width: $icon-size;
          border-radius: 50%;

          img {
            height: $icon-size;
            width: $icon-size;
            border-radius: 50%;
            object-fit: cover;
            border: 2px solid var(--color-danger);
          }
        }

        > div:last-child {
          > span {
            font-size: 16px;
            max-width: 218px;
            display: block;
            height: 25px;
            color: #000;
            overflow: hidden;
            text-overflow: ellipsis;
            white-space: nowrap;
            cursor: pointer;
            padding: 0 5px;
            transition: all 0.4s;

            @media (max-width: 1920px) {
              max-width: 172px;
            }

            @media (max-width: 1440px) {
              max-width: 210px;
            }
          }

          .description {
            max-height: 70px;
            margin: 10px 5px;
            // 最多显示三行 超出省略号显示
            display: -webkit-box;
            -webkit-box-orient: vertical;
            -webkit-line-clamp: 3;
            overflow: hidden;

            color: var(--color-dark-light);
            font-size: 14px;
            line-height: 18px;
          }
        }
      }

      &:hover {
        box-shadow: 0 0 2rem var(--color-dark-light);

        .flex-start-center {
          > div:last-child {
            span {
              color: var(--color-purple);
            }
          }
        }
      }
    }
  }

  .card-box-shadow {
    box-shadow: 0 0.625rem 1.875rem -0.9375rem rgba(0, 0, 0, 0.1);
    border: 1px solid var(--color-secondary);
  }
}
</style>
