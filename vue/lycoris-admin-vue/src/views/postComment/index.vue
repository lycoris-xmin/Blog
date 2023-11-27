<template>
  <page-layout :loading="model.loading">
    <div class="search-panel">
      <el-form :label-position="'top'" ref="formRef">
        <el-form-item class="form-group" label="标题">
          <el-input v-model="model.title" />
        </el-form-item>
        <el-form-item class="form-group" label="评论内容">
          <el-input v-model="model.content" />
        </el-form-item>
        <el-form-item class="form-group" label="评论用户">
          <el-autocomplete v-model="model.userInput" :fetch-suggestions="querySearchUser" @select="handleSelect" :value-key="'name'" />
        </el-form-item>
      </el-form>
    </div>

    <lycoris-table
      :table-height="'calc(100vh - 385px)'"
      :column="column"
      :page-index="table.pageIndex"
      :page-size="table.pageSize"
      :list="table.list"
      :count="table.count"
      :loading="table.loading"
      :toolbar="{ search: true }"
      @page-change="handleCurrentChange"
      @toolbar-search="$search"
    >
      <template #userName="{ row }">
        <div class="flex-start-center">
          <el-tooltip v-if="row.isOwner" effect="dark" content="自己" placement="top">
            <span>{{ row.userName }}</span>
          </el-tooltip>

          <span v-else>{{ row.userName }}</span>
        </div>
      </template>

      <template #action="{ row, index }">
        <el-button type="info" plain v-if="row.originalContent" @click="$view(row)">查看</el-button>

        <el-popconfirm title="确定要删除吗?" @confirm="$delete(index, row)">
          <template #reference>
            <el-button type="danger" :loading="row.loading" plain>删除</el-button>
          </template>
        </el-popconfirm>
      </template>
    </lycoris-table>

    <el-dialog v-model="model.dialogVisible" title="评论原内容" width="30%">
      <div class="word-auto-line" style="font-size: 16px; letter-spacing: 2px; line-height: 35px">
        {{ model.originalContent }}
      </div>
    </el-dialog>
  </page-layout>
</template>

<script setup name="comment">
import { ref, reactive, onMounted } from 'vue';
import PageLayout from '../layout/page-layout.vue';
import LycorisTable from '../../components/lycoris-table/index.vue';
import { getCommentList, deleteComment } from '../../api/postComment';
import toast from '../../utils/toast';

const model = reactive({
  loading: true,
  title: '',
  content: '',
  userInput: '',
  user: void 0,
  dialogVisible: false,
  originalContent: ''
});

const column = ref([
  {
    column: 'title',
    name: '文章标题',
    width: 250,
    overflow: true
  },
  {
    column: 'content',
    name: '评论内容'
  },
  {
    column: 'userName',
    name: '评论用户',
    width: 220
  },
  {
    column: 'createTime',
    name: '评论时间',
    width: 220
  },
  {
    column: 'ipAddress',
    name: 'IP归属地',
    width: 200
  },
  {
    column: 'action',
    name: '操作',
    width: 200,
    fixed: 'right'
  }
]);

const table = reactive({
  count: 0,
  list: [],
  pageIndex: 1,
  pageSize: 15,
  loading: false
});

onMounted(async () => {
  Object.freeze(column);
  await getTableList();
  model.loading = false;
});

const getTableList = async () => {
  try {
    table.loading = true;

    let res = await getCommentList({
      pageIndex: table.pageIndex,
      pageSize: table.pageSize,
      title: model.title,
      content: model.content,
      userId: model.user?.id
    });

    if (res && res.resCode == 0) {
      table.count = res.data.count;
      table.list = res.data.list;
    }
  } finally {
    table.loading = false;
  }
};

const querySearchUser = (val, callback) => {
  if (!val || model.user?.name == val) {
    callback([]);
    return;
  }

  callback([
    {
      name: '123',
      id: 1
    }
  ]);
};

const handleSelect = item => {
  model.user = item;
};

const $search = () => {
  table.pageIndex = 1;
  getTableList();
};

const handleCurrentChange = index => {
  table.pageIndex = index;
  getTableList();
};

const $view = row => {
  model.originalContent = row.originalContent;
  model.dialogVisible = true;
};

const $delete = async (index, row) => {
  //
  row.loading = true;

  try {
    let res = await deleteComment(row.id);
    if (res && res.resCode == 0) {
      toast.success('删除成功');
      if (table.list.length == table.count || table.list.length < table.pageSize) {
        table.list.splice(index, 1);
        table.count--;
      } else {
        getTableList();
      }
    }
  } finally {
    row.loading = false;
  }
};
</script>

<style lang="scss" scoped></style>
