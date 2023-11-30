import { fileURLToPath, URL } from 'node:url';

import { defineConfig } from 'vite';
import vue from '@vitejs/plugin-vue';
import postcssPresetEnv from 'postcss-preset-env';
// import pluginPurgeCss from 'vite-plugin-purgecss';
import { Plugin as importToCDN } from 'vite-plugin-cdn-import';
import cssnano from 'cssnano';
import { terser } from 'rollup-plugin-terser';
import { visualizer } from 'rollup-plugin-visualizer';
import VueSetupExtend from 'vite-plugin-vue-setup-extend';
import viteCompression from 'vite-plugin-compression';

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [
    vue(),
    VueSetupExtend(),
    // pluginPurgeCss(),
    importToCDN({
      modules: [
        {
          name: 'vue',
          var: 'Vue',
          path: 'https://cdn.bootcdn.net/ajax/libs/vue/3.2.47/vue.global.prod.min.js'
        },
        {
          name: 'vue-demi',
          var: 'VueDemi',
          path: 'https://cdn.bootcdn.net/ajax/libs/vue-demi/0.14.0/index.iife.min.js'
        },
        {
          name: 'vue-router',
          var: 'VueRouter',
          path: `https://cdn.bootcdn.net/ajax/libs/vue-router/4.1.6/vue-router.global.prod.min.js`
        },
        {
          name: 'axios',
          var: 'axios',
          path: 'https://cdn.bootcdn.net/ajax/libs/axios/1.3.6/axios.min.js'
        },
        {
          name: 'element-plus',
          var: 'ElementPlus',
          path: 'https://cdn.bootcdn.net/ajax/libs/element-plus/2.3.3/index.full.min.js',
          css: 'https://cdn.bootcdn.net/ajax/libs/element-plus/2.3.3/index.min.css'
        },
        {
          name: '@element-plus/icons-vue',
          var: 'ElementPlusIconsVue',
          path: 'https://cdn.bootcdn.net/ajax/libs/element-plus-icons-vue/2.1.0/global.iife.min.js'
        },
        {
          name: 'clipboard',
          var: 'clipboard',
          path: 'https://cdn.bootcdn.net/ajax/libs/clipboard.js/2.0.11/clipboard.min.js'
        },
        {
          name: 'crypto-js',
          var: 'CryptoJS',
          path: 'https://cdn.bootcdn.net/ajax/libs/crypto-js/4.1.1/crypto-js.min.js'
        },
        {
          name: 'echarts',
          var: 'echarts',
          path: 'https://cdn.bootcdn.net/ajax/libs/echarts/5.4.3/echarts.min.js'
        },
        {
          name: '@microsoft/signalr',
          var: 'signalR',
          path: 'https://cdn.bootcdn.net/ajax/libs/microsoft-signalr/7.0.5/signalr.min.js'
        },
        {
          name: 'cherry-markdown',
          var: 'Cherry',
          path: 'https://cdn.jsdelivr.net/gh/lycoris-xmin/blog_fileserver/cherry-markdown/cherry-markdown.esm.js',
          css: 'https://cdn.jsdelivr.net/gh/lycoris-xmin/blog_fileserver/cherry-markdown/cherry-markdown.css'
        }
      ]
    }),
    visualizer({
      gzipSize: true,
      brotliSize: true,
      emitFile: false,
      filename: 'visualizer.html',
      open: false
    }),
    viteCompression()
  ],
  resolve: {
    alias: {
      '@': fileURLToPath(new URL('./src', import.meta.url))
    }
  },
  css: {
    postcss: {
      plugins: [
        postcssPresetEnv(),
        cssnano({
          preset: 'default'
        })
      ]
    }
  },
  build: {
    rollupOptions: {
      plugins: [terser()]
    }
  },
  server: {
    host: '127.0.0.1',
    port: 5174,
    open: true
  }
});
