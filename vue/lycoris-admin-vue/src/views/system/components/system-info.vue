<template>
  <div class="monitor">
    <div class="server-rate">
      <el-progress type="dashboard" :percentage="80" :color="model.progressColors">
        <template #default="{ percentage }">
          <p class="percentage-value">{{ percentage }}%</p>
          <span class="percentage-label">CPU使用率</span>
        </template>
      </el-progress>
      <el-progress type="dashboard" :percentage="60" :color="model.progressColors">
        <template #default="{ percentage }">
          <p class="percentage-value">{{ percentage }}%</p>
          <span class="percentage-label">内存使用率</span>
        </template>
      </el-progress>
    </div>
    <div class="server-info">
      <el-descriptions :column="1" border>
        <el-descriptions-item label="系统内存">{{ model.totalRam }}</el-descriptions-item>
        <el-descriptions-item label="系统类型">{{ model.osDescription }}</el-descriptions-item>
        <el-descriptions-item label="系统架构">{{ model.osArchitecture }}</el-descriptions-item>
        <el-descriptions-item label="系统启动时间">{{ model.runTime }}</el-descriptions-item>
        <el-descriptions-item label="系统运行时间">{{}}</el-descriptions-item>
      </el-descriptions>
    </div>
  </div>
</template>

<script setup>
import { reactive, onMounted } from 'vue';

const props = defineProps({
  value: {
    type: Number,
    required: true
  }
});

const model = reactive({
  progressColors: [
    { color: '#f56c6c', percentage: 20 },
    { color: '#e6a23c', percentage: 40 },
    { color: '#5cb87a', percentage: 60 },
    { color: '#1989fa', percentage: 80 },
    { color: '#6f7ad3', percentage: 100 }
  ],
  totalRam: '2GB',
  osDescription: 'Linux 4.18.0-147.5.1.el8_1.x86_64 #1 SMP Wed Feb 5 02:00:39 UTC 2020',
  osArchitecture: 'X64',
  runTime: '2000-01-01 00:00:00'
});

const emit = defineEmits(['tabComplete']);

onMounted(() => {
  try {
  } finally {
    emit('tabComplete', props.value);
  }
});
</script>

<style lang="scss" scoped>
.monitor {
  padding: 30px 0;
  width: 1400px;
  margin: 0 auto;

  .server-rate {
    display: flex;
    gap: 100px;
    justify-content: center;
    align-items: center;

    .percentage-value {
      margin-bottom: 10px;
    }

    .percentage-label {
      font-size: 14px;
    }
  }

  .server-info {
    padding: 20px 100px;
  }
}
</style>
