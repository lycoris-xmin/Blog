export default {
  setup() {
    return {
      func: void 0,
      observer: void 0
    };
  },
  directives: {
    lazy: {
      mounted(el, binding) {
        this.observe = new IntersectionObserver(enr => {
          if (enr && enr.length) {
            let entity = enr[0];
            if (entity.intersectionRatio >= binding.arg && binding.value && typeof binding.value == 'function') {
              binding.value();
              this.observer.unobserve(el);
              this.observer = void 0;
            }
          }
        });

        this.observer.observe(el);
      },
      unmounted(el) {
        if (this.observer) {
          this.observer.unobserve(el);
        }
      }
    }
  }
};
