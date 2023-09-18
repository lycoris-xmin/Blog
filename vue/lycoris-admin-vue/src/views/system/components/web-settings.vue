<template>
  <div style="padding-top: 18px">
    <el-form label-position="left" :model="model" :label-width="110">
      <el-row :gutter="24">
        <el-col :span="8">
          <el-form-item label="网站名称">
            <el-input v-model="model.webName"></el-input>
          </el-form-item>
          <el-form-item label="网站前台地址">
            <el-input v-model="model.webPath"></el-input>
          </el-form-item>
          <el-form-item label="后台登录地址">
            <el-input v-model="model.adminPath"></el-input>
          </el-form-item>
          <el-form-item label="网站建立时间">
            <el-date-picker class="date" v-model="model.buildTime" type="date" placeholder="网站建立时间" format="YYYY-MM-DD" />
          </el-form-item>
          <div class="submit">
            <el-button type="primary" :loading="model.loading" @click="submit">保存</el-button>
          </div>
        </el-col>
      </el-row>
    </el-form>
  </div>
</template>

<script setup>
import { onMounted, reactive } from 'vue';
import { getWebSettings, saveWebSettings } from '../../../api/configuration';
import toast from '../../../utils/toast';

const props = defineProps({
  value: {
    type: Number,
    required: true
  }
});

const model = reactive({
  webName: '',
  webPath: '',
  adminPath: '',
  buildTime: '',
  loading: false
});

const emit = defineEmits(['tabComplete']);

onMounted(async () => {
  try {
    let res = await getWebSettings();
    if (res.resCode == 0) {
      model.webName = res.data.webName;
      model.webPath = res.data.webPath;
      model.adminPath = res.data.adminPath;
      model.buildTime = res.data.buildTime || '';
    }
  } finally {
    emit('tabComplete', props.value);
  }
});

const submit = async () => {
  model.loading = true;

  try {
    let res = await saveWebSettings({ ...model });
    if (res.resCode == 0) {
      toast.success('保存成功');
    }
  } finally {
    model.loading = false;
  }
};
</script>

<style lang="scss" scoped>
.submit {
  text-align: left;

  .el-button {
    width: 120px;
  }
}
</style>
