<template>
  <div class="about-panel">
    <div class="form-group form-group-col">
      <label>个人简介</label>
      <el-input v-model="info.description" type="textarea" :autosize="{ minRows: 6, maxRows: 10 }" show-word-limit maxlength="350" placeholder="非必填" />
    </div>

    <div class="body-grid">
      <div class="form-group form-group-col">
        <label>邮箱</label>
        <el-tooltip effect="dark" content="基础信息请在个人资料中修改" placement="bottom">
          <el-input :value="props.owner.email" disabled />
        </el-tooltip>
      </div>
      <div class="form-group form-group-col">
        <label>QQ</label>
        <el-tooltip effect="dark" content="基础信息请在个人资料中修改" placement="bottom">
          <el-input :value="props.owner.qq" disabled />
        </el-tooltip>
      </div>
      <div class="form-group form-group-col">
        <label>微信</label>
        <el-tooltip effect="dark" content="基础信息请在个人资料中修改" placement="bottom">
          <el-input :value="props.owner.wechat" disabled />
        </el-tooltip>
      </div>
      <div class="form-group form-group-col">
        <label>GitHub</label>
        <el-tooltip effect="dark" content="基础信息请在个人资料中修改" placement="bottom">
          <el-input :value="props.owner.gitHub" disabled />
        </el-tooltip>
      </div>
      <div class="form-group form-group-col">
        <label>哔哩哔哩</label>
        <el-tooltip effect="dark" content="基础信息请在个人资料中修改" placement="bottom">
          <el-input :value="props.owner.bilibili" disabled />
        </el-tooltip>
      </div>
      <div class="form-group form-group-col">
        <label>网易云音乐</label>
        <el-tooltip effect="dark" content="基础信息请在个人资料中修改" placement="bottom">
          <el-input :value="props.owner.cloudMusic" disabled />
        </el-tooltip>
      </div>
    </div>

    <div class="body-grid" style="margin: 10px 0">
      <div class="form-group form-group-col">
        <label>学历</label>
        <el-select v-model="info.educational" clearable>
          <el-option label="初中" value="初中" />
          <el-option label="高中" value="高中" />
          <el-option label="中专" value="中专" />
          <el-option label="大专" value="大专" />
          <el-option label="本科" value="本科" />
          <el-option label="硕士研究生" value="硕士研究生" />
          <el-option label="博士研究生" value="博士研究生" />
        </el-select>
      </div>
      <div class="form-group form-group-col">
        <label>院校类别</label>
        <el-select v-model="info.institutions" clearable>
          <el-option label="双非" :value="0" />
          <el-option label="211" :value="1" />
          <el-option label="985" :value="2" />
        </el-select>
      </div>
      <div class="form-group form-group-col">
        <label>出生年月</label>
        <el-date-picker v-model="info.birth" type="date" format="YYYY-MM-DD" value-format="YYYY-MM-DD" />
      </div>
      <div class="form-group form-group-col">
        <label>性别</label>
        <el-select v-model="info.sex">
          <el-option label="保密" :value="0" />
          <el-option label="男" :value="1" />
          <el-option label="女" :value="2" />
        </el-select>
      </div>
    </div>

    <div class="tag-content">
      <label>语言掌握</label>
      <div class="flex-start-center">
        <el-tag class="tag-m" v-for="code in info.code" :key="code" closable :disable-transitions="false" @close="handleClose('code', code)">
          {{ code }}
        </el-tag>
        <el-input class="tag-input" v-if="model.codeInputVisible" ref="codeInputRef" v-model="model.codeInput" @keyup.enter="handleInputConfirm('code')" @blur="handleInputConfirm('code')" />
        <el-button v-else type="primary" plain @click="showInput('code')" size="small"> 添加语言</el-button>
      </div>
    </div>

    <div class="tag-content">
      <label>爱好标签</label>
      <div class="flex-start-center">
        <el-tag class="tag-m" v-for="hobby in info.hobby" :key="hobby" closable :disable-transitions="false" @close="handleClose('hobby', hobby)">
          {{ hobby }}
        </el-tag>
        <el-input class="tag-input" v-if="model.hobbyInputVisible" ref="hobbyInputRef" v-model="model.hobbyInput" @keyup.enter="handleInputConfirm('hobby')" @blur="handleInputConfirm('hobby')" />
        <el-button v-else type="primary" plain @click="showInput('hobby')" size="small"> 添加爱好标签</el-button>
      </div>
    </div>

    <div class="form-group form-group-col">
      <label>关于我</label>
      <el-input v-model="info.introduction" type="textarea" :autosize="{ minRows: 6, maxRows: 10 }" show-word-limit maxlength="350" />
    </div>
  </div>
</template>

<script setup>
import { ref, reactive, nextTick, onMounted } from 'vue';
import { getAboutMe, saveAboutMe } from '../../../api/website-about';
import toast from '../../../utils/toast';

const props = defineProps({
  owner: {
    type: Object,
    default: void 0
  }
});

const codeInputRef = ref();
const hobbyInputRef = ref();

const model = reactive({
  codeInputVisible: false,
  codeInput: '',
  hobbyInputVisible: false,
  hobbyInput: ''
});

const info = reactive({
  description: '',
  educational: '',
  institutions: 0,
  sex: 0,
  birth: '',
  code: [],
  hobby: [],
  introduction: ''
});

onMounted(async () => {
  let res = await getAboutMe('info');
  if (res && res.resCode == 0) {
    if (res.data) {
      let data = JSON.parse(res.data);

      if (data.description && data.description.length) {
        info.description = data.description.join('\n');
      }

      info.educational = data.educational;
      info.institutions = data.institutions == undefined ? 0 : data.institutions;
      info.sex = data.sex || 0;
      info.birth = data.birth || '';
      info.code = data.code || [];
      info.hobby = data.hobby || [];

      if (data.introduction && data.introduction.length) {
        info.introduction = data.introduction.join('\n');
      }
    }
  }
});

const handleClose = (target, value) => {
  info[target].splice(info[target].indexOf(value), 1);
};

const showInput = target => {
  if (target == 'code') {
    model.codeInputVisible = true;
    nextTick(() => {
      codeInputRef.value.input.focus();
    });
  } else {
    model.hobbyInputVisible = true;
    nextTick(() => {
      hobbyInputRef.value.input.focus();
    });
  }
};

const handleInputConfirm = target => {
  if (target == 'code') {
    if (model.codeInput) {
      if (info.code.includes(model.codeInput)) {
        toast.warn(`${model.codeInput} 语言已存在`);
        return;
      }

      info.code.push(model.codeInput);
    }

    model.codeInputVisible = false;
    model.codeInput = '';
  } else {
    if (model.hobbyInput) {
      if (info.hobby.includes(model.hobbyInput)) {
        toast.warn(`${model.hobbyInput} 爱好标签已存在`);
        return;
      }

      info.hobby.push(model.hobbyInput);
    }

    model.hobbyInputVisible = false;
    model.hobbyInput = '';
  }
};

const getInfo = () => {
  let data = {};

  if (info.description) {
    data.description = info.description.split('\n');
  }

  if (info.educational) {
    data.educational = info.educational;

    if (info.institutions) {
      data.institutions = parseInt(info.institutions);
    }
  }

  if (info.birth != '') {
    data.birth = info.birth;
  }

  if (info.code && info.code.length) {
    data.code = info.code;
  }

  if (info.hobby && info.hobby.length) {
    data.hobby = info.hobby;
  }

  if (info.introduction) {
    data.introduction = info.introduction.split('\n');
  }

  return Object.keys(data).length == 0 ? void 0 : data;
};

const submit = async () => {
  let data = getInfo();
  if (data) {
    let res = await saveAboutMe('info', data);
    if (res && res.resCode != 0) {
      return false;
    }
  }

  return true;
};

defineExpose({
  submit
});
</script>

<style lang="scss" scoped>
.body-grid {
  display: grid;
  grid-template-columns: repeat(6, 1fr);
  grid-gap: 10px;

  @media (max-width: 1920px) {
    grid-template-columns: repeat(4, 1fr);
  }

  .grid-all-row {
    grid-column-start: 1;
    grid-column-end: 7;

    @media (max-width: 1920px) {
      grid-column-end: 5;
    }
  }
}

.form-group {
  display: flex;
  justify-content: flex-start;
  align-items: center;

  label {
    padding-bottom: 4px;
    flex-shrink: 0;
    padding: 5px;
  }

  :deep(.el-date-editor) {
    width: 100%;
  }

  .el-select {
    width: 100%;
  }
}

.form-group-col {
  flex-direction: column;
  align-items: flex-start;
}

.tag-content {
  padding: 10px 5px;
  display: flex;
  justify-content: flex-start;
  align-items: flex-start;
  flex-direction: column;

  label {
    flex-shrink: 0;
    padding: 5px 0;
  }

  .tag-input {
    width: 200px;
  }

  .tag-m {
    margin-right: 10px;
    padding: 4px 6px;
    cursor: default;

    :deep(span:first-child) {
      font-size: 16px;
    }
  }

  .el-button {
    :deep(> span) {
      font-size: 14px;
    }
  }
}
</style>
