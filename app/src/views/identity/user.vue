<template>
  <page-header-wrapper>
    <a-card :bordered="false">
      <div class="table-page-search-wrapper">
        <a-form layout="inline">
          <a-row :gutter="48">
            <a-col :md="6" :sm="24">
              <a-form-item label="关键词">
                <a-input v-model="queryParam.filter" placeholder="" />
              </a-form-item>
            </a-col>
            <a-col :md="6" :sm="24">
              <a-form-item label="角色">
                <a-select v-model="queryParam.roleId" placeholder="请选择" default-value="" allowClear>
                  <a-select-option value="">全部</a-select-option>
                  <a-select-option :value="role.id" v-for="role in roles" :key="role.id">{{
                    role.name
                  }}</a-select-option>
                </a-select>
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
        <a-button type="primary" icon="plus" @click="handleAdd" v-if="hasPermission('user.create')">新建</a-button>
      </div>

      <s-table ref="table" size="default" rowKey="id" :columns="columns" :data="loadData" showPagination="auto">
        <span slot="serial" slot-scope="text, record, index">
          {{ index + 1 }}
        </span>
        <span slot="formatEnabled" slot-scope="text, record">
          {{ record.enabled ? '启用' : '禁用' }}
        </span>
        <span slot="formatDate" slot-scope="text, record">
          {{ moment(record.creationTime).format('YYYY-MM-DD HH:mm') }}
        </span>
        <span slot="formatRoles" slot-scope="text, record">
          {{ record.roles.map((x) => x.name).join(',') }}
        </span>
        <span slot="action" slot-scope="text, record">
          <template>
            <a @click="handleEdit(record)" v-if="hasPermission('user.update')">编辑</a>
            <a-divider type="vertical" />
            <a-popconfirm
              title="确定删除吗?"
              ok-text="确定"
              cancel-text="取消"
              @confirm="handleDel(record)"
              v-if="record.account !== 'admin' && hasPermission('user.delete')"
            >
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
          <a-form-model-item label="账号" prop="account">
            <a-input v-model="info.account" placeholder="请输入账号" />
          </a-form-model-item>
          <a-form-model-item label="密码" prop="password">
            <a-input v-model="info.password" placeholder="请输入密码" />
          </a-form-model-item>
          <a-form-model-item label="姓名" prop="userName">
            <a-input v-model="info.userName" placeholder="请输入姓名" />
          </a-form-model-item>
          <a-form-model-item label="手机号" prop="phoneNumber">
            <a-input v-model="info.phoneNumber" placeholder="请输入手机号" />
          </a-form-model-item>
          <a-form-model-item label="邮箱" prop="email">
            <a-input v-model="info.email" placeholder="请输入邮箱" />
          </a-form-model-item>
          <a-form-model-item label="身份证号" prop="idCard">
            <a-input v-model="info.idCard" placeholder="请输入身份证号" />
          </a-form-model-item>
          <a-form-model-item label="性别" prop="gender">
            <a-radio-group v-model="info.gender">
              <a-radio :value="false"> 女 </a-radio>
              <a-radio :value="true"> 男 </a-radio>
            </a-radio-group>
          </a-form-model-item>
          <a-form-model-item label="公司" prop="companyName">
            <a-input v-model="info.companyName" placeholder="请输入公司" />
          </a-form-model-item>
          <a-form-model-item label="部门" prop="department">
            <a-input v-model="info.department" placeholder="请输入部门" />
          </a-form-model-item>
          <a-form-model-item label="地址" prop="address">
            <a-textarea v-model="info.address" :auto-size="{ minRows: 3, maxRows: 5 }" placeholder="请输入地址" />
          </a-form-model-item>
          <a-form-model-item label="角色" v-if="info.account !== 'admin'">
            <a-select
              mode="tags"
              v-model="roleIds"
              allowClear
              style="width: 100%"
              placeholder="请选择角色"
              @change="(keys) => (roleIds = keys)"
            >
              <a-select-option v-for="role in roles" :key="role.id">{{ role.name }}</a-select-option>
            </a-select>
          </a-form-model-item>
          <a-form-model-item label="是否启用" prop="enabled">
            <a-radio-group v-model="info.enabled">
              <a-radio-button :value="true"> 启用 </a-radio-button>
              <a-radio-button :value="false"> 禁用 </a-radio-button>
            </a-radio-group>
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
import { get, add, edit, del } from '@/api/user'
import { get as getRoles } from '@/api/role'
import { deepClone } from '@/utils/util'

const columns = [
  {
    title: '#',
    scopedSlots: { customRender: 'serial' },
  },
  {
    title: '姓名',
    dataIndex: 'userName',
  },
  {
    title: '账号',
    dataIndex: 'account',
  },
  {
    title: '角色',
    dataIndex: 'roles',
    scopedSlots: { customRender: 'formatRoles' },
  },
  {
    title: '头像',
    dataIndex: 'avatar',
  },
  {
    title: '手机号',
    dataIndex: 'phoneNumber',
  },
  {
    title: '邮箱',
    dataIndex: 'email',
  },
  {
    title: '身份证号',
    dataIndex: 'idCard',
  },
  {
    title: '性别',
    dataIndex: 'gender',
  },
  {
    title: '公司',
    dataIndex: 'companyName',
  },
  {
    title: '部门',
    dataIndex: 'department',
  },
  {
    title: '地址',
    dataIndex: 'address',
  },
  {
    title: '是否启用',
    dataIndex: 'enabled',
    scopedSlots: { customRender: 'formatEnabled' },
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
const defaultEmptyPassword = '*********'
const defaultInfo = {
  id: null,
  userName: '',
  account: '',
  password: '',
  avatar: '',
  phoneNumber: '',
  email: '',
  idCard: '',
  gender: false,
  companyName: '',
  department: '',
  address: '',
  enabled: true,
  roles: [],
}
export default {
  name: 'user',
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
        includeRoles: true,
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
      roleIds: [],
      rules: {
        account: [
          { required: true, message: '请输入账号', trigger: 'blur' },
          { min: 4, max: 10, message: '长度需 4 ~ 10 位', trigger: 'blur' },
        ],
        password: [
          { required: true, message: '请输入密码', trigger: 'blur' },
          { min: 6, message: '最少6个字符', trigger: 'blur' },
        ],
        userName: [
          { required: true, message: '请输入姓名', trigger: 'blur' },
          { max: 10, message: '最多10个字符', trigger: 'blur' },
        ],
      },
    }
  },
  created() {},
  computed: {
    ...mapGetters(['hasPermission']),
  },
  mounted() {
    getRoles({}).then((res) => {
      this.roles = res.result
    })
  },
  methods: {
    handleAdd() {
      this.info = Object.assign({}, defaultInfo)
      this.roleIds = []
      this.visible = true
    },
    handleEdit(record) {
      this.info = deepClone(record)
      this.info.password = defaultEmptyPassword
      this.roleIds = this.info.roles.map((x) => x.id)
      this.visible = true
    },
    handleOk() {
      this.confirmLoading = true
      this.$refs.form.validate((valid) => {
        if (valid) {
          this.info.roleIds = this.roleIds
          if (this.info.id) {
            // 修改
            if (this.info.password === defaultEmptyPassword) {
              this.info.password = ''
            }

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