<template>
  <el-collapse v-model="model.activeName" accordion>
    <el-collapse-item name="1">
      <template #title>
        <p class="collapse-title">Showdoc推送设置</p>
      </template>

      <div class="harf-body showdoc">
        <el-form label-position="top">
          <el-form-item label="推送地址">
            <el-input v-model="showdoc.host" type="textarea" placeholder="https://push.showdoc.com.cn/server/api/push/**********************" :rows="3"></el-input>
          </el-form-item>
          <div class="form-group flex-start-center">
            <label>监控推送</label>
            <el-switch v-model="showdoc.monitoringPush" inline-prompt active-text="启用" inactive-text="禁用" @change="handleMonitoringPushChange" />
          </div>

          <div class="monitor-input" :class="{ show: showdoc.showMonitor }">
            <el-form-item label="CPU推送阈值">
              <el-input v-model="showdoc.cpuRate" type="number" @input="value => showdocInput(value, 'cpuRate', 50, 100)">
                <template #append>%</template>
              </el-input>
            </el-form-item>

            <el-form-item label="内存推送阈值">
              <el-input v-model="showdoc.ramRate" type="number" @input="value => showdocInput(value, 'ramRate', 50, 100)">
                <template #append>%</template>
              </el-input>
            </el-form-item>
          </div>

          <div class="form-group flex-start-center">
            <label>评论推送</label>
            <el-switch v-model="showdoc.commentPush" inline-prompt active-text="启用" inactive-text="禁用" />
          </div>

          <div class="form-group flex-start-center">
            <label>留言推送</label>
            <el-switch v-model="showdoc.messagePush" inline-prompt active-text="启用" inactive-text="禁用" />
          </div>
        </el-form>

        <div>
          <el-button @click="showTestModal">推送测试</el-button>
          <el-button type="primary" @click="saveShowdoc" :loading="showdoc.saveLoading">保存</el-button>
        </div>
      </div>

      <el-dialog v-model="showdoc.visible" title="推送测试" width="550px">
        <el-form label-position="top">
          <el-form-item label="推送测试标题">
            <el-input v-model="showdoc.title" placeholder="请输入要推送测试标题"></el-input>
          </el-form-item>
          <el-form-item label="推送测试内容">
            <el-input v-model="showdoc.content" type="textarea" :autosize="{ minRows: 6 }" placeholder="请输入要推送测试内容（内容支持文本、Markdown、html格式）"></el-input>
          </el-form-item>
        </el-form>

        <template #footer>
          <span class="dialog-footer">
            <el-button @click="closeShowdocTest">关闭</el-button>
            <el-button type="primary" @click="handleShowdocPushTest" :loading="showdoc.testLoading">测试</el-button>
          </span>
        </template>
      </el-dialog>
    </el-collapse-item>

    <el-collapse-item name="2">
      <template #title>
        <p class="collapse-title">数据清理设置</p>
      </template>

      <div class="clear-body">
        <div class="form-body">
          <el-divider content-position="left">
            <p class="form-title">文件清理</p>
          </el-divider>
          <div class="input-info">
            <span>时间范围 0 - 30 天,0 - 表示不清理</span>
          </div>
          <div class="form-group-body">
            <div class="form-group">
              <div class="form-text">
                <label>静态文件清理</label>
                <small>*仅清理未使用的文件</small>
              </div>
              <el-input v-model="dataClear.staticFile" type="number" @input="value => fileClearInput(value, 'staticFile', 1, 30)">
                <template #append>天</template>
              </el-input>
            </div>
            <div class="form-group">
              <div class="form-text">
                <label>缓存文件保留</label>
              </div>
              <el-input v-model="dataClear.tempFile" type="number" @input="value => fileClearInput(value, 'tempFile', 1, 30)">
                <template #append>天</template>
              </el-input>
            </div>
            <div class="form-group">
              <div class="form-text">
                <label>日志文件保留</label>
              </div>
              <el-input v-model="dataClear.logFile" type="number" @input="value => fileClearInput(value, 'logFile', 1, 30)">
                <template #append>天</template>
              </el-input>
            </div>
          </div>

          <el-divider content-position="left">
            <p class="form-title">数据库清理</p>
          </el-divider>
          <div class="input-info">
            <span>时间范围 0 - 365 天,0 - 表示不清理</span>
          </div>
          <div class="form-group-body">
            <div class="form-group">
              <div class="form-text">
                <label>请求日志保留</label>
              </div>
              <el-input v-model="dataClear.requestLog" type="number" @input="value => fileClearInput(value, 'requestLog', 0, 365)">
                <template #append>天</template>
              </el-input>
            </div>
            <div class="form-group">
              <div class="form-text">
                <label>浏览日志保留</label>
              </div>
              <el-input v-model="dataClear.browseLog" type="number" @input="value => fileClearInput(value, 'browseLog', 0, 365)">
                <template #append>天</template>
              </el-input>
            </div>
            <div class="form-group">
              <div class="form-text">
                <label>文章评论保留</label>
              </div>
              <el-input v-model="dataClear.postComment" type="number" @input="value => fileClearInput(value, 'postComment', 0, 365)">
                <template #append>天</template>
              </el-input>
            </div>
            <div class="form-group">
              <div class="form-text">
                <label>网站留言保留</label>
              </div>
              <el-input v-model="dataClear.leaveMessage" type="number" placeholder="范围 0- 365 天" @input="value => fileClearInput(value, 'leaveMessage', 0, 365)">
                <template #append>天</template>
              </el-input>
            </div>
          </div>
        </div>

        <el-button type="primary" :loading="dataClear.saveLoading" @click="handleSaveDataClear">保存</el-button>
      </div>
    </el-collapse-item>

    <el-collapse-item name="3">
      <template #title>
        <p class="collapse-title">打赏设置</p>
      </template>
    </el-collapse-item>
  </el-collapse>
</template>

<script setup>
import { onMounted, reactive } from 'vue';
import { getOthersetting, saveDataClear, showdocPushTest, saveShowdocSetting } from '../../../api/configuration';
import toast from '../../../utils/toast';
import { debounce } from '../../../utils/tool';

const props = defineProps({
  value: {
    type: Number,
    required: true
  }
});

const model = reactive({
  activeName: '1'
});

const showdoc = reactive({
  host: '',
  monitoringPush: false,
  cpuRate: 50,
  ramRate: 50,
  commentPush: false,
  messagePush: false,
  showMonitor: false,
  title: '',
  content: '',
  visible: false,
  testLoading: false,
  saveLoading: false
});

const dataClear = reactive({
  staticFile: 1,
  tempFile: 1,
  logFile: 7,
  requestLog: 30,
  browseLog: 30,
  postComment: 0,
  leaveMessage: 0,
  saveLoading: false
});

const emit = defineEmits(['tabComplete']);

onMounted(async () => {
  await getSettings();
  emit('tabComplete', props.value);
});

const getSettings = async () => {
  try {
    let res = await getOthersetting();
    if (res && res.resCode == 0) {
      showdoc.host = res.data.showdoc.host;
      showdoc.monitoringPush = res.data.showdoc.monitoringPush;
      showdoc.cpuRate = res.data.showdoc.cpuRate;
      showdoc.ramRate = res.data.showdoc.ramRate;
      showdoc.commentPush = res.data.showdoc.commentPush;
      showdoc.messagePush = res.data.showdoc.messagePush;

      dataClear.staticFile = res.data.dataClear.staticFile;
      dataClear.tempFile = res.data.dataClear.tempFile;
      dataClear.logFile = res.data.dataClear.logFile;

      dataClear.requestLog = res.data.dataClear.requestLog;
      dataClear.browseLog = res.data.dataClear.browseLog;
      dataClear.postComment = res.data.dataClear.postComment;
      dataClear.leaveMessage = res.data.dataClear.leaveMessage;
    }
  } catch (error) {}
};

const showdocInput = debounce((value, model, min, max) => {
  if (value < min) {
    showdoc[model] = min;
  } else if (value > max) {
    showdoc[model] = max;
  }
}, 500);

const handleMonitoringPushChange = value => {
  showdoc.showMonitor = value;
};

const showTestModal = () => {
  if (!showdoc.host) {
    toast.warn('请先填写推送地址');
    return;
  }
  showdoc.visible = true;
};

const closeShowdocTest = () => {
  showdoc.visible = false;
  showdoc.title = '';
  showdoc.content = '';
};

const handleShowdocPushTest = async () => {
  if (!showdoc.host) {
    toast.warn('推送地址不能为空');
    return;
  }

  if (!showdoc.title) {
    toast.warn('推送测试标题不能为空');
    return;
  }

  if (!showdoc.content) {
    toast.warn('推送测试内容不能为空');
    return;
  }

  let data = {
    host: showdoc.host,
    title: showdoc.title,
    content: showdoc.content
  };
  showdoc.testLoading = true;
  //
  try {
    let res = await showdocPushTest(data);
    if (res && res.resCode == 0) {
      toast.success('公众号消息已推送');
      showdoc.title = '';
      showdoc.content = '';
    }
  } finally {
    showdoc.testLoading = false;
  }
};

const saveShowdoc = async () => {
  if (!showdoc.host) {
    toast.warn('推送地址不能为空');
    return;
  }

  showdoc.saveLoading = true;

  try {
    let res = await saveShowdocSetting({ ...showdoc });
    if (res && res.resCode == 0) {
      toast.success('保存成功');
    }
  } finally {
    showdoc.saveLoading = false;
  }
};

const fileClearInput = debounce((value, model, min, max) => {
  if (value < min) {
    dataClear[model] = min;
  } else if (value > max) {
    dataClear[model] = max;
  }
}, 500);

const handleSaveDataClear = async () => {
  dataClear.saveLoading = true;

  try {
    let res = await saveDataClear({ ...dataClear });
    if (res && res.resCode == 0) {
      toast.success('保存成功');
    }
  } finally {
    dataClear.saveLoading = false;
  }
};
</script>

<style lang="scss" scoped>
.collapse-title {
  font-size: 18px;
  letter-spacing: 2.5px;
  font-weight: 500;
}

.showdoc {
  .form-group {
    margin-bottom: 18px;
  }

  .monitor-input {
    display: grid;
    grid-template-columns: repeat(2, 1fr);
    grid-gap: 35px;
    height: 0px;
    overflow: hidden;
    transition: all 0.4s;

    &.show {
      height: 80px;
    }
  }
}

.harf-body {
  padding: 25px 10px;
}

.clear-body {
  width: 100%;

  .input-info {
    padding-top: 5px;

    span {
      font-size: 14px;
      background-color: var(--color-danger);
      color: #fff;
      padding: 2px 8px;
      border-radius: 2px;
      cursor: default;

      &::before {
        content: '配置说明：';
      }
    }
  }

  .form-body {
    .form-title {
      font-size: 16px;
      font-weight: 400;
    }

    .form-group-body {
      display: flex;
      justify-content: flex-start;
      align-items: flex-end;
      padding: 15px 0;
      padding-bottom: 45px;
    }
  }
}

.form-group {
  width: 350px;
  margin-right: 18px;

  > label:first-child {
    padding-right: 10px;
    font-size: 14px;

    &::after {
      content: '：';
    }
  }

  .form-text {
    padding-bottom: 5px;

    small {
      color: var(--color-danger);
      padding-left: 5px;
    }
  }
}
</style>
