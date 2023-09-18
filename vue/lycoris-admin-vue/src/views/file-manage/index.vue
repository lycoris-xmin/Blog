<template>
  <page-layout :loading="model.loading"> </page-layout>
</template>

<script setup name="file-manage">
import { reactive, onMounted } from 'vue';
import PageLayout from '../layout/page-layout.vue';

const model = reactive({
  loading: true
});

onMounted(() => {
  model.loading = false;
});
</script>

<style lang="scss" scoped></style>
