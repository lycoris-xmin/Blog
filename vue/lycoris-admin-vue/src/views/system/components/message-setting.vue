<template>
  <div class="tab-panel-container">
    <div class="harf-body">
      <el-form>
        <div class="form-panel">
          <div class="header remind-header">
            <p>留言提醒<small class="title-info-text">可拖动改变顺序</small></p>
            <el-button type="primary" plain @click="showEditor">新增</el-button>
          </div>
          <ul ref="remindRef">
            <li v-for="(item, index) in model.messageRemind" :key="item" class="remind">
              <div class="remind-view">
                <p class="text">{{ item }}</p>
                <div class="flex-center-center">
                  <el-icon :size="18" class="action-flag" @click="showEditor(item, index)">
                    <component :is="'edit'"></component>
                  </el-icon>
                  <el-icon :size="18" class="action-flag" @click="deleteRemind(index)">
                    <component :is="'delete'"></component>
                  </el-icon>
                </div>
              </div>
            </li>
          </ul>
          <p v-if="model.messageRemind.length == 0">暂无数据</p>
        </div>

        <div class="form-panel in-line">
          <p class="header">留言设置<small class="title-info-text">限制用户留言频率，避免被人恶意留言导致数据库崩溃</small></p>
          <el-form-item label="留言频率">
            <el-input v-model="model.frequencySecond" type="number" placeholder="0-表示不限制"> <template #append>秒</template></el-input>
          </el-form-item>
        </div>

        <div class="submit">
          <el-button type="primary" :loading="model.loading" @click="submit">保存</el-button>
        </div>
      </el-form>
    </div>

    <el-dialog v-model="model.editorVisible" title="留言提醒" width="500px">
      <el-input ref="remindInputRef" v-model="model.remind" type="textarea" :autosize="{ minRows: 4 }" maxlength="100" show-word-limit></el-input>
      <template #footer>
        <span class="dialog-footer">
          <el-button @click="closeEditor">关闭</el-button>
          <el-button type="primary" @click="addMessageRemind"> 确定 </el-button>
        </span>
      </template>
    </el-dialog>
  </div>
</template>

<script setup>
import { onMounted, reactive, ref } from 'vue';
import Sortable from 'sortablejs/modular/sortable.core.esm.js';
import { getMessageSetting, saveMessageSetting } from '@/api/configuration';
import toast from '../../../utils/toast';

const props = defineProps({
  value: {
    type: Number,
    required: true
  }
});

const remindRef = ref();
const remindInputRef = ref();

const model = reactive({
  messageRemind: [],
  frequencySecond: 0,
  editorVisible: false,
  remind: '',
  index: void 0,
  loading: false
});

const emit = defineEmits(['tabComplete']);

onMounted(async () => {
  new Sortable(remindRef.value, {
    animation: 150,
    chosenClass: 'active',
    onEnd: function (evt) {
      const changeData = model.messageRemind.splice(evt.oldIndex || 0, 1);
      model.messageRemind.splice(evt.newIndex || 0, 0, changeData[0]);
    }
  });

  await getSetting();

  emit('tabComplete', props.value);
});

const getSetting = async () => {
  try {
    let res = await getMessageSetting();
    if (res && res.resCode == 0) {
      model.messageRemind = res.data.messageRemind;
      model.frequencySecond = res.data.frequencySecond;
    }
  } catch (error) {}
};

const deleteRemind = index => {
  model.messageRemind.splice(index, 1);
};

const showEditor = (item, index) => {
  //
  if (item && typeof index == 'number') {
    model.remind = item;
    model.index = index;
  }

  model.editorVisible = true;

  setTimeout(() => {
    remindInputRef.value.focus();
  }, 50);
};

const closeEditor = () => {
  model.remind = '';
  model.index = void 0;
  model.editorVisible = false;
};

const addMessageRemind = () => {
  if (model.remind) {
    if (model.index == undefined) {
      model.messageRemind.push(model.remind);
    } else {
      model.messageRemind[model.index] = model.remind;
    }
  }
  closeEditor();
};

const submit = async () => {
  //
  model.loading = true;
  try {
    let res = await saveMessageSetting({ ...model });
    if (res && res.resCode == 0) {
      toast.success('保存成功');
    }
  } finally {
    model.loading = false;
  }
};
</script>

<style lang="scss" scoped>
.remind-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.remind {
  padding: 5px 8px;
  margin: 5px 0;
  padding: 10px 8px;
  border-radius: 5px;
  border: 1px solid var(--color-secondary);
  cursor: move;

  .remind-view {
    display: flex;
    justify-content: space-between;
    align-items: center;

    p.text {
      overflow: hidden;
      word-break: break-all;
      text-overflow: ellipsis;
      word-wrap: nowrap;
      line-height: 30px;
      padding: 0 5px;
    }

    .flex-center-center {
      cursor: default;

      .action-flag {
        cursor: pointer;

        &:first-child {
          margin-right: 10px;
        }

        &:hover {
          color: var(--color-danger);
        }
      }
    }
  }

  &.active {
    color: #fff;
    background-color: var(--color-primary-light);
  }
}
</style>
