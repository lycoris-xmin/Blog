<template>
  <div class="server-result" :class="{ forbidden: model.statusCode == 403, notfound: model.statusCode == 404 }">
    <h1>{{ model.statusCode }}</h1>
    <div class="content" ref="writerRef">
      <p>
        <span>ERROR CODE</span>:
        <i>"{{ model.code }}"</i>
      </p>
      <p><span>ERROR DESCRIPTION</span>: "{{ model.description }}"</p>
      <p><span>ERROR POSSIBLY CAUSED BY</span>: {{ model.causedBy }}...</p>
      <p><span>SOME PAGES ON THIS SERVER THAT YOU DO HAVE PERMISSION TO ACCESS</span>: [<RouterLink :to="'/'">返回首页</RouterLink>]</p>
      <p><span>HAVE A NICE DAY :-)</span></p>
    </div>
  </div>
</template>

<script setup name="server-error">
import { reactive, onMounted, ref } from 'vue';
import { useRoute } from 'vue-router';
import { typewriter } from '@/utils/tool';

const route = useRoute();
const writerRef = ref();
const model = reactive({
  statusCode: 500,
  code: 'HTTP 500 InternalServerError',
  description: '服务内部发生错误，暂时无法使用，请稍候再试！',
  causedBy: '服务更新或可能代码存在Bug还未及时修复，请暂时不要使用该功能'
});

onMounted(() => {
  if (route.params) {
    if (route.params.statusCode) {
      model.statusCode = route.params.statusCode;
      if (model.statusCode == 404) {
        model.code = 'HTTP 404 NotFound';
        model.description = '资源不存在！';
        model.causedBy = '外部资源链接失效，或内部资源被错误删除';
      } else if (model.statusCode == 403) {
        model.code = 'HTTP 403 Forbidden';
        model.description = '权限不足，拒绝访问。 您无权访问此服务器上的此页面！';
        model.causedBy = '服务器未找到您的登录状态或您当前的权限不足，建议您重新登录或刷新页面尝试';
      }
    }
  }

  typewriter(writerRef.value);
});
</script>

<style lang="scss" scoped>
@import url('https://fonts.googleapis.com/css?family=Share+Tech+Mono|Montserrat:700');

.server-result {
  height: 100vh;
  width: 100%;

  * {
    margin: 0;
    padding: 0;
    border: 0;
    font-size: 100%;
    font: inherit;
    vertical-align: baseline;
    box-sizing: border-box;
    color: inherit;
  }

  h1 {
    font-size: 45vw;
    text-align: center;
    position: fixed;
    width: 100vw;
    z-index: 1;
    color: #ffffff26;
    text-shadow: 0 0 50px rgba(0, 0, 0, 0.07);
    top: 50%;
    -webkit-transform: translateY(-50%);
    transform: translateY(-50%);
    font-family: 'Montserrat', monospace;
  }

  .content {
    background: rgba(0, 0, 0, 0);
    width: 70vw;
    position: relative;
    top: 50%;
    -webkit-transform: translateY(-50%);
    transform: translateY(-50%);
    margin: 0 auto;
    padding: 30px 30px 10px;
    box-shadow: 0 0 150px -20px rgba(0, 0, 0, 0.5);
    z-index: 3;
    cursor: default;

    p {
      font-family: 'Share Tech Mono', monospace;
      color: #f5f5f5;
      margin: 0 0 20px;
      font-size: 17px;
      line-height: 1.2;
      overflow: hidden;
      word-break: break-all;
    }

    span {
      color: #f0c674;
    }

    i {
      color: #8abeb7;
    }

    a {
      color: var(--color-primary);
      text-decoration: none;
      cursor: pointer;

      &:hover {
        color: var(--color-purple);
      }
    }

    b {
      color: #81a2be;
    }
  }
}
</style>

<style lang="scss">
body {
  background-image: linear-gradient(120deg, #882000 0%, #000000 100%);
  height: 100vh;

  &:has(.forbidden) {
    background-image: linear-gradient(120deg, #4f0088 0%, #000000 100%);
    height: 100vh;
  }

  &:has(.notfound) {
    background-image: linear-gradient(120deg, #006888 0%, #000000 100%);
    height: 100vh;
  }
}
</style>
