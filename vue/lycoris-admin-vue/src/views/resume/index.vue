<template>
  <div class="container">
    <div class="theme-option flex-center-center" v-if="!model.print">
      <el-select>
        <el-option v-for="item in model.theme" :key="item.value" :label="item.label" :value="item.value"></el-option>
      </el-select>
      <el-button type="primary" plain @click="exportResume">导出为PDF</el-button>
    </div>
    <div class="resume-theme">
      <div id="theme-body">
        <default-theme></default-theme>
      </div>
    </div>
  </div>
</template>

<script setup>
import { reactive, onMounted } from 'vue';
import defaultTheme from './components/theme-default.vue';

const model = reactive({
  theme: [
    {
      value: 0,
      label: '默认'
    }
  ],
  print: false
});

onMounted(() => {
  window.onbeforeprint = () => {
    model.print = true;
  };

  window.onafterprint = () => {
    model.print = false;
  };
});

const exportResume = () => {
  //
  model.print = true;
  setTimeout(() => {
    window.print();
  }, 0);
};
</script>

<style lang="scss" scoped>
.container {
  min-height: 100vh;
  padding: 20px;

  .theme-option {
    .el-button {
      margin-left: 10px;
    }
  }

  .resume-theme {
    #theme-body {
      margin: 0 atuo;
    }
  }
}

@media print {
  @page {
    size: A4;
    margin: 0;
    marks: none;
  }
}
</style>
