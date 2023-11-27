<template>
  <div class="about-panel">
    <div class="skill-grid">
      <skill-panel title="前端" :data="skill.web" @skill-change="data => skillChange('web', data)"></skill-panel>
      <skill-panel title="后端" :data="skill.server" @skill-change="data => skillChange('server', data)"></skill-panel>
      <skill-panel title="中间件及服务" :data="skill.middleware" @skill-change="data => skillChange('middleware', data)"></skill-panel>
      <skill-panel title="数据库" :data="skill.sql" @skill-change="data => skillChange('sql', data)"></skill-panel>
    </div>
    <div class="description">
      <label>其他描述</label>
      <el-input v-model="skill.description" :autosize="{ minRows: 6, maxRows: 10 }" type="textarea" show-word-limit maxlength="300"></el-input>
    </div>
  </div>
</template>

<script setup>
import { onMounted, reactive } from 'vue';
import skillPanel from './skill-panel.vue';
import { getAboutMe, saveAboutMe } from '../../../../api/website';

const skill = reactive({
  web: [],
  server: [],
  middleware: [],
  sql: [],
  description: ''
});

onMounted(async () => {
  let res = await getAboutMe('skill');

  if (res && res.resCode == 0) {
    if (res.data) {
      let data = JSON.parse(res.data);

      skill.web = data.web || [];
      skill.server = data.server || [];
      skill.middleware = data.middleware || [];
      skill.sql = data.sql || [];
      if (data.description && data.description.length) {
        skill.description = data.description.join('\n');
      }
    }
  }
});

const skillChange = (targrt, map) => {
  skill[targrt] = map.orderDescBy('proficiency');
};

const getSkillData = () => {
  let data = {};

  if (skill.web && skill.web.length) {
    data.web = skill.web;
  }

  if (skill.server && skill.server.length) {
    data.server = skill.server;
  }

  if (skill.middleware && skill.middleware.length) {
    data.middleware = skill.middleware;
  }

  if (skill.sql && skill.sql.length) {
    data.sql = skill.sql;
  }

  if (skill.description) {
    data.description = skill.description.split('\n');
  }

  return Object.keys(data).length == 0 ? void 0 : data;
};

const submit = async () => {
  let data = getSkillData();
  if (data) {
    let res = await saveAboutMe('skill', data);
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
.skill-grid {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  grid-gap: 20px;
}

.description {
  margin: 40px 0;

  .el-textarea {
    margin-top: 10px;
  }
}
</style>
