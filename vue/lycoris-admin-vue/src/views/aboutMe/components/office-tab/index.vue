<template>
  <div class="about-panel">
    <office-list :list="model.list" @add="showModal()" @edit="(item, index) => showModal(item, index)" @delete="(item, index) => $delete(item, index)">
      <template #modal>
        <el-dialog v-model="model.visible" title="就职经历" width="500px" :close-on-click-modal="false" :before-close="close">
          <el-form label-width="100px" label-position="top">
            <el-form-item label="就职时间">
              <div class="flex-start-center">
                <el-date-picker v-model="model.beginTime" type="date" format="YYYY-MM-DD" value-format="YYYY-MM-DD" placeholder="入职时间" />
                <span style="padding: 0 5px">至</span>
                <el-date-picker v-model="model.endTime" type="date" format="YYYY-MM-DD" value-format="YYYY-MM-DD" placeholder="离职时间" />
              </div>
            </el-form-item>
            <el-form-item label="入职公司 ">
              <el-input v-model="model.company"></el-input>
            </el-form-item>
            <el-form-item label="入职岗位">
              <el-input v-model="model.job"></el-input>
            </el-form-item>
            <el-form-item label="工作内容">
              <el-input v-model="model.info" type="textarea" :autosize="{ minRows: 6, maxRows: 12 }" maxlength="300" show-word-limit></el-input>
            </el-form-item>
          </el-form>
          <template #footer>
            <el-button @click="close()">取消</el-button>
            <el-button type="primary" @click="add">确定</el-button>
          </template>
        </el-dialog>
      </template>
    </office-list>
  </div>
</template>

<script setup>
import { reactive, onMounted } from 'vue';
import officeList from './office-list.vue';
import { getAboutMe, saveAboutMe } from '../../../../api/website';
import swal from '../../../../utils/swal';
import toast from '../../../../utils/toast';

const model = reactive({
  list: [],
  visible: false,
  index: -1,
  beginTime: '',
  endTime: '',
  job: '',
  company: '',
  info: ''
});

onMounted(async () => {
  let res = await getAboutMe('office');
  if (res && res.resCode == 0) {
    if (res.data) {
      let list = JSON.parse(res.data);
      if (list && list.length) {
        model.list = list;
      }
    }
  }
});

const showModal = (item, index) => {
  if (item) {
    model.beginTime = item.beginTime;
    model.endTime = item.endTime;
    model.company = item.company;
    model.job = item.job;
    model.info = item.info;

    model.index = index || -1;
  }

  model.visible = true;
};

const $delete = async (item, index) => {
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

  if (!model.company) {
    toast.warn('请输入公司名称');
    return;
  }

  if (!model.job) {
    toast.warn('请输入岗位名称');
    return;
  }

  if (!model.info) {
    toast.warn('请输入主要工作内容');
    return;
  } else if (model.info.length > 300) {
    toast.warn('主要工作内容字数不能超过300个字符');
    return;
  }

  if (model.index == -1) {
    model.list.push({
      beginTime: model.beginTime,
      endTime: model.endTime,
      company: model.company,
      job: model.job,
      info: model.info
    });

    model.list = model.list.orderBy('beginTime');
  } else {
    model[model.index] = {
      beginTime: model.beginTime,
      endTime: model.endTime,
      company: model.company,
      job: model.job,
      info: model.info
    };
  }

  close();
};

const submit = async () => {
  if (model.list && model.list.length > 0) {
    let res = await saveAboutMe('office', [...model.list]);
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
.flex-start-center {
  span {
    cursor: default;
  }
}
</style>
