<template>
  <page-layout :loading="model.loading" :affix="true">
    <template #tool>
      <el-button type="primary" @click="submit">保存</el-button>
    </template>

    <el-tabs tab-position="top" class="about-tabs">
      <transition-list :tag="'div'">
        <el-tab-pane label="个人信息">
          <info-tab :owner="stores.owner" ref="infoRef"></info-tab>
        </el-tab-pane>
        <el-tab-pane label="技能掌握">
          <skill-tab ref="skillRef"></skill-tab>
        </el-tab-pane>
        <el-tab-pane label="项目经验">
          <project-tab ref="projectRef"></project-tab>
        </el-tab-pane>
        <el-tab-pane label="工作经历">
          <office-tab ref="officeRef"></office-tab>
        </el-tab-pane>
      </transition-list>
    </el-tabs>
  </page-layout>
</template>

<script setup name="about-web">
import { onMounted, reactive, ref } from 'vue';
import pageLayout from '../layout/page-layout.vue';
import transitionList from '../../components/transitions/list-transition.vue';
import infoTab from './components/info-tab.vue';
import skillTab from './components/skill-tab/index.vue';
import projectTab from './components/project-tab.vue';
import officeTab from './components/office-tab/index.vue';
import { stores } from '../../stores';
import { getUserBrief } from '../../api/user';
import toast from '../../utils/toast';

const infoRef = ref();
const skillRef = ref();
const projectRef = ref();
const officeRef = ref();

const model = reactive({
  loading: true
});

onMounted(async () => {
  try {
    //
    await ownerInit();
  } finally {
    model.loading = false;
  }
});

const ownerInit = async () => {
  try {
    if (!stores.owner.isValid) {
      let res = await getUserBrief();

      if (res && res.resCode == 0) {
        stores.owner.setData(res.data);
      }
    }
  } catch (error) {}
};

const submit = async () => {
  {
    let res = await infoRef.value.submit();
    if (!res) {
      toast.error('保存个人信息失败');
      return;
    }
  }

  {
    let res = await skillRef.value.submit();
    if (!res) {
      toast.error('保存技能掌握信息失败');
      return;
    }
  }

  {
    let res = await projectRef.value.submit();
    if (!res) {
      toast.error('保存项目经验失败');
      return;
    }
  }

  {
    let res = await officeRef.value.submit();
    if (!res) {
      toast.error('保存工作经历失败');
      return;
    }
  }

  toast.success('保存成功');
};
</script>

<style lang="scss" scoped>
.about-panel {
  padding-bottom: 20px;
  margin-bottom: 15px;
}

.about-tabs {
  height: 100%;

  :deep(.el-tabs__item) {
    font-size: 18px;
  }
}
</style>
