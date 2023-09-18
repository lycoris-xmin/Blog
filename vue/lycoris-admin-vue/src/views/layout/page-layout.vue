<template>
  <div class="container" id="affix-container">
    <el-affix target="#affix-container" :offset="94" v-if="props.affix">
      <div class="card-header" v-if="showHeader">
        <div class="title">
          <span>{{ props.title || $route.meta.title }}</span>
        </div>
        <div class="tool flex-center-center">
          <slot name="tool"></slot>
        </div>
      </div>
    </el-affix>
    <div v-else>
      <div class="card-header" v-if="showHeader">
        <div class="title">
          <span>{{ props.title || $route.meta.title }}</span>
        </div>
      </div>
    </div>
    <div class="card-content">
      <div v-show="!props.loading">
        <slot></slot>
      </div>
      <div class="loading-preloader" v-if="props.loading">
        <div class="loading-preloader-inner"></div>
      </div>
    </div>
  </div>
</template>

<script setup>
const props = defineProps({
  showHeader: {
    type: Boolean,
    default: true
  },
  title: {
    type: String,
    default: ''
  },
  loading: {
    type: Boolean,
    default: false
  },
  affix: {
    type: Boolean,
    default: false
  }
});
</script>

<style lang="scss" scoped>
.container {
  padding: 0 20px 10px 20px;
  height: calc(100vh - 125px);
  position: relative;
  overflow-y: auto;

  .card-header {
    padding-top: 10px;
    background-color: #fff;
    width: 100%;
    display: flex;
    justify-content: space-between;
    align-items: center;
    padding-bottom: 5px;
    border-bottom: 1px solid var(--color-secondary);
    margin-bottom: 5px;

    .title {
      span {
        font-size: 20px;
        font-weight: 500;
        cursor: default;
      }
    }

    .tool {
      gap: 10px;
      padding: 0 10px;
    }
  }

  .card-content {
    padding: 10px 0;
    position: relative;
    min-height: calc(100vh - 190px);

    .loading-preloader {
      position: absolute;
      top: 0;
      left: 0;
      width: 100%;
      height: 100%;
      background-color: #fff;
      z-index: 10000;
    }

    .loading-preloader .loading-preloader-inner {
      display: block;
      position: relative;
      left: 50%;
      top: 50%;
      width: 80px;
      height: 80px;
      margin: -40px 0 0 -40px;
      border-radius: 50%;
      border: 3px solid transparent;
      border-top-color: #3498db;
      animation: spin 2s linear infinite;
    }

    .loading-preloader .loading-preloader-inner:before {
      content: '';
      position: absolute;
      top: 5px;
      left: 5px;
      right: 5px;
      bottom: 5px;
      border-radius: 50%;
      border: 3px solid transparent;
      border-top-color: #e74c3c;
      animation: spin 3s linear infinite;
    }

    .loading-preloader .loading-preloader-inner:after {
      content: '';
      position: absolute;
      top: 15px;
      left: 15px;
      right: 15px;
      bottom: 15px;
      border-radius: 50%;
      border: 3px solid transparent;
      border-top-color: #f9c922;
      animation: spin 1.5s linear infinite;
    }

    @keyframes spin {
      0% {
        transform: rotate(0deg);
      }
      to {
        transform: rotate(1turn);
      }
    }
  }
}
</style>

<style lang="scss">
.search-panel {
  width: 100%;
  height: 60px;
  margin-bottom: 20px;

  form {
    display: flex;
    justify-content: flex-start;
    align-items: center;
    gap: 20px;

    .form-group {
      width: 230px;

      .el-form-item__content {
        width: 230px;
      }

      .el-input {
        width: 100%;
      }
    }

    .form-group.form-group-lg {
      width: 400px;
    }
  }
}
</style>
