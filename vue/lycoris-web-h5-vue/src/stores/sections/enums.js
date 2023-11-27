import { defineStore } from 'pinia';

export default defineStore('enums', {
  state: () => {
    return {
      category: []
    };
  },
  getters: {
    categoryIsValid: state => {
      return !state.category && state.category.length > 0;
    }
  },
  actions: {
    setCategory(list) {
      list = list || [];
      this.category = list.map(x => {
        return {
          name: x.name,
          value: x.value,
          icon: x.data || ''
        };
      });
    }
  }
});
