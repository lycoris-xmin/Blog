<template>
  <div class="tab-panel-container">SEO未开发</div>
</template>

<script setup>
import { onMounted } from 'vue';

const props = defineProps({
  value: {
    type: Number,
    required: true
  }
});

const emit = defineEmits(['tabComplete']);

onMounted(() => {
  emit('tabComplete', props.value);
});
</script>

<style lang="scss" scoped></style>
