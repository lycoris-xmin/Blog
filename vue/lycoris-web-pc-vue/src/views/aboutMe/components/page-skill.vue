<template>
  <div class="about-me-screen-view" :style="{ height: props.index > 3 ? '0px' : props.height }">
    <div class="view-body flex-center-center" style="flex-direction: column">
      <img src="@/assets/images/bg.jpg" alt="" />
      <div class="technology">
        <div class="gird-tech">
          <div class="header">
            <span>前端</span>
          </div>

          <ul class="defree">
            <li v-for="item in skill.web" :key="item">
              <span class="defree-name">{{ item.name }}</span>
              <processbar
                :value="item.proficiency"
                :height="'15px'"
                :position="'end'"
                :play="props.index == 3"
                :text-color="'#fff'"
                :duration="0.02"
                :show-percentage="true"
                :schedule-color="progressColors"
                :animation="true"
              ></processbar>
            </li>
          </ul>
        </div>

        <div class="gird-tech">
          <div class="header">
            <span>后端</span>
          </div>

          <ul class="defree">
            <li v-for="item in skill.server" :key="item">
              <span class="defree-name">{{ item.name }}</span>
              <processbar
                :value="item.proficiency"
                :height="'15px'"
                :position="'end'"
                :play="props.index == 3"
                :text-color="'#fff'"
                :duration="0.02"
                :show-percentage="true"
                :schedule-color="progressColors"
                :animation="true"
              ></processbar>
            </li>
          </ul>
        </div>

        <div class="gird-tech">
          <div class="header">
            <span>中间件及服务</span>
          </div>

          <ul class="defree">
            <li v-for="item in skill.middleware" :key="item">
              <span class="defree-name">{{ item.name }}</span>
              <processbar
                :value="item.proficiency"
                :height="'15px'"
                :position="'end'"
                :play="props.index == 3"
                :text-color="'#fff'"
                :duration="0.02"
                :show-percentage="true"
                :schedule-color="progressColors"
                :animation="true"
              ></processbar>
            </li>
          </ul>
        </div>

        <div class="gird-tech">
          <div class="header">
            <span>数据库</span>
          </div>

          <ul class="defree">
            <li v-for="item in skill.sql" :key="item">
              <span class="defree-name">{{ item.name }}</span>
              <processbar
                :value="item.proficiency"
                :height="'15px'"
                :position="'end'"
                :play="props.index == 3"
                :text-color="'#fff'"
                :duration="0.02"
                :show-percentage="true"
                :schedule-color="progressColors"
                :animation="true"
              ></processbar>
            </li>
          </ul>
        </div>
      </div>

      <div class="technology-des" v-if="skill.description && skill.description.length">
        <p v-for="item in skill.description" :key="item">{{ item }}</p>
      </div>
    </div>
  </div>
</template>

<script setup>
import { computed, ref } from 'vue';
import processbar from '@/components/progress-bar/index.vue';

const progressColors = ref([
  { color: '#fa8181', value: 20 },
  { color: '#f96197', value: 40 },
  { color: '#77d2dc', value: 60 },
  { color: '#48b0f7', value: 80 },
  { color: '#15c377', value: 100 }
]);

const props = defineProps({
  index: {
    type: Number,
    defalut: false
  },
  height: {
    type: String,
    defalut: '0px'
  },
  data: {
    type: Object,
    defalut: void 0
  }
});

const skill = computed(() => {
  if (!props.data) {
    return {
      web: [],
      server: [],
      middleware: [],
      sql: [],
      description: []
    };
  }

  return {
    web: props.data.web || [],
    server: props.data.server || [],
    middleware: props.data.middleware || [],
    sql: props.data.sql || [],
    description: props.data.description || []
  };
});
</script>

<style lang="scss" scoped>
.about-me-screen-view {
  --technology-width: 1600px;

  .technology {
    position: relative;
    display: grid;
    grid-template-columns: repeat(4, 1fr);
    grid-gap: 60px;
    width: var(--technology-width);

    @media (max-width: 1920px) {
      --technology-width: 1300px;
    }

    .gird-tech {
      color: #fff;
      .header {
        span {
          font-size: 24px;
          letter-spacing: 2px;
          padding: 2px 4px;
        }

        span:before {
          content: '#';
          padding-right: 10px;
          color: var(--color-danger);
        }
      }
    }

    .defree {
      li {
        list-style: none;
        padding: 10px 0;

        .defree-name {
          padding-bottom: 5px;
          padding-left: 5px;
          font-size: 17px;
          letter-spacing: 2px;
        }
      }
    }
  }

  .technology-des {
    position: relative;
    width: var(--technology-width);
    padding: 30px 0;
    color: #fff;
    letter-spacing: 2px;
    font-size: 16px;

    @media (max-width: 1920px) {
      --technology-width: 1300px;
    }
  }

  .technology-des::before {
    content: '[PS]';
    padding-right: 10px;
    color: var(--color-danger);
  }
}
</style>
