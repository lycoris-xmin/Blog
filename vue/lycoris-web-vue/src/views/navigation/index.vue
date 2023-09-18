<template>
  <div class="container">
    <transition-list class="nav-group">
      <li class="card" v-for="item in model.list" :key="item.group">
        <div class="group-title">
          <span>{{ item.group }}</span>
        </div>
        <div class="nav-domain">
          <div class="domain-item" v-for="groupItem in item.groupList" :key="groupItem.domain">
            <router-link :to="{ name: 'nav-jump', query: { name: groupItem.name, link: groupItem.domain } }" target="_blank">{{ groupItem.name }}</router-link>
          </div>
        </div>
      </li>
    </transition-list>
  </div>
</template>

<script setup name="navigation">
import { onMounted, reactive } from 'vue';
import { getSiteNavigationList } from '../../api/navigation';
import transitionList from '../../components/transitions/list-transition.vue';

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
        grid-template-columns: repeat(12, minmax(100px, 1fr));
        grid-gap: 5px;

        .domain-item {
          text-align: center;
          > a {
            transition: all 0.4s;
            padding: 10px;
            border-radius: 4px;
            color: #000;
            letter-spacing: 2.5px;

            &:hover {
              background-color: var(--color-secondary);
              color: var(--color-danger);
            }
          }
        }
      }
    }
  }
}
</style>
