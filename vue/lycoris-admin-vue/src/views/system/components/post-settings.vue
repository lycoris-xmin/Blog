<template>
  <div style="padding-top: 18px">
    <el-form>
      <el-row :gutter="24">
        <el-col :span="12">
          <el-card>
            <template #header>
              <div class="card-header">
                <span>编写设置</span>
              </div>
            </template>
            <el-col :span="10">
              <el-form-item label="自动保存">
                <el-switch v-model="model.autoSave" inline-prompt style="--el-switch-off-color: #ff4949" active-text="开" inactive-text="关" />
              </el-form-item>
              <el-form-item label="保存间隔">
                <el-input v-model="model.second" type="number" placeholder="0"> <template #append>秒</template></el-input>
              </el-form-item>
              <el-form-item label="博客文章随机图"></el-form-item>
            </el-col>
            <div class="post-random-img-group">
              <div class="random-img-group flex-start-center">
                <div class="random-img" v-for="(item, index) in model.images" :key="index">
                  <img :src="item" />
                  <div class="img-wrap">
                    <div class="wrap-body flex-center-center">
                      <el-icon :size="30" @click="deleteRandowImg(index)">
                        <component :is="'delete'"></component>
                      </el-icon>
                    </div>
                  </div>
                </div>
                <div class="random-img" v-show="model.images.length < 6">
                  <div class="upload-body flex-center-center" @click="addRandowImg">
                    <input type="file" accept="image/*" ref="uploadRandomImage" @change="randomImageChange" hidden />
                    <el-icon :size="30">
                      <component :is="'plus'"></component>
                    </el-icon>
                  </div>
                </div>
              </div>
            </div>
          </el-card>
        </el-col>
      </el-row>
      <div class="submit">
        <el-button type="primary" :loading="model.loading" @click="submit">保存</el-button>
      </div>
    </el-form>
  </div>
</template>

<script setup>
import { reactive, ref, onMounted } from 'vue';
import { getPostSettings, savePostSettings, uploadFile } from '../../../api/configuration';
import toast from '../../../utils/toast';

const uploadRandomImage = ref();

const props = defineProps({
  value: {
    type: Number,
    required: true
  }
});

const model = reactive({
  autoSave: false,
  second: 0,
  images: ['/images/404.png', '/images/404.png', '/images/404.png', '/images/404.png', '/images/404.png', '/images/404.png'],
  files: [],
  loading: false
});

const emit = defineEmits(['tabComplete']);

onMounted(async () => {
  try {
    let res = await getPostSettings();
    if (res && res.resCode == 0) {
      model.autoSave = res.data.autoSave;
      model.second = res.data.second;
      model.images = res.data.images || [];

      model.images.forEach(() => model.files.push(''));
    }
  } finally {
    emit('tabComplete', props.value);
  }
});

const deleteRandowImg = index => {
  model.images.splice(index, 1);
  model.files.splice(index, 1);
};

const addRandowImg = () => {
  uploadRandomImage.value.click();
};

const randomImageChange = e => {
  if (e.target && e.target.files.length) {
    model.images.push(URL.createObjectURL(e.target.files[0]));
    model.files.push(e.target.files[0]);
  }
};

const submit = async () => {
  let data = {
    autoSave: model.autoSave,
    second: model.second,
    images: model.images
  };

  model.loading = true;

  // 先上传文件
  try {
    if (model.files && model.files.length) {
      for (let i = 0; i < model.files.length; i++) {
        let file = model.files[i];
        if (file) {
          let res = await uploadFile('App.PostSettings', file);
          if (res && res.resCode == 0) {
            data.images[i] = res.data;
          } else {
            toast.error('上传图片失败');
            return;
          }
        }
      }
    }

    let res = await savePostSettings(data);
    if (res && res.resCode == 0) {
      model.files = [];
      //
      toast.success('保存成功');
    }
  } finally {
    model.loading = false;
  }
};
</script>

<style lang="scss" scoped>
.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.post-random-img-group {
  margin: 0 auto;

  .random-img-group {
    flex-wrap: wrap;
    flex-direction: row;
  }

  .random-img {
    position: relative;
    height: 140px;
    width: 210px;
    margin: 10px;

    img {
      height: 140px;
      width: 210px;
      object-fit: cover;
      border: 2px dashed var(--color-dark-light);
    }

    .img-wrap {
      position: absolute;
      top: 0;
      left: 0;
      height: 140px;
      width: 210px;
      background: #00000085;
      display: none;

      .wrap-body {
        height: 100%;
        width: 100%;

        :deep(.el-icon) {
          color: #fff;
          cursor: pointer;
          transition: all 0.5s;
        }

        :deep(.el-icon):hover {
          color: var(--color-danger);
        }
      }
    }

    .upload-body {
      height: 100%;
      width: 100%;
      border: 2px dashed var(--color-dark-light);
      transition: all 0.2s;
      cursor: pointer;

      :deep(.el-icon) {
        cursor: pointer;
        transition: all 0.2s;
      }
    }

    .upload-body:hover {
      border-color: var(--color-info);

      :deep(.el-icon) {
        color: var(--color-info);
      }
    }
  }

  .random-img:hover {
    .img-wrap {
      display: block;
    }
  }
}

.submit {
  margin-top: 20px;
  text-align: left;

  .el-button {
    width: 120px;
  }
}
</style>
