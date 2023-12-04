import { HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import { api } from '../config.json';

export default class {
  _subscribeHandler = [];

  constructor() {
    this.hubConnection = null;
  }

  async setupSignalR(url) {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(`${api.server}${api.routePrefix}${url}`)
      .withAutomaticReconnect({
        nextRetryDelayInMilliseconds: retryContext => {
          if (retryContext.previousRetryCount > 15) {
            return 90000;
          } else if (retryContext.previousRetryCount > 10) {
            return 60000;
          } else if (retryContext.previousRetryCount > 3) {
            return 30000;
          } else {
            return 3000;
          }
        }
      })
      .configureLogging(api.signalR.level === 'info' ? LogLevel.Information : LogLevel.Error)
      .build();

    await this.hubConnection.start();

    this.hubConnection.onreconnected(() => {
      if (this._subscribeHandler.length > 0) {
        for (let item of this._subscribeHandler) {
          this.subscribe(item.eventName, item.callback);
        }
      }

      for (let callback of this._connectdCallback) {
        callback();
      }
    });

    for (let callback of this._connectdCallback) {
      callback();
    }

    if (this._subscribeHandler.length > 0) {
      for (let item of this._subscribeHandler) {
        this.hubConnection.on(item.eventName, item.callback);
        item.subscribe = true;
      }
    }
  }

  subscribe(eventName, callback) {
    // 记录监听
    let data = {
      eventName,
      callback,
      subscribe: false
    };

    let index = this._subscribeHandler.findIndex(x => x.eventName == data.eventName);
    if (this._subscribeHandler.findIndex(x => x.eventName == data.eventName) == -1) {
      this._subscribeHandler.push();
    }

    if (!this.hubConnection) {
      if (index == -1) {
        this._subscribeHandler.push(data);
      } else {
        this._subscribeHandler[index].subscribe = false;
      }

      return;
    } else {
      this.hubConnection.on(eventName, callback);
      if (index == -1) {
        data.subscribe = true;
        this._subscribeHandler.push(data);
      } else if (!this._subscribeHandler[index].subscribe) {
        this._subscribeHandler[index].subscribe = true;
      }
    }
  }

  unsubscribe(eventName) {
    if (!this.hubConnection) {
      console.error('signalR disconnect');
      return;
    }

    let index = this._subscribeHandler.findIndex(x => x.eventName == eventName);
    if (index > -1) {
      this._subscribeHandler.splice(index, 1);
      this.hubConnection.off(eventName);
    }
  }

  async invoke(methodName, ...args) {
    if (!this.hubConnection) {
      throw new Error('signalR disconnect');
    }

    await this.hubConnection.invoke(methodName, ...args);
  }

  async stop() {
    if (!this.hubConnection) {
      console.error('signalR disconnect');
      return;
    }

    await this.hubConnection.stop();
  }

  _connectdCallback = [];
  connectdHadler(callback) {
    if (this.hubConnection != null) {
      callback();
    } else {
      this._connectdCallback.push(callback);
    }
  }
}
