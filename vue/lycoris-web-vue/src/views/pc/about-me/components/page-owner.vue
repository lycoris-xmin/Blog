<template>
  <div class="about-me-screen-view" :style="{ height: props.index > 2 ? '0px' : props.height }">
    <div class="view-body flex-center-center">
      <img src="@/assets/images/bg.jpg" alt="" />
      <div class="view-body flex-center-center">
        <div class="owner">
          <div class="flex-start-center info" style="flex-direction: column">
            <img :src="stores.owner.avatar" alt="" />
          </div>
          <ul>
            <li>
              <div>学历</div>
              <span v-if="data.educational">{{ data.institutions != undefined ? `${data.educational}${data.institutions > 0 ? `(${data.institutions == 1 ? '211' : '985'}院校)` : ''}` : data.educational }}</span>
              <span v-else>山河大学</span>
            </li>
            <li>
              <div>性别</div>
              <span>{{ data.sex || '保密' }}</span>
            </li>
            <li>
              <div>年龄</div>
              <span v-if="data.age">{{ data.age }}岁</span>
              <span v-else>长命百岁</span>
            </li>
            <li v-if="data.code && data.code.length">
              <div>语言掌握</div>
              <div class="flex-start-center flex-row-warp">
                <span class="badge badge-info" v-for="item in data.code" :key="item">{{ item }}</span>
              </div>
            </li>
            <li v-if="data.hobby && data.hobby.length">
              <div>标签</div>
              <div class="flex-start-center flex-row-warp">
                <span class="badge badge-info" v-for="item in data.hobby" :key="item">{{ item }}</span>
              </div>
            </li>
            <li v-if="data.introduction">
              <div>关于我</div>
              <p v-for="item in data.introduction" :key="item">{{ item }}</p>
              <p v-if="data.institutions == null || !data.institutions.length">博主太懒还没有编写</p>
            </li>
          </ul>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { computed } from 'vue';
import { stores } from '@/stores';

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

const data = computed(() => {
  if (!props.data) {
    return {
      sex: '保密',
      age: 0,
      code: [],
      hobby: [],
      introduction: ''
    };
  }

  return {
    educational: props.data.educational || '',
    institutions: props.data.institutions == undefined ? 0 : props.data.institutions,
    sex: props.data.sex || '保密',
    age: props.data.age || 0,
    code: props.data.code?.filter(Boolean) || [],
    hobby: props.data.hobby?.filter(Boolean) || [],
    introduction: props.data.introduction || []
  };
});
</script>

<style lang="scss" scoped>
.owner {
  width: 1000px;
  display: grid;
  grid-template-columns: 1fr 2fr;

  .info {
    padding-top: 10px;
    > img {
      height: 200px;
      width: 200px;
      border-radius: 20%;
    }
  }

  ul {
    color: #fff;
    font-size: 20px;

    li {
      padding: 8px 8px;

      span,
      p {
        font-size: 17px;
        letter-spacing: 2px;
      }

      .flex-row-warp {
        flex-flow: row wrap;
        gap: 10px;
        padding-top: 10px;
      }
    }
  }
}
</style>
