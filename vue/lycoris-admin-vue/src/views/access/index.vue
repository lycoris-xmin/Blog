<template>
  <page-layout :loading="model.loading"> </page-layout>
</template>

<script setup name="access">
import { onMounted, reactive } from 'vue';
import pageLayout from '../layout/page-layout.vue';

const model = reactive({
  loading: true
});

onMounted(() => {
  model.loading = false;
});
</script>

<style scoped></style>
