<template>
  <page-layout :loading="model.loading">
    <div class="search-panel">
      <el-form :label-position="'top'" ref="formRef">
        <el-form-item class="form-group" label="标题">
          <el-input v-model="model.title" />
        </el-form-item>
        <el-form-item class="form-group" label="文章类型">
          <el-select v-model="model.type" placeholder="- 全部 -" clearable>
            <el-option :key="0" :label="'原创'" :value="0" />
            <el-option :key="1" :label="'转载'" :value="1" />
          </el-select>
        </el-form-item>
        <el-form-item class="form-group" label="分类">
          <el-select v-model="model.category" placeholder="- 全部 -" clearable>
            <el-option v-for="item in stores.enums.category" :key="item.value" :label="item.name" :value="item.value" />
          </el-select>
        </el-form-item>
        <el-form-item class="form-group" label="文章状态">
          <el-select v-model="model.isPublish" placeholder="- 全部 -" clearable>
            <el-option :key="0" :label="'草稿'" :value="0" />
            <el-option :key="1" :label="'发布'" :value="1" />
          </el-select>
        </el-form-item>
      </el-form>
    </div>

    <lycoris-table
      :column="column"
      :table-height="'calc(100vh - 390px)'"
      :page-index="table.pageIndex"
      :page-size="table.pageSize"
      :list="table.list"
      :count="table.count"
      :loading="table.loading"
      :toolbar="{ search: true, create: true }"
      @page-change="handleCurrentChange"
      @toolbar-create="$create"
      @toolbar-search="$search"
    >
      <template #icon="{ value }">
        <div class="icon flex-center-center">
          <img :src="value" onerror="javascript:this.src='/images/404.png'" />
        </div>
      </template>

      <template #title="{ value, row: { id, type, info, browseCount, commentCount } }">
        <div class="post">
          <div class="title">
            <a :href="`${stores.webSetting.webPath.trimEnd('/')}/post/${id}`" target="_blank">
              <span class="badge" :class="{ 'badge-purple': type == 0, 'badge-info': type == 1 }">{{ type == 0 ? '原创' : '转载' }}</span>
              <span>{{ value }}</span>
            </a>
          </div>
          <div class="info">{{ info }}...</div>
          <div class="statistics">
            <el-icon>
              <component :is="'reading'"></component>
            </el-icon>
            <span>{{ countChange(browseCount) }}</span>
            <span class="divider">/</span>
            <el-icon>
              <component :is="'message'"></component>
            </el-icon>
            <span>{{ countChange(commentCount) }}</span>
          </div>
        </div>
      </template>

      <template #comment="{ index, scope }">
        <el-switch
          v-model="scope.row.comment"
          :loading="model.commentSwitchLoading[index]"
          :before-change="$event => commentChange(index, scope.row.id)"
          :inactive-value="0"
          :active-value="1"
          inline-prompt
          active-text="开启"
          inactive-text="关闭"
        />
      </template>

      <template #recommend="{ index, scope }">
        <el-switch
          v-model="scope.row.recommend"
          :loading="model.recommendSwitchLoading[index]"
          :before-change="$event => recommendChange(index, scope.row)"
          :inactive-value="0"
          :active-value="1"
          active-text="推荐"
          inline-prompt
        />
      </template>

      <template #isPublish="{ value }">
        <el-tag class="cell-tag" :type="value == 1 ? 'success' : 'warning'">{{ value == 1 ? '发布' : '草稿' }}</el-tag>
      </template>

      <template #action="{ index, row }">
        <el-popconfirm title="确定要发布该文章吗?" @confirm="$publish(index, row.id)" v-if="row.isPublish == 0">
          <template #reference>
            <el-button type="success" plain>发布</el-button>
          </template>
        </el-popconfirm>
        <el-button type="primary" plain @click="$update(row.id)">编辑</el-button>
        <el-popconfirm title="确定要删除吗?" @confirm="$delete(index, row)">
          <template #reference>
            <el-button type="danger" :loading="row.loading" plain>删除</el-button>
          </template>
        </el-popconfirm>
      </template>
    </lycoris-table>
  </page-layout>
</template>

<script setup name="post">
import { reactive, onMounted, ref } from 'vue';
import { useRouter } from 'vue-router';
import pageLayout from '../layout/page-layout.vue';
import LycorisTable from '../../components/lycoris-table/index.vue';
import { getList as getPostList, deletePost, publishPost, setPostComment, setPostRecommend } from '../../api/post.js';
import { getCategoryEnums } from '../../api/category';
import { stores } from '../../stores';
import toast from '../../utils/toast';
import { countChange } from '../../utils/tool';

const router = useRouter();
const formRef = ref();

const model = reactive({
  loading: true,
  commentSwitchLoading: [],
  recommendSwitchLoading: [],
  title: '',
  type: null,
  category: null,
  isPublish: null
});

const column = [
  {
    column: 'icon',
    name: '封面图',
    width: '210px'
  },
  {
    column: 'title',
    name: '标题'
  },
  {
    column: 'categoryName',
    name: '文章分类',
    width: '200px',
    overflow: true
  },
  {
    column: 'comment',
    name: '评论',
    align: 'center',
    width: '80px'
  },
  {
    column: 'recommend',
    name: '推荐',
    align: 'center',
    width: '80px'
  },
  {
    column: 'isPublish',
    name: '状态',
    align: 'center',
    width: '80px'
  },
  {
    column: 'action',
    name: '操作',
    width: '280px',
    align: 'center',
    fixed: 'right'
  }
];

const table = reactive({
  count: 0,
  list: [],
  pageIndex: 1,
  pageSize: 10,
  loading: false
});

onMounted(async () => {
  getCategory();
  await getTableList(false);
  model.loading = false;
});

const getCategory = async () => {
  try {
    if (!stores.enums.category.length) {
      let res = await getCategoryEnums();
      if (res && res.resCode == 0) {
        stores.enums.setCategory(res.data.list);
      }
    }
  } catch (error) {}
};

const handleCurrentChange = pageIndex => {
  table.pageIndex = pageIndex;
  getTableList();
};

const getTableList = async (autoLoading = true) => {
  if (autoLoading) {
    table.loading = true;
  }

  try {
    let res = await getPostList({
      pageIndex: table.pageIndex,
      pageSize: table.pageSize,
      title: model.title,
      type: model.type,
      category: model.category,
      isPublish: model.isPublish
    });

    if (res.resCode == 0) {
      table.count = res.data.count;
      table.list = res.data.list;

      if (table.list > 0) {
        model.commentSwitchLoading = [];
        model.recommendSwitchLoading = [];
        table.list.forEach(() => {
          model.commentSwitchLoading.push(false);
          model.recommendSwitchLoading.push(false);
        });
      }
    }
  } finally {
    if (autoLoading) {
      table.loading = false;
    }
  }
};

const $create = () => {
  router.push({
    name: 'post-editor'
  });
};

const $search = () => {
  table.pageIndex = 1;
  getTableList();
};

const commentChange = async (index, id) => {
  model.commentSwitchLoading[index] = true;

  try {
    let data = {
      id,
      comment: table.list[index].comment == 0 ? 1 : 0
    };
    let res = await setPostComment(data);
    if (res && res.resCode == 0) {
      if (data.comment) {
        toast.success('开启文章评论功能');
      } else {
        toast.warn('关闭文章评论功能');
      }
      return Promise.resolve(true);
    } else {
      return Promise.reject();
    }
  } catch {
    return Promise.reject();
  } finally {
    model.commentSwitchLoading[index] = false;
  }
};

const recommendChange = async (index, row) => {
  if (!row.isPublish) {
    toast.info('文章还未发布，无法设置为推荐文章');
    return Promise.reject();
  }

  model.recommendSwitchLoading[index] = true;
  try {
    let data = {
      id: row.id,
      recommend: table.list[index].recommend == 0 ? 1 : 0
    };
    let res = await setPostRecommend(data);
    if (res && res.resCode == 0) {
      if (data.recommend) {
        toast.success('设置文章推荐');
      } else {
        toast.warn('取消文章推荐');
      }
      return Promise.resolve(true);
    } else {
      return Promise.reject();
    }
  } catch {
    return Promise.reject();
  } finally {
    model.recommendSwitchLoading[index] = false;
  }
};

const $publish = async (index, id) => {
  let res = await publishPost(id);
  if (res && res.resCode == 0) {
    table.list[index].isPublish = true;
    toast.success('发布成功');
  }
};

const $update = id => {
  router.push({
    name: 'post-editor',
    query: {
      id: id
    }
  });
};

const $delete = async (index, row) => {
  row.loading = true;
  try {
    let res = await deletePost(row.id);
    if (res.resCode == 0) {
      if (table.list.length <= table.count) {
        table.list.splice(index, 1);
        table.count--;
      } else {
        getTableList();
      }

      toast.success('删除成功');
    }
  } finally {
    row.loading = false;
  }
};
</script>

<style lang="scss" scoped>
.icon {
  img {
    height: 140px;
    width: 210px;
    object-fit: cover;
    border: 1px solid var(--color-secondary);
    border-radius: 8px;
    cursor: pointer;
  }
}

.post {
  min-height: 140px;
  padding: 10px 0;
  display: flex;
  flex-direction: column;
  justify-content: flex-start;
  align-items: flex-start;

  .title {
    font-size: 18px;
    font-weight: 500;
    padding-bottom: 10px;

    cursor: pointer;

    a,
    a:active,
    a:focus {
      color: var(--color-dark);
      transition: color 0.5s;
    }

    a:hover {
      color: var(--color-info);
    }

    span.badge {
      font-size: 12px;
    }
  }

  .info {
    font-size: 14px;
    color: var(--color-dark-light);
    overflow: hidden;
    text-overflow: ellipsis;
    display: -webkit-box;
    -webkit-line-clamp: 3; // 显示几行
    -webkit-box-orient: vertical;
    height: 70px;
  }

  .statistics {
    display: flex;
    justify-content: start;
    align-items: center;
    font-size: 14px;
    color: var(--color-dark-light);
    padding-top: 5px;

    .el-icon {
      margin-right: 5px;
    }

    .divider {
      padding: 0 10px;
    }
  }
}

:deep(.cell-tag) {
  span {
    font-size: 14px !important;
  }
}
</style>
