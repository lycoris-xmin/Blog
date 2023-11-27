<template>
  <div class="tab-panel-container">
    <div class="harf-body">
      <el-form>
        <div class="form-panel">
          <div class="header">
            <p>留言提醒</p>
            <el-button>新增</el-button>
          </div>
          <ul ref="remindRef">
            <li draggable="true" v-for="item in model.messageRemind" :key="item" class="remind">
              <p class="text">{{ item }}</p>
              <el-icon :size="18" class="delete-flag">
                <component :is="'delete'"></component>
              </el-icon>
            </li>
          </ul>
        </div>
      </el-form>
    </div>
  </div>
</template>

<script setup>
import { onMounted, reactive, ref } from 'vue';

const props = defineProps({
  value: {
    type: Number,
    required: true
  }
});

const remindRef = ref();

const model = reactive({
  messageRemind: ['提醒一', '提醒二', '提醒三', '提醒四', '提醒五', '提醒六', '提醒日'],
  sortable: void 0
});

const emit = defineEmits(['tabComplete']);

onMounted(() => {
  emit('tabComplete', props.value);
});
</script>

<style lang="scss" scoped>
.header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.remind {
  padding: 5px 8px;
  margin: 1px 0;
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 5px 8px;
  border-radius: 5px;
  border: 1px solid var(--color-secondary);
  transition: all 0.4s;

  .delete-flag {
    cursor: pointer;
  }

  span.text {
    cursor: default;
  }
}
</style>
