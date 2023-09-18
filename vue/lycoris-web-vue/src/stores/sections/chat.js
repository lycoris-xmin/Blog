import { defineStore } from 'pinia';
const key = 'l-z-chat-state';
const messageKey = 'l-z-message';

const getLocalStorage = key => {
  let value = localStorage.getItem(key);
  if (value) {
    value = JSON.parse(value);
  }

  return value || {};
};

const setLocalStorage = (key, value) => {
  localStorage.setItem(key, JSON.stringify(value));
};

const removeLocalStorage = key => {
  localStorage.removeItem(key);
};

export default defineStore('chat', {
  state: () => {
    const value = getLocalStorage(key);

    const message = getLocalStorage(messageKey);

    return {
      firstShowWangEditor: value?.firstShowWangEditor || false,
      submitType: value?.submitType || 0,
      unreadMessage: message
    };
  },
  getters: {
    totalUnreadMessage: state => {
      if (state.unreadMessage && Object.keys(state.unreadMessage).length) {
        return Object.values(state.unreadMessage).reduce((accumulator, currentValue) => accumulator + currentValue, 0);
      }

      return 0;
    }
  },
  actions: {
    setSubmitType(type) {
      this.submitType = type;
      let value = getLocalStorage(key);
      value.submitType = type;
      setLocalStorage(key, value);
    },
    setFirstShowWangEditor(firstShowWangEditor) {
      this.firstShowWangEditor = firstShowWangEditor;
      let value = getLocalStorage(key);
      value.firstShowWangEditor = firstShowWangEditor;
      setLocalStorage(key, value);
    },
    additionUnreadMessage(roomId, count) {
      this.unreadMessage[roomId] = this.unreadMessage[roomId] || 0;
      this.unreadMessage[roomId] = this.unreadMessage[roomId] + count;

      setLocalStorage(messageKey, this.unreadMessage);
    },
    clearUnreadMessage(roomId) {
      delete this.unreadMessage[roomId];

      const value = { ...this.unreadMessage };
      for (let item in value) {
        if (value[item] === 0) {
          delete value[item];
        }
      }
      if (value && Object.keys(value).length) {
        setLocalStorage(messageKey, this.unreadMessage);
      } else {
        removeLocalStorage(messageKey);
      }
    },
    resetChatStore() {
      //
      this.firstShowWangEditor = false;
    }
  }
});
