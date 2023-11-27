<template>
  <div class="flex-center-center">
    <div class="card router-link-info">
      <div class="info">
        将要跳转到 <span>{{ model.name }}</span> 网站
      </div>
      <ul class="warning">
        <li>本站所有收录的网站均为第三方网站，在第三方网站上任何行为，本站概不负责。</li>
        <li>请不要相信第三方网站任何有偿行为，谨防上当受骗。</li>
        <li>若网站链接失效，请联系博主进行处理。</li>
        <li>部分第三方网站可能存在某些不可抗力因素，导致不能收录，尽请谅解。</li>
      </ul>
      <div class="jump">
        <el-button type="danger" @click="jumpLink">
          跳转网站
          <span v-if="model.second > 0">({{ model.second }}s)</span>
        </el-button>
      </div>
    </div>
  </div>
</template>

<script setup name="nav-jump">
import { reactive, onMounted } from 'vue';
import { useRoute } from 'vue-router';

const model = reactive({
  name: '',
  lnik: '',
  second: 10
});

onMounted(() => {
  const route = useRoute();
  model.name = route.query.name;
  model.lnik = route.query.link;

  const timer = setInterval(() => {
    //
    if (model.second <= 0) {
      clearInterval(timer);
      jumpLink();
      return;
    }

    model.second--;
  }, 1000);
});

const jumpLink = () => {
  location.href = model.lnik;
};
</script>

<style lang="scss" scoped>
.flex-center-center {
  min-height: 100vh;
  width: 100%;

  .router-link-info {
    background-color: var(--color-secondary);
    padding: 20px;
    width: 600px;
    border-radius: 20px;
    transform: translateY(-20%);
    padding: 35px 20px;

    .info {
      text-align: center;
      padding: 20px 15px;
      font-size: 20px;

      > span {
        color: var(--color-danger);
      }
    }

    .warning {
      padding: 30px 20px;
      font-size: 16px;

      > li {
        margin-bottom: 10px;
        &:last-child {
          margin-bottom: 10px;
        }
      }
    }

    .jump {
      text-align: center;

      .el-button {
        padding-top: 20px;
        padding-bottom: 20px;
        width: 200px;
        font-size: 20px;
      }
    }
  }
}
</style>

<style lang="scss">
@import url(../assets/theme/theme.min.css);
</style>
