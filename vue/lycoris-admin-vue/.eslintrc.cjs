/* eslint-env node */
require('@rushstack/eslint-patch/modern-module-resolution');

module.exports = {
  root: true,
  extends: ['plugin:vue/vue3-essential', 'eslint:recommended', '@vue/eslint-config-prettier/skip-formatting'],
  parserOptions: {
    ecmaVersion: 'latest'
  },
  rules: {
    // 关闭文件多单词检测
    'vue/multi-word-component-names': 'off',
    'no-empty': 'off'

    // 'no-unused-vars': 'warn'
  }
};
