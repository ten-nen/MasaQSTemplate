<template>
  <page-header-wrapper>
    <a-card :bordered="false">
      <div class="table-page-search-wrapper">
        <a-form layout="inline">
          <a-row :gutter="48">
            <a-col :md="6" :sm="24">
              <a-form-item label="关键词">
                <a-input v-model="queryParam.filter" placeholder="请输入搜索关键词" />
              </a-form-item>
            </a-col>
            <a-col :md="4" :sm="24">
              <span class="table-page-search-submitButtons">
                <a-button type="primary" @click="$refs.table.refresh(true)">查询</a-button>
                <a-button style="margin-left: 8px" @click="() => (this.queryParam = {})">重置</a-button>
              </span>
            </a-col>
          </a-row>
        </a-form>
      </div>

      <div class="table-operator">
        <a-button type="primary" icon="plus" @click="handleAdd(null)" v-if="hasPermission('permission.create')">新建</a-button>
      </div>

      <s-table ref="table" size="default" rowKey="id" :columns="columns" :data="loadData" showPagination="auto">
        <span slot="serial" slot-scope="text, record, index">
          {{ index + 1 }}
        </span>
        <span slot="formatDate" slot-scope="text, record">
          {{ moment(record.creationTime).format('YYYY-MM-DD HH:mm') }}
        </span>
        <span slot="action" slot-scope="text, record">
          <template>
            <a @click="handleEdit(record)" v-if="hasPermission('permission.update')">编辑</a>
            <a-divider type="vertical" />
            <a @click="handleAdd(record.id)" v-if="!record.parentId&&hasPermission('permission.create')">新建下级</a>
            <a-divider type="vertical" v-if="!record.parentId" />
            <a-popconfirm title="确定删除吗?" ok-text="确定" cancel-text="取消" @confirm="handleDel(record)"  v-if="hasPermission('permission.delete')">
              <a style="color: red">删除</a>
            </a-popconfirm>
          </template>
        </span>
      </s-table>
      <a-drawer
        :title="info.id ? '编辑' : '新建'"
        :width="720"
        :visible="visible"
        :body-style="{ paddingBottom: '80px' }"
        @close="visible = false"
      >
        <a-form-model ref="form" :model="info" :rules="rules" :label-col="{ span: 4 }" :wrapper-col="{ span: 20 }">
          <a-form-model-item v-show="info.id" label="ID" prop="id">
            <a-input v-model="info.id" disabled />
          </a-form-model-item>
          <a-form-model-item label="名称" prop="name">
            <a-input v-model="info.name" placeholder="请输入名称" />
          </a-form-model-item>
          <a-form-model-item label="编码" prop="code">
            <a-input v-model="info.code" placeholder="请输入编码" />
          </a-form-model-item>
          <a-form-model-item label="排序" prop="order">
            <a-input-number v-model="info.order" placeholder="请输入序号" />
          </a-form-model-item>
          <a-form-model-item label="描述" prop="description">
            <a-textarea v-model="info.description" :auto-size="{ minRows: 3, maxRows: 5 }" placeholder="请输入描述" />
          </a-form-model-item>
        </a-form-model>
        <div class="drawer-footer">
          <a-button :style="{ marginRight: '8px' }" @click="visible = false"> 关闭 </a-button>
          <a-button type="primary" :loading="confirmLoading" @click="handleOk"> 保存 </a-button>
        </div>
      </a-drawer>
    </a-card>
  </page-header-wrapper>
</template>

<script>
import { mapGetters } from 'vuex'
import { STable, Ellipsis } from '@/components'
import { get, add, edit, del } from '@/api/permission'
import { deepClone } from '@/utils/util'

const columns = [
  {
    title: '#',
    scopedSlots: { customRender: 'serial' },
  },
  {
    title: '名称',
    dataIndex: 'name',
  },
  {
    title: '编码',
    dataIndex: 'code',
  },
  {
    title: '排序',
    dataIndex: 'order',
  },
  {
    title: '描述',
    dataIndex: 'description',
  },
  {
    title: '创建时间',
    dataIndex: 'creationTime',
    scopedSlots: { customRender: 'formatDate' },
  },
  {
    title: '操作',
    dataIndex: 'action',
    width: '150px',
    scopedSlots: { customRender: 'action' },
  },
]
const defaultInfo = {
  id: null,
  name: '',
  code: '',
  parentId: null,
  order: 0,
  description: '',
}
export default {
  name: 'permission',
  components: {
    STable,
    Ellipsis,
  },
  data() {
    this.columns = columns
    return {
      // create model
      visible: false,
      confirmLoading: false,
      // 查询参数
      queryParam: {
        includeChildren: true,
      },
      // 加载数据方法 必须为 Promise 对象
      loadData: (parameter) => {
        const requestParameters = Object.assign({}, parameter, this.queryParam)
        requestParameters.page = requestParameters.pageNo
        delete requestParameters.pageNo
        return get(requestParameters).then((res) => {
          return {
            pageSize: requestParameters.pageSize,
            pageNo: requestParameters.page,
            totalCount: res.total,
            totalPage: res.totalPages,
            data: res.result,
          }
        })
      },
      roles: [],
      info: this.$form.createForm(this, Object.assign({}, defaultInfo)),
      rules: {
        name: [{ required: true, message: '请输入名称', trigger: 'blur' }],
        code: [{ required: true, message: '请输入编码', trigger: 'blur' }],
      },
    }
  },
  created() {},
  computed: {
    ...mapGetters(['hasPermission']),
  },
  methods: {
    handleAdd(parentId) {
      this.info = Object.assign({}, defaultInfo)
      this.info.parentId = parentId
      this.visible = true
    },
    handleEdit(record) {
      this.info = deepClone(record)
      this.visible = true
    },
    handleOk() {
      this.confirmLoading = true
      this.$refs.form.validate((valid) => {
        if (valid) {
          if (this.info.id) {
            // 修改
            edit(this.info).then((res) => {
              this.visible = false
              this.confirmLoading = false
              // 刷新表格
              this.$refs.table.refresh()
              this.$message.info('修改成功')
            })
          } else {
            // 新增
            add(this.info).then((res) => {
              this.visible = false
              this.confirmLoading = false
              // 刷新表格
              this.$refs.table.refresh()
              this.$message.info('新增成功')
            })
          }
        } else {
          this.confirmLoading = false
          return false
        }
      })
    },
    handleDel(record) {
      del(record.id).then((res) => {
        this.$refs.table.refresh()
        this.$message.info('删除成功')
      })
    },
  },
}
</script>