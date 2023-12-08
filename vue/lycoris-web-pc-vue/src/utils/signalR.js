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

      if (this._reconnectedCallback.length > 0) {
        for (let item of this._reconnectedCallback) {
          item();
        }
      }
    });
  }

  subscribe(eventName, callback) {
    if (!this.hubConnection) {
      console.error('signalR disconnect');
      return;
    }

    this.hubConnection.on(eventName, callback);

    // 记录监听
    if (this._subscribeHandler.findIndex(x => x.eventName == eventName) == -1) {
      this._subscribeHandler.push({
        eventName,
        callback
      });
    }
  }

  unsubscribe(eventName) {
    if (!this.hubConnection) {
      console.error('signalR disconnect');
      return;
    }

    this.hubConnection.off(eventName);

    let index = this._subscribeHandler.findIndex(x => x.eventName == eventName);
    if (index > -1) {
      this._subscribeHandler.splice(index, 1);
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

  _reconnectedCallback = [];

  reconnectedHanlder(callback) {
    this._reconnectedCallback.push(callback);
  }
}
