<template>
  <el-dialog v-model="model.dialogVisible" title="请求日志" width="65%">
    <div class="descriptions">
      <div class="request">
        <div class="request-flex-item border-bottom flex-start-center">
          <div class="flex-item-title w-100">请求地址</div>
          <div class="flex-item-value">
            <p class="request-route">
              <span class="http-method" :class="{ get: model.httpMethod == 'GET', post: model.httpMethod == 'POST' }">{{ model.httpMethod }}</span>
              {{ model.route }}
            </p>
          </div>
        </div>
        <div class="request-flex-item border-bottom flex-start-center">
          <div class="flex-start-center" style="width: 50%">
            <div class="flex-item-title w-100">客户端IP</div>
            <div class="flex-item-value">{{ model.ip }}</div>
          </div>
          <div class="flex-start-center border-left" style="width: 50%">
            <div class="flex-item-title w-100">IP归属地</div>
            <div class="flex-item-value">{{ model.ipAddress }}</div>
          </div>
        </div>
        <div class="request-flex-item border-bottom flex-start-center" v-if="model.headers.length">
          <div class="flex-item-title w-100">请求头</div>
          <div class="flex-item-value" style="width: 100%; overflow: auto">
            <ul class="request-headers">
              <li v-for="item in model.headers" :key="item.key">
                <span>{{ item.key }}</span>
                :
                <span>{{ item.value }}</span>
              </li>
            </ul>
          </div>
        </div>
        <div class="request-flex-item border-bottom flex-start-center" v-if="model.params">
          <div class="flex-item-title w-100">请求体</div>
          <div class="flex-item-value" style="width: 100%; overflow: auto">
            <pre v-html="syntaxHighlight(model.params)"></pre>
          </div>
        </div>
        <div class="request-flex-item border-bottom flex-start-center">
          <div class="flex-start-center" style="width: 50%">
            <div class="flex-item-title w-100">响应状态码</div>
            <div class="flex-item-value">{{ model.statusCode }}</div>
          </div>
          <div class="flex-start-center border-left" style="width: 50%">
            <div class="flex-item-title w-100">响应耗时</div>
            <div class="flex-item-value">{{ model.elapsedMilliseconds }}ms</div>
          </div>
        </div>
        <div class="request-flex-item border-bottom flex-start-center">
          <div class="flex-item-title w-100">响应体</div>
          <div class="flex-item-value" style="width: 100%; overflow: auto">
            <pre v-html="syntaxHighlight(model.response)"></pre>
          </div>
        </div>
        <div class="request-flex-item border-bottom flex-start-center" v-show="model.exception">
          <div class="flex-item-title w-100">错误信息</div>
          <div class="flex-item-value" style="width: 100%; overflow: auto">
            {{ model.exception || '' }}
          </div>
        </div>
        <div class="request-flex-item border-bottom flex-start-center" v-show="model.stackTrace">
          <div class="flex-item-title w-100">错误堆栈</div>
          <div class="flex-item-value" style="width: 100%; overflow: auto">
            {{ model.stackTrace || '' }}
          </div>
        </div>
      </div>
    </div>
  </el-dialog>
</template>

<script setup>
import { reactive } from 'vue';
import { getInfo } from '../../../api/requestLog';

const model = reactive({
  dialogVisible: false,
  id: '',
  httpMethod: '',
  route: '',
  headers: [],
  params: '',
  success: true,
  statusCode: 0,
  response: '',
  elapsedMilliseconds: 8,
  ip: '',
  ipAddress: '',
  exception: '',
  stackTrace: '',
  createTime: ''
});

const getLogInfo = async id => {
  try {
    let res = await getInfo(id);
    if (res && res.resCode == 0) {
      return {
        statusCode: res.data.statusCode,
        headers: res.data.headers,
        params: res.data.params,
        response: res.data.response,
        exception: res.data.exception,
        stackTrace: res.data.stackTrace
      };
    }
  } catch {
    return false;
  }
};

const show = async row => {
  if (model.id != row.id) {
    model.headers = [];
    let result = await getLogInfo(row.id);

    if (!result) {
      model.id = '';
      model.route = '';
      model.headers = [];
      model.params = '';
      model.success = false;
      model.statusCode = 0;
      model.response = '';
      model.elapsedMilliseconds = 0;
      model.ip = '';
      model.ipAddress = '';
      model.exception = '';
      model.stackTrace = '';
      model.createTime = '';
      return;
    }

    model.id = row.id;
    model.httpMethod = row.httpMethod;
    model.route = row.route;

    if (result.headers && Object.keys(result.headers).length > 0) {
      for (let item in result.headers) {
        model.headers.push({
          key: item,
          value: result.headers[item]
        });
      }
    }

    model.success = row.success;
    model.elapsedMilliseconds = row.elapsedMilliseconds;
    model.ip = row.ip;
    model.ipAddress = row.ipAddress;
    model.createTime = row.createTime;

    model.statusCode = result.statusCode;
    model.params = result.params;
    model.response = result.response;
    model.exception = result.exception;
    model.stackTrace = result.stackTrace;
  }

  model.dialogVisible = true;
};

const syntaxHighlight = json => {
  json = json || {};
  if (typeof json == 'string') {
    try {
      let temp = JSON.parse(json);
      json = temp;
    } catch {
      return json;
    }
  }

  json = JSON.stringify(json, undefined, 2);

  json = json.replace(/&/g, '&').replace(/</g, '<').replace(/>/g, '>');

  return json.replace(/("(\\u[a-zA-Z0-9]{4}|\\[^u]|[^\\"])*"(\s*:)?|\b(true|false|null)\b|-?\d+(?:\.\d*)?(?:[eE][+\\-]?\d+)?)/g, function (match) {
    var cls = 'number';
    if (/^"/.test(match)) {
      if (/:$/.test(match)) {
        cls = 'key';
      } else {
        cls = 'string';
      }
    } else if (/true|false/.test(match)) {
      cls = 'boolean';
    } else if (/null/.test(match)) {
      cls = 'null';
    }
    return '<span class="' + cls + '">' + match + '</span>';
  });
};

defineExpose({
  show
});
</script>

<style lang="scss" scoped>
.descriptions {
  position: relative;
  max-height: 800px;
  width: 100%;
  overflow-y: auto;
  padding: 10px 20px;

  .request {
    border: 1px solid var(--color-secondary);

    .request-flex-item {
      align-items: stretch;
      overflow: hidden;

      .flex-item-title {
        flex-shrink: 0;
        background-color: #f5f7fa;
        color: var(--color-dark);
        padding: 10px;
        border-right: 1px solid var(--color-secondary);
        cursor: default;

        display: flex;
        align-items: center;
      }

      .flex-item-title.w-100 {
        width: 100px;
      }

      .flex-item-value {
        background-color: #fff;
        color: var(--color-dark);
        padding: 10px;
        overflow: hidden;
      }
    }

    .border-top {
      border-top: 1px solid var(--color-secondary);
    }

    .border-bottom {
      border-bottom: 1px solid var(--color-secondary);
    }

    .border-left {
      border-left: 1px solid var(--color-secondary);
    }

    .border-right {
      border-right: 1px solid var(--color-secondary);
    }

    pre {
      padding: 5px;
      font-family: Consolas, monaco, monospace;

      :deep(span) {
        overflow: hidden;
        word-break: break-all;
        white-space: normal;
      }

      :deep(.key) {
        color: var(--color-dark);
      }

      :deep(.string) {
        color: #3ab79f;
      }

      :deep(.number) {
        color: var(--color-warning);
      }

      :deep(.boolean) {
        color: var(--color-primary);
      }

      :deep(.null) {
        color: var(--color-pink);
      }
    }

    .request-headers {
      padding: 5px;
      li {
        list-style: none;
        display: block;
        overflow: hidden;
        word-break: break-all;
        padding: 2px 0;

        &:first-child {
          padding-top: 0;
        }

        &:last-child {
          padding-bottom: 0;
        }

        > span {
          line-height: 14px;
          &:last-child {
            cursor: pointer;
            transition: all 0.3s;
            &:hover {
              color: var(--color-primary);
            }
          }
        }
      }
    }
  }
}

@media (max-width: 1920px) {
  .descriptions {
    max-height: 600px;
  }
}
</style>
