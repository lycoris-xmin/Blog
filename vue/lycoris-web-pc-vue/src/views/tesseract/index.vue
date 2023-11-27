<template>
  <pageLayout title="文字识别" icon="tools">
    <div class="card">
      <div>
        <el-button type="primary" plain @click="recognizeText">文字识别</el-button>
      </div>
      <div class="tool-body">
        <div class="flex-center-center">
          <el-upload :auto-upload="false" :show-file-list="false" :on-change="fileChnage">
            <div class="imgfile-upload flex-center-center">
              <img v-if="model.imageUrl" :src="model.imageUrl" />
              <el-icon v-else :size="50">
                <component :is="'plus'"></component>
              </el-icon>
            </div>
          </el-upload>
        </div>
        <div class="flex-center-center">
          <el-input type="textarea" v-model="model.recognizedText" class="ocr-content"> </el-input>
        </div>
      </div>
    </div>
  </pageLayout>
</template>

<script setup>
import { reactive, onMounted } from 'vue';
import pageLayout from '../layout/page-layout.vue';
import Tesseract from 'tesseract.js';

const model = reactive({
  recognizedText: '',
  imageUrl: '',
  imageFile: void 0
});

const emit = defineEmits(['loading', 'browse']);

onMounted(() => {
  emit('loading', false);
  emit('browse');
});

const fileChnage = file => {
  model.imageFile = file.raw;
  model.imageUrl = URL.createObjectURL(file.raw);
};

const recognizeText = async () => {
  if (model.imageFile == void 0) {
    return;
  }
  //chi_sim
  const {
    data: { text }
  } = await Tesseract.recognize(model.imageFile, 'eng', { logger: m => console.log(m) });

  model.recognizedText = text;
  console.log(Tesseract);
  // const worker = createWorker({
  //   logger: m => console.log(m) // 可选：用于在控制台输出识别过程中的日志
  // });

  // await worker.load();
  // await worker.loadLanguage('eng');
  // await worker.initialize('eng');

  // const {
  //   data: { text }
  // } = await worker.recognize(model.imageFile);
  // model.recognizedText = text;

  // await worker.terminate();
};
</script>

<style lang="scss" scoped>
.card {
  .tool-body {
    height: calc(100vh - 305px);
    display: grid;
    grid-template-columns: repeat(2, 1fr);
    grid-gap: 20px;

    .imgfile-upload {
      border-radius: 8px;
      border: 2px dashed var(--color-secondary);

      height: 700px;
      width: 700px;
      cursor: pointer;
      transition: all 0.3s;

      .el-icon {
        transition: all 0.3s;
      }

      img {
        padding: 5px;
        height: 700px;
        width: 700px;
        border-radius: 8px;
        object-fit: fill;
      }
    }

    .imgfile-upload:hover {
      border-color: var(--color-info);

      .el-icon {
        color: var(--color-info);
      }
    }

    .ocr-content {
      :deep(textarea) {
        height: 700px;
      }
    }
  }
}
</style>
