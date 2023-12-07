<template>
  <page-layout :loading="model.loading">
    <div class="search-panel">
      <el-form :label-position="'top'" ref="formRef">
        <el-form-item class="form-group" label="收录名称">
          <el-input v-model="model.name" />
        </el-form-item>
        <el-form-item class="form-group" label="收录地址">
          <el-input v-model="model.domain" />
        </el-form-item>
        <el-form-item class="form-group" label="收录分组">
          <el-select v-model="model.group" placeholder="- 全部 -" clearable>
            <el-option v-for="item in model.groupOption" :key="item.value" :label="item.name" :value="item.value" />
          </el-select>
        </el-form-item>
      </el-form>
    </div>

    <lycoris-table
      :column="column"
      :page-index="table.pageIndex"
      :table-height="'calc(100vh - 385px)'"
      :page-size="table.pageSize"
      :hide-on-single-page="true"
      :list="table.list"
      :count="table.count"
      :loading="table.loading"
      :toolbar="{ search: true, create: true }"
      @page-change="handleCurrentChange"
      @toolbar-create="$create"
      @toolbar-search="$search"
    >
      <template #domain="{ value }">
        <a class="domain-link" :href="value" target="_blank">{{ value }}</a>
      </template>

      <template #action="{ row, index }">
        <el-button type="warning" plain @click="$update(index, row)">编辑</el-button>
        <el-popconfirm title="确定要删除吗?" @confirm="$delete(index, row)">
          <template #reference>
            <el-button type="danger" plain :loading="row.loading">删除</el-button>
          </template>
        </el-popconfirm>
      </template>
    </lycoris-table>

    <create-or-update ref="modalRef" :group="model.groupOption" @complete="complete"></create-or-update>
  </page-layout>
</template>

<script setup name="navigation">
import { reactive, ref, onMounted } from 'vue';
import pageLayout from '../layout/page-layout.vue';
import LycorisTable from '../../components/lycoris-table/index.vue';
import createOrUpdate from './components/createorupdate.vue';
import { getList, getGroupOptions, deleteSiteNavigation } from '../../api/navigation';
import toast from '../../utils/toast';

const modalRef = ref();

const model = reactive({
  loading: true,
  name: '',
  group: '',
  domain: '',
  groupOption: []
});

const column = [
  {
    column: 'name',
    name: '收录名称',
    width: '200px'
  },
  {
    column: 'domain',
    name: '收录地址'
  },
  {
    column: 'group',
    name: '收录分组',
    width: '200px'
  },
  {
    column: 'hrefCount',
    name: '收录热度',
    width: '120px',
    align: 'center'
  },
  {
    column: 'action',
    name: '操作',
    align: 'center',
    width: '180px'
  }
];

const table = reactive({
  count: 0,
  list: [],
  pageIndex: 1,
  pageSize: 15,
  loading: false
});

onMounted(async () => {
  try {
    getGroups();
    await getTableList();
  } finally {
    model.loading = false;
  }
});

const handleCurrentChange = index => {
  table.pageIndex = index;
};

const getTableList = async () => {
  try {
    table.loading = true;

    let res = await getList({
      name: model.name,
      group: model.group,
      domain: model.domain,
      pageIndex: table.pageIndex,
      pageSize: table.pageSize
    });
    if (res && res.resCode == 0) {
      table.count = res.data.count;
      table.list = res.data.list;
    }
  } finally {
    table.loading = false;
  }
};

const getGroups = async () => {
  try {
    let res = await getGroupOptions();
    if (res && res.resCode == 0) {
      model.groupOption = res.data.list;
    }
  } catch (error) {}
};

const $search = () => {
  table.pageIndex = 1;
  getTableList();
};

const $create = () => {
  //
  modalRef.value.show({}, -1);
};

const complete = (data, index) => {
  // 更新数据
  if (index == undefined || index == -1) {
    if (table.list.length == table.pageSize) {
      table.list.pop();
    }

    table.list.unshift(data);
    table.count++;
  } else {
    table.list[index] = data;
  }

  // 补充新的分组
  if (!model.groupOption.filter(x => x.value == data.group).length) {
    model.groupOption.push({
      value: data.group,
      name: data.group
    });
  }
};

const $update = (index, row) => {
  //
  modalRef.value.show(row, index);
};

const $delete = async (index, row) => {
  row.loading = true;

  try {
    //
    let res = await deleteSiteNavigation(row.id);
    if (res && res.resCode == 0) {
      toast.success('删除成功');

      if (table.list.length <= table.count) {
        table.list.splice(index, 1);
        table.count--;
        return;
      }

      if (table.length == 1) {
        if (table.pageIndex > 1) {
          table.pageIndex--;
        }
      }

      getTableList();
    }
  } finally {
    row.loading = false;
  }
};
</script>

<style lang="scss" scoped>
.domain-link {
  transition: all 0.3s;

  &:hover {
    font-size: 18px;
    color: var(--color-primary);
  }
}
</style>
