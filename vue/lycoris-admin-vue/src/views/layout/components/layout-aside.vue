<template>
  <el-aside class="layout-sidebar">
    <div class="layout-logo flex-center-center">
      <a :href="props.webPath" target="_blank"><img src="/logo/logo-sidebar-lycoris.png" /></a>
    </div>
    <div class="layout-menu">
      <el-scrollbar>
        <el-menu :default-active="$route.path" :unique-opened="true">
          <div v-for="(item, index) in props.menus" :key="item.path">
            <router-link :to="item.path" class="menu-router" v-if="item.path">
              <el-menu-item :index="item.path" :menu-path="item.path">
                <el-icon>
                  <component :is="item.icon"></component>
                </el-icon>
                <span class="menu-name" :title="item.name.length > 13 ? item.name : ''">{{ item.name }}</span>
              </el-menu-item>
            </router-link>

            <el-sub-menu v-else :index="`${index}`">
              <template #title>
                <el-icon>
                  <component :is="item.icon"></component>
                </el-icon>
                <span class="menu-name" :title="item.name.length > 13 ? item.name : ''">{{ item.name }}</span>
              </template>

              <div v-for="child in item.routes" :key="child.path">
                <router-link :to="child.path" class="menu-router flex-start-center">
                  <el-menu-item :index="child.path" :menu-path="child.path">
                    <span class="menu-name" :title="child.name.length > 13 ? child.name : ''">{{ child.name }}</span>
                  </el-menu-item>
                </router-link>
              </div>
            </el-sub-menu>
          </div>
        </el-menu>
      </el-scrollbar>
    </div>
  </el-aside>
</template>

<script setup>
const props = defineProps({
  menus: {
    type: Array,
    required: true
  },
  webPath: {
    type: String,
    default: ''
  }
});
</script>

<style lang="scss" scoped>
$layout-header-h: 64px;

.layout-sidebar {
  height: 100vh;
  width: 250px;

  .layout-logo {
    height: $layout-header-h;
    cursor: pointer;
  }

  .layout-menu {
    min-height: calc(100vh - $layout-header-h - 10px);
    border-right: solid 1px var(--el-menu-border-color);
    margin-bottom: 10px;

    .el-menu {
      border-right: 0;
      .menu-router {
        color: var(--color-text);
        text-decoration: none;
        width: 100%;
        height: 100%;

        .el-menu-item {
          width: 100%;

          .menu-name {
            overflow: hidden;
            white-space: nowrap;
            text-overflow: ellipsis;
            font-size: 15px;
            letter-spacing: 2.5px;
            line-height: 15px;
          }
        }
      }

      .el-menu-item.is-active {
        border-right: solid 2px var(--color-primary);
        background-color: var(--el-menu-hover-bg-color);

        .menu-router {
          color: var(--el-menu-active-color);
        }
      }

      .el-sub-menu {
        .el-menu-item {
          .menu-name {
            padding-left: 10px;
          }
        }
      }
    }
  }
}
</style>
