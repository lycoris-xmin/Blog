<template>
  <div class="about-panel">
    <li-list :title="model.title" :list="model.list" @add="showModal()" @edit="(item, index) => showModal(item)" @delete="(item, index) => $delete(item, index)">
      <template #modal>
        <el-dialog v-model="model.visible" title="项目经验" width="500px" :close-on-click-modal="false" :before-close="close">
          <el-form label-width="100px" label-position="top">
            <el-form-item label="项目时间">
              <div class="flex-start-center">
                <el-date-picker v-model="model.beginTime" type="date" format="YYYY-MM-DD" value-format="YYYY-MM-DD" placeholder="项目开始时间" />
                <span style="padding: 0 5px">至</span>
                <el-date-picker v-model="model.endTime" type="date" format="YYYY-MM-DD" value-format="YYYY-MM-DD" placeholder="项目结束时间" />
              </div>
            </el-form-item>
            <el-form-item label="项目名称">
              <el-input v-model="model.name"></el-input>
            </el-form-item>
            <el-form-item label="项目描述">
              <el-input v-model="model.info" type="textarea" :autosize="{ minRows: 6, maxRows: 12 }" maxlength="300" show-word-limit></el-input>
            </el-form-item>
          </el-form>
          <template #footer>
            <el-button @click="close()">取消</el-button>
            <el-button type="primary" @click="add">添加</el-button>
          </template>
        </el-dialog>
      </template>
    </li-list>
  </div>
</template>

<script setup>
import { reactive, onMounted } from 'vue';
import liList from './li-list/index.vue';
import { getAboutMe, saveAboutMe } from '../../../api/configuration';
import swal from '../../../utils/swal';
import toast from '../../../utils/toast';

const model = reactive({
  title: ['项目时间', '项目名称', '项目信息', '操作'],
  list: [],
  visible: false,
  beginTime: '',
  endTime: '',
  name: '',
  info: ''
});

onMounted(async () => {
  let res = await getAboutMe('project');
  if (res && res.resCode == 0) {
    if (res.data) {
      let list = JSON.parse(res.data);
      if (list && list.length) {
        model.list = list;
      }
    }
  }
});

const showModal = item => {
  if (item) {
    model.beginTime = item.beginTime;
    model.endTime = item.endTime;
    model.name = item.name;
    model.info = item.info;
  }

  model.visible = true;
};

const $delete = async (item, index) => {
  //
  let res = await swal.confirm('确定要删除该数据吗?', '删除警告');
  if (res) {
    model.list.splice(index, 1);
  }
};

const close = done => {
  if (done == undefined) {
    done = () => {
      model.visible = false;
    };
  }

  model.beginTime = '';
  model.endTime = '';
  model.name = '';
  model.info = '';

  done();
};

const add = () => {
  //
  if (model.beginTime > model.endTime) {
    toast.warn('项目结束时间不允许比项目开始时间还早');
    return;
  }

  if (!model.name) {
    toast.warn('请输入项目名称');
    return;
  }

  if (!model.info) {
    toast.warn('请输入项目描述');
    return;
  } else if (model.info.length > 300) {
    toast.warn('项目描述字数不能超过300个字符');
    return;
  }

  model.list.push({
    beginTime: model.beginTime,
    endTime: model.endTime,
    name: model.name,
    info: model.info
  });

  model.list = model.list.orderBy('beginTime');

  close();
};

const submit = async () => {
  if (model.list && model.list.length > 0) {
    let res = await saveAboutMe('project', [...model.list]);
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
.project-body {
  ul {
    margin: 10px 0;
    border-top: 1px solid var(--color-secondary);
  }

  .project-li {
    list-style: none;
    padding: 10px 5px;
    border-radius: 5px;
    border-bottom: 1px solid var(--color-secondary);
    transition: all 0.3s;

    .project-action {
      display: flex;
      justify-content: flex-start;
      align-items: center;
      gap: 10px;

      .el-icon:first-child {
        color: var(--color-warning);
      }

      .el-icon:last-child {
        color: var(--color-danger);
      }
    }

    .el-icon {
      cursor: pointer;
      transition: all 0.4s;
    }

    .el-icon:hover {
      color: var(--color-info) !important;
    }

    .time {
      gap: 10px;
    }
  }

  .project-li:hover {
    background-color: var(--color-secondary);
  }

  .project-li.project-grid {
    display: grid;
    grid-template-columns: 300px 400px 1fr 150px;
    grid-gap: 10px;

    .flex-start-center {
      span {
        cursor: default;
      }
    }
  }
}
</style>
