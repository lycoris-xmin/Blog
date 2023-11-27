class CrossWindowCommunication {
  _callback = void 0;

  constructor() {}

  sendMessage(channel, message) {
    const messageObj = {
      channel: channel
    };

    if (typeof message == 'string' || typeof message == 'number') {
      messageObj.content = message;
    } else {
      messageObj.content = {
        ...message
      };
    }

    window.postMessage(messageObj);
  }

  subscribe(channel, callback) {
    this._callback = callback;
    const listener = event => {
      const messageObj = event.data;

      if (this.channel == channel) {
        this._callback(messageObj.content);
      }
    };
    window.addEventListener('message', listener);
  }

  unsubscribe() {
    window.removeEventListener('message', this._callback);
  }
}

export default CrossWindowCommunication;
