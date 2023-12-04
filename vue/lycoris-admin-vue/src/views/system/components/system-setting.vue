<template>
  <div class="card-group">
    <div class="card-group-cell">
      <div class="card">
        <el-collapse v-model="collapse.showdoc" accordion>
          <el-collapse-item name="showdoc">
            <template #title>
              <div class="card-title">
                <span>Showdoc 公众号推送</span>
              </div>
            </template>

            <div class="card-body">
              <el-form label-position="top">
                <el-form-item label="推送地址">
                  <el-input v-model="showdoc.host" type="textarea" placeholder="https://push.showdoc.com.cn/server/api/push/**********************" :rows="3"></el-input>
                </el-form-item>
                <el-form-item label="推送测试标题">
                  <el-input v-model="showdoc.title" placeholder="请输入要推送测试标题"></el-input>
                </el-form-item>
                <el-form-item label="推送测试内容">
                  <el-input v-model="showdoc.content" type="textarea" :autosize="{ minRows: 3 }" placeholder="请输入要推送测试内容（内容支持文本、Markdown、html格式）"></el-input>
                </el-form-item>
              </el-form>
              <div style="text-align: right">
                <el-button @click="handleShowdocPushTest" :loading="showdoc.testLoading">推送测试</el-button>
                <el-button type="primary" @click="saveShowdoc" :loading="showdoc.saveLoading">保存</el-button>
              </div>
            </div>
          </el-collapse-item>
        </el-collapse>
      </div>
    </div>

    <div class="card-group-cell">
      <div class="card">
        <el-collapse v-model="collapse.systemFileClear" accordion>
          <el-collapse-item name="systemFileClear">
            <template #title>
              <div class="card-title">
                <span>系统文件清理</span>
              </div>
            </template>

            <div class="card-body">
              <p class="input-remind">天数范围 1 - 30 天</p>
              <el-form label-position="top">
                <div class="form-group">
                  <div class="form-text">
                    <label>静态文件保留</label>
                    <p style="color: var(--color-danger)">*仅会清理被标记为未使用的文件</p>
                  </div>

                  <el-input v-model="systemFileClear.staticFile" type="number" @input="value => fileClearInput(value, 'staticFile', 1, 30)">
                    <template #append>天</template>
                  </el-input>
                </div>
                <div class="form-group">
                  <div class="form-text">
                    <label>缓存文件保留</label>
                  </div>
                  <el-input v-model="systemFileClear.tempFile" type="number" @input="value => fileClearInput(value, 'tempFile', 1, 30)">
                    <template #append>天</template>
                  </el-input>
                </div>
                <div class="form-group">
                  <div class="form-text">
                    <label>日志文件保留</label>
                  </div>
                  <el-input v-model="systemFileClear.logFile" type="number" @input="value => fileClearInput(value, 'logFile', 1, 30)">
                    <template #append>天</template>
                  </el-input>
                </div>
              </el-form>
              <div style="text-align: right">
                <el-button type="primary" @click="savefileClear" :loading="systemFileClear.saveLoading">保存</el-button>
              </div>
            </div>
          </el-collapse-item>
        </el-collapse>
      </div>
    </div>

    <div class="card-group-cell">
      <div class="card">
        <el-collapse v-model="collapse.dbClear" accordion>
          <el-collapse-item name="dbClear">
            <template #title>
              <div class="card-title">
                <span>数据库清理</span>
              </div>
            </template>

            <div class="card-body">
              <p class="input-remind">0-表示不清理，天数范围 0 - 365 天</p>
              <el-form label-position="top">
                <div class="form-group">
                  <div class="form-text">
                    <label>请求日志保留</label>
                  </div>
                  <el-input v-model="systemDbClear.requestLog" type="number" @input="value => fileClearInput(value, 'requestLog', 0, 365)">
                    <template #append>天</template>
                  </el-input>
                </div>
                <div class="form-group">
                  <div class="form-text">
                    <label>浏览日志保留</label>
                  </div>
                  <el-input v-model="systemDbClear.browseLog" type="number" @input="value => fileClearInput(value, 'browseLog', 0, 365)">
                    <template #append>天</template>
                  </el-input>
                </div>
                <div class="form-group">
                  <div class="form-text">
                    <label>文章评论保留</label>
                  </div>
                  <el-input v-model="systemDbClear.postComment" type="number" @input="value => fileClearInput(value, 'postComment', 0, 365)">
                    <template #append>天</template>
                  </el-input>
                </div>
                <div class="form-group">
                  <div class="form-text">
                    <label>网站留言保留</label>
                  </div>
                  <el-input v-model="systemDbClear.leaveMessage" type="number" placeholder="范围 0- 365 天" @input="value => fileClearInput(value, 'leaveMessage', 0, 365)">
                    <template #append>天</template>
                  </el-input>
                </div>
              </el-form>
              <div style="text-align: right">
                <el-button type="primary" @click="saveDbClear" :loading="systemDbClear.saveLoading">保存</el-button>
              </div>
            </div>
          </el-collapse-item>
        </el-collapse>
      </div>
    </div>
  </div>
</template>

<script setup>
import { onMounted, reactive } from 'vue';
import { getSystemSetting, showdocPushTest, saveShowdocSetting, saveSystemFileClear, saveSystemDbClear } from '../../../api/configuration';
import toast from '../../../utils/toast';
import { debounce } from '../../../utils/tool';

const props = defineProps({
  value: {
    type: Number,
    required: true
  }
});

const collapse = reactive({
  showdoc: 'showdoc',
  systemFileClear: 'systemFileClear',
  dbClear: 'dbClear'
});

const showdoc = reactive({
  host: '',
  title: '',
  content: '',
  dialog: false,
  testLoading: false,
  saveLoading: false
});

const systemFileClear = reactive({
  staticFile: 1,
  tempFile: 1,
  logFile: 7,
  saveLoading: false
});

const systemDbClear = reactive({
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
    let res = await getSystemSetting();
    if (res && res.resCode == 0) {
      showdoc.host = res.data.showDocHost;

      systemFileClear.staticFile = res.data.systemFileClear.staticFile;
      systemFileClear.tempFile = res.data.systemFileClear.tempFile;
      systemFileClear.logFile = res.data.systemFileClear.logFile;

      systemDbClear.requestLog = res.data.systemDBClear.requestLog;
      systemDbClear.browseLog = res.data.systemDBClear.browseLog;
      systemDbClear.postComment = res.data.systemDBClear.postComment;
      systemDbClear.leaveMessage = res.data.systemDBClear.leaveMessage;
    }
  } catch (error) {}
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
    let res = await saveShowdocSetting(showdoc.host);
    if (res && res.resCode == 0) {
      toast.success('保存成功');
    }
  } finally {
    showdoc.saveLoading = false;
  }
};

const fileClearInput = debounce((value, model, min, max) => {
  if (min == 1 && max == 30) {
    if (value < min) {
      systemFileClear[model] = min;
    } else if (value > max) {
      systemFileClear[model] = max;
    }
  } else if (min == 0 && max == 365) {
    if (value < min) {
      systemDbClear[model] = min;
    } else if (value > max) {
      systemDbClear[model] = max;
    }
  }
}, 500);

const savefileClear = async () => {
  systemFileClear.saveLoading = true;
  try {
    let res = await saveSystemFileClear({ ...systemFileClear });
    if (res && res.resCode == 0) {
      toast.success('保存成功');
    }
  } finally {
    systemFileClear.saveLoading = false;
  }
};

const saveDbClear = async () => {
  systemDbClear.saveLoading = true;
  try {
    let res = await saveSystemDbClear({ ...systemDbClear });
    if (res && res.resCode == 0) {
      toast.success('保存成功');
    }
  } finally {
    systemDbClear.saveLoading = false;
  }
};
</script>

<style lang="scss" scoped>
.system-setting {
  .card-group {
    display: grid;
    grid-template-columns: repeat(auto-fill, minmax(300px, 500px));
    grid-gap: 15px;
    padding: 10px;

    .card-group-cell {
      padding: 10px;
    }

    .form-group {
      margin-bottom: 18px;

      .form-text {
        margin-bottom: 8px;
        line-height: 22px;
        font-size: var(--el-form-label-font-size);
        color: var(--el-text-color-regular);
      }
    }
  }

  .card {
    border: 1px solid var(--color-secondary);
    border-radius: 10px;
    box-shadow: 0 0 6px 5px var(--color-secondary);
    padding: 10px;

    :deep(.el-collapse) {
      border: 0;

      .el-collapse-item__header {
        border: 0;
      }

      .el-collapse-item__wrap {
        border: 0;
      }
    }

    .card-title {
      padding-left: 15px;

      > span {
        font-size: 18px;
        font-weight: 500;
      }
    }

    .card-body {
      padding: 10px 15px;

      .input-remind {
        color: var(--color-danger);
        margin: 0;
        padding-bottom: 10px;
      }
    }
  }
}
</style>
