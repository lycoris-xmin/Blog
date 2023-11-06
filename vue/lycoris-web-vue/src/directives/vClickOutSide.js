export default {
  mounted(el, binding) {
    el.handler = function (e) {
      // 点击范围在绑定的元素范围内，不执行指令操作
      if (el.contains(e.target)) {
        return;
      }

      if (typeof binding.value === 'function') {
        // 绑定给指令的如果是一个函数，那么将回调并指定this
        binding.value.apply(this, arguments);
      }
    };
    // 监听全局的点击事件
    document.addEventListener('click', el.handler);
  },
  // 解除事件绑定
  beforeUnmount(el) {
    document.removeEventListener('click', el.handler);
  }
};
