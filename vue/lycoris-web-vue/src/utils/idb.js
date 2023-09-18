import { openDB } from 'idb';

export default class {
  constructor(databaseName, objectStoreName) {
    this.databaseName = databaseName;
    this.objectStoreName = objectStoreName;
  }

  async openDatabase() {
    if (this.db) {
      return this.db;
    }

    this.db = await openDB(this.databaseName, 1, {
      upgrade(db) {
        if (!db.objectStoreNames.contains(this.objectStoreName)) {
          db.createObjectStore(this.objectStoreName, { keyPath: 'id' });
        }
      }
    });

    return this.db;
  }

  async addData(data) {
    const db = await this.openDatabase();
    const tx = db.transaction(this.objectStoreName, 'readwrite');
    const store = tx.objectStore(this.objectStoreName);
    await store.add(data);
    await tx.complete;
  }

  async getData(id) {
    const db = await this.openDatabase();
    const tx = db.transaction(this.objectStoreName, 'readonly');
    const store = tx.objectStore(this.objectStoreName);
    const data = await store.get(id);
    await tx.complete;
    return data;
  }

  async updateData(data) {
    const db = await this.openDatabase();
    const tx = db.transaction(this.objectStoreName, 'readwrite');
    const store = tx.objectStore(this.objectStoreName);
    await store.put(data);
    await tx.complete;
  }

  async deleteData(id) {
    const db = await this.openDatabase();
    const tx = db.transaction(this.objectStoreName, 'readwrite');
    const store = tx.objectStore(this.objectStoreName);
    await store.delete(id);
    await tx.complete;
  }

  async queryDataByCondition(condition) {
    const db = await this.openDatabase();
    const tx = db.transaction(this.objectStoreName, 'readonly');
    const store = tx.objectStore(this.objectStoreName);
    const request = store.openCursor();

    let results = [];

    return new Promise((resolve, reject) => {
      request.onsuccess = event => {
        const cursor = event.target.result;

        if (cursor) {
          const data = cursor.value;

          // 根据条件过滤数据
          if (this.checkCondition(data, condition)) {
            results.push(data);
          }

          cursor.continue();
        } else {
          resolve(results);
        }
      };

      request.onerror = () => {
        reject(request.error);
      };
    });
  }

  async queryDataByPage(pageNumber, pageSize) {
    const db = await this.openDatabase();
    const tx = db.transaction(this.objectStoreName, 'readonly');
    const store = tx.objectStore(this.objectStoreName);
    const request = store.openCursor();

    let count = 0;
    let results = [];

    return new Promise((resolve, reject) => {
      request.onsuccess = event => {
        const cursor = event.target.result;

        if (cursor) {
          count++;

          if (count >= (pageNumber - 1) * pageSize && count <= pageNumber * pageSize) {
            results.push(cursor.value);
          }

          if (count < pageNumber * pageSize + pageSize) {
            cursor.continue();
          } else {
            resolve(results);
          }
        } else {
          resolve(results);
        }
      };

      request.onerror = () => {
        reject(request.error);
      };
    });
  }

  checkCondition(data, condition) {
    // 根据条件检查数据是否满足要求
    // 这里假设条件是一个函数，接受数据作为参数，返回一个布尔值
    return condition(data);
  }
}
