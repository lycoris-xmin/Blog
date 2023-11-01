class BroadcastChannelHelper {
  constructor(channelName) {
    this.channel = new BroadcastChannel(channelName);
  }

  sendMessage(message) {
    this.channel.postMessage({ type: 'message', data: message });
  }

  onMessage(callback) {
    this.channel.onmessage = function (event) {
      if (event.data.type === 'message') {
        callback(event.data.data);
      }
    };
  }

  close() {
    this.channel.close();
  }
}

export default BroadcastChannelHelper;
