<template>
  <div class="code">
    <div class="code-header">
      <span>{{ props.title }}</span>
    </div>
    <transition-list>
      <li>
        <div class="skill-li">
          <div>技术栈</div>
          <div>掌握度</div>
          <div>操作</div>
        </div>
      </li>

      <li v-for="(item, index) in model.data" :key="item.id">
        <div class="skill-li" v-show="!item.modify">
          <el-tooltip :content="item.name" v-if="item.name.length > 20">
            <div class="skill-name">{{ item.name }}</div>
          </el-tooltip>
          <div v-else class="skill-name">{{ item.name }}</div>
          <div class="skill-proficiency">{{ item.proficiency }}</div>
          <div class="skill-action">
            <el-icon :size="20" @click="showEditPanel(index)">
              <component :is="'edit'"></component>
            </el-icon>
            <el-icon :size="20" @click="$delete(index)">
              <component :is="'close'"></component>
            </el-icon>
          </div>
        </div>
        <div v-if="item.modify">
          <div class="action-panel">
            <div>
              <el-input v-model="item.name" :ref="`editNameRef${index}`" placeholder="技术名称" />
            </div>
            <div>
              <el-input v-model="item.proficiency" placeholder="掌握程度" type="number" />
            </div>
            <div class="flex-center-center">
              <el-button @click="cancelEditInputConfirm(index)">取消</el-button>
              <el-button @click="handleEditInputConfirm(index)">确定</el-button>
            </div>
          </div>
        </div>
      </li>

      <li v-show="model.showAddPanel">
        <div class="action-panel">
          <div>
            <el-input v-model="model.name" ref="nameRef" placeholder="技术名称" />
          </div>
          <div>
            <el-input v-model="model.proficiency" placeholder="掌握程度" type="number" />
          </div>
          <div class="flex-center-center">
            <el-button @click="cancelInputConfirm">取消</el-button>
            <el-button @click="handleInputConfirm">确定</el-button>
          </div>
        </div>
      </li>

      <li class="flex-center-center add" @click="showAddPanel">
        <el-icon :size="16">
          <component :is="'plus'"></component>
        </el-icon>
      </li>
    </transition-list>
  </div>
</template>

<script setup>
import { ref, nextTick, reactive, watch, getCurrentInstance } from 'vue';
import transitionList from '../../../../components/transitions/list-transition.vue';
import swal from '../../../../utils/swal';

const that = getCurrentInstance();
const nameRef = ref();

const model = reactive({
  name: '',
  proficiency: '',
  data: []
});

const props = defineProps({
  title: {
    type: String,
    default: ''
  },
  data: {
    type: Array,
    default: void 0
  }
});

watch(
  () => props.data,
  value => {
    if (!value || !value.length) {
      model.data = [];
      return;
    }

    model.data = value?.map(x => {
      return {
        id: x.name,
        name: x.name,
        proficiency: x.proficiency,
        modify: false
      };
    });
  }
);

const emit = defineEmits(['skillChange']);

const $delete = index => {
  swal.confirm('确定要删除该数据吗?', '删除确认').then(result => {
    if (result) {
      model.data.splice(index, 1);
      emit('skillChange', model.data);
    }
  });
};

const showAddPanel = () => {
  model.showAddPanel = true;
  nextTick(() => {
    nameRef.value.input.focus();
  });
};

const showEditPanel = index => {
  model.data[index].modify = true;
  cancelInputConfirm();
  nextTick(() => {
    that.refs[`editNameRef${index}`][0].focus();
  });
};

const cancelInputConfirm = () => {
  model.showAddPanel = false;
  model.name = '';
  model.proficiency = '';
};

const handleInputConfirm = () => {
  if (model.name && model.proficiency) {
    model.data.push({
      id: model.name,
      name: model.name,
      proficiency: model.proficiency,
      modify: false
    });

    let map = model.data.map(x => {
      return {
        name: x.name.trim(),
        proficiency: x.proficiency
      };
    });
    emit('skillChange', map);
    model.showAddPanel = false;
  } else if (!model.name && (!model.proficiency || model.proficiency == 0)) {
    model.showAddPanel = false;
  }

  if (!model.showAddPanel) {
    model.name = '';
    model.proficiency = '';
  }
};

const cancelEditInputConfirm = index => {
  model.data[index].name = props.data[index].name;
  model.data[index].proficiency = props.data[index].proficiency;
  model.data[index].modify = false;
};

const handleEditInputConfirm = index => {
  const item = model.data[index];
  if (item.name && item.proficiency) {
    emit('skillChange', model.data);
    model.data[index].modify = false;
  }
};
</script>

<style lang="scss" scoped>
.code {
  .code-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    padding-right: 20px;
    margin-bottom: 10px;
  }

  ul {
    padding: 10px;
    border-radius: 5px;
    border: 2px dashed var(--color-secondary);

    li {
      list-style: none;
      padding: 10px 5px;
      border-radius: 5px;
      border-bottom: 1px solid var(--color-secondary);
      margin: 5px 0;
      transition: all 0.3s;

      .skill-li {
        display: grid;
        grid-template-columns: 4fr 0.6fr 0.8fr;
        grid-gap: 10px;

        @media (max-width: 1920px) {
          grid-template-columns: 4fr 0.9fr 0.8fr;
        }

        .skill-name {
          overflow: hidden;
          text-overflow: ellipsis;
          white-space: nowrap;
          word-break: break-all;
          cursor: default;
        }

        .skill-proficiency {
          text-align: right;
          cursor: default;
        }

        .skill-action {
          display: flex;
          justify-content: space-around;
          align-items: center;

          .el-icon:first-child {
            color: var(--color-warning);
            cursor: pointer;
            transition: all 0.4s;
          }

          .el-icon:last-child {
            color: var(--color-danger);
            cursor: pointer;
            transition: all 0.4s;
          }

          .el-icon:hover {
            color: var(--color-info);
          }
        }
      }

      .action-panel {
        display: grid;
        grid-template-columns: 3fr 2fr 2fr;
        grid-gap: 10px;
      }
    }

    li.add {
      cursor: pointer;
    }

    li:hover {
      background-color: var(--color-secondary);

      .el-icon {
        color: var(--color-info);
      }

      :deep(.el-slider__runway) {
        background-color: #fff;
      }
    }
  }
}

.slide_animate-enter-active {
  animation: slideInDown 0.5s;
}

.slide_animate-leave-active {
  animation: slideOutUp 0.3s;
}
</style>
