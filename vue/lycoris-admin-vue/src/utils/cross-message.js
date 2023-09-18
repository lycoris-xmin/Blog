class CrossWindowCommunication {
  target = '';
  channel = '';
  _callback = void 0;

  constructor(channel, target = '') {
    this.channel = channel;
    this.target = target;
  }

  sendMessage(message) {
    const messageObj = {
      channel: this.channel
    };

    if (typeof message == 'string' || typeof message == 'number') {
      messageObj.content = message;
    } else {
      messageObj.content = {
        ...message
      };
    }

    if (this.target) {
      window.postMessage(messageObj, this.target);
    } else {
      window.postMessage(messageObj);
    }
  }

  subscribe(callback) {
    this._callback = callback;
    const listener = event => {
      const messageObj = event.data;

      if (this.target) {
        if (event.origin === this.target) {
          if (messageObj.channel === this.channel) {
            this._callback(messageObj.content);
          }
        }
      } else {
        if (messageObj.channel === this.channel) {
          this._callback(messageObj.content);
        }
      }
    };
    window.addEventListener('message', listener);
  }

  unsubscribe() {
    window.removeEventListener('message', this._callback);
  }
}

export default CrossWindowCommunication;
