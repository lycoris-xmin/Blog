<template>
  <div class="container">
    <transition-list class="nav-group">
      <li class="card" v-for="item in model.list" :key="item.group">
        <div class="group-title">
          <span>{{ item.group }}</span>
        </div>
        <div class="nav-domain">
          <div class="domain-item" v-for="groupItem in item.groupList" :key="groupItem.domain">
            <router-link class="flex-center-center" :to="{ name: 'nav-jump', query: { name: groupItem.name, link: groupItem.domain } }" target="_blank">
              <div class="ico">
                <el-image :src="`${groupItem.domain}/favicon.ico`" :alt="groupItem.name" lazy>
                  <template #error>
                    <el-icon>
                      <component :is="'eleme'"></component>
                    </el-icon>
                  </template>
                </el-image>
              </div>
              {{ groupItem.name }}
            </router-link>
          </div>
        </div>
      </li>
    </transition-list>
  </div>
</template>

<script setup name="navigation">
import { onMounted, reactive } from 'vue';
import { getSiteNavigationList } from '@/api/navigation';
import transitionList from '@/components/transitions/list-transition.vue';

const model = reactive({
  list: []
});
const emit = defineEmits(['loading', 'browse']);

onMounted(async () => {
  try {
    let res = await getSiteNavigationList();
    if (res && res.resCode == 0) {
      model.list = res.data.list;
    }
  } finally {
    emit('loading', false);
    emit('browse');
  }
});
</script>

<style lang="scss" scoped>
.container {
  height: calc(100vh - 140px);
  width: 100%;
  padding: 45px 0;

  .nav-group {
    padding: 0 20px;

    li {
      list-style: none;
      margin-bottom: 20px;
      &:last-child {
        margin-bottom: 25px;
      }

      .group-title {
        font-size: 20px;
        cursor: default;

        span {
          background: var(--main-linear-gradient) no-repeat left bottom;
          background-size: 66% 3px;
          letter-spacing: 3px;
          padding-bottom: 10px;
        }
      }

      .nav-domain {
        padding: 25px 0px 20px 0px;
        display: grid;
        grid-template-columns: repeat(10, minmax(50px, 1fr));
        grid-gap: 10px;

        .domain-item {
          text-align: center;

          > a {
            transition: all 0.4s;
            padding: 8px 5px;
            border-radius: 4px;
            color: #000;
            letter-spacing: 2.5px;
            background-color: var(--color-secondary-light);

            &:hover {
              background-color: var(--color-secondary);
              color: var(--color-primary);
            }

            .ico {
              padding-right: 5px;
              display: flex;
              align-items: center;

              :deep(.el-image) {
                width: 15px;
                height: 15px;

                .el-image__wrapper {
                  display: flex;
                  align-items: center;
                }
              }
            }
          }
        }
      }
    }
  }
}
</style>
