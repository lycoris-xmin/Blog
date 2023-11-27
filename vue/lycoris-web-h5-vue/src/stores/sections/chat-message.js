import { defineStore } from 'pinia';

export default defineStore('chat-message', {
  state: () => {
    return {
      messages: {}
    };
  },
  actions: {
    setRoomMessageList(roomId, { count, list }, { pageIndex = 1 }) {
      this._calcMessageTime(roomId, list);

      this.messages[roomId].count = count;
      this.messages[roomId].list.unshift(...list);

      if (this.messages[roomId].pageIndex < pageIndex) {
        this.messages[roomId].pageIndex = pageIndex;
      }
    },
    addRoomMessage(roomId, data) {
      this._initRoomMessages(roomId);

      this._calcMessageTime(roomId, [{ ...data }]);

      this.messages[roomId].count++;
      this.messages[roomId].list.push(data);
    },
    updateRoomMessage(roomId, data) {
      const index = this.messages[roomId].list.findIndex(x => x.id == data.messageId);

      if (this.messages[roomId].times[data.messageId]) {
        this.messages[roomId].times[data.id] = this.messages[roomId].times[data.messageId];
        delete this.messages[roomId].times[data.messageId];
      }

      delete data.messageId;
      this.messages[roomId].list[index] = data;
    },
    updateErrorMessage(roomId, messageId, error) {
      const index = this.messages[roomId].list.findIndex(x => x.id == messageId);
      if (index > -1) {
        this.messages[roomId].list[index].ackError = error;
      }
    },
    resetChatMessageStore() {
      this.messages = {};
    },
    _initRoomMessages(roomId) {
      this.messages[roomId] = this.messages[roomId] || {
        count: 0,
        list: [],
        pageIndex: 0,
        times: {}
      };
    },
    _paginateArray(roomId, pageNumber, pageSize) {
      const startIndex = (pageNumber - 1) * pageSize;
      const endIndex = startIndex + pageSize;
      return this.messages[roomId].list.slice(startIndex, endIndex);
    },
    _calcMessageTime(roomId, list, showTimeSpan = 300000) {
      if (!list || list.length === 0) {
        return;
      }

      const timeList = list.map(x => ({
        id: x.id,
        createTime: new Date(x.createTime)
      }));

      const messages = this.messages[roomId];

      let lastTime = messages.list.length ? new Date(messages.list[messages.list.length - 1].createTime) : new Date();
      const times = Object.values(messages.times);

      if (times.length) {
        if (list.length > 1) {
          const timeLength = times.length;
          for (const item of timeList) {
            if (item.createTime.getTime() - lastTime.getTime() >= showTimeSpan) {
              messages.times[item.id] = item.createTime.format('yyyy/M/d HH:mm:ss');
              lastTime = item.createTime;
            }
          }

          if (times.length == timeLength) {
            const item = timeList[0];
            messages.times[item.id] = item.createTime.format('yyyy/M/d HH:mm:ss');
          }
        } else {
          const itemTime = new Date(list[0].createTime);
          if (itemTime.getTime() - lastTime.getTime() >= showTimeSpan) {
            messages.times[list[0].id] = itemTime.format('yyyy/M/d HH:mm:ss');
          }
        }
      } else {
        if (list.length > 1) {
          for (let i = 0; i < timeList.length; i++) {
            if (i === 0) {
              if (new Date().getTime() - timeList[i].createTime.getTime() >= showTimeSpan) {
                messages.times[timeList[i].id] = timeList[i].createTime.format('yyyy/M/d HH:mm:ss');
              }
              continue;
            }

            const beforeItem = timeList[i - 1];
            const afterItem = timeList[i];

            if (afterItem.createTime.getTime() - beforeItem.createTime.getTime() >= showTimeSpan) {
              messages.times[afterItem.id] = afterItem.createTime.format('yyyy/M/d HH:mm:ss');
            }
          }
        }

        if (times.length == 0) {
          const itemTime = new Date(list[0].createTime);
          if (itemTime.getTime() - lastTime.getTime() >= showTimeSpan) {
            messages.times[list[0].id] = itemTime.format('yyyy/M/d HH:mm:ss');
          }
        }
      }
    }
  }
});
