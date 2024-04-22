# 基于masafx、ant-design-vue-pro，Web后台管理项目模板
### 后端：[MASA.Framework](https://github.com/masastack/MASA.Framework) 
### O/RM：[SqlSugar](https://github.com/DotNetNext/SqlSugar) 
### 前端：[vue-admin-project](https://github.com/vueComponent/ant-design-vue-pro)

### 环境要求：NET8

## 使用步骤
#### 1、安装模板
``` cmd
dotnet new install MasaQSTemplate
```

#### 2、创建项目，比如Demo
``` cmd
dotnet new qst -n Demo            
```

#### 3、net run或者(编译后)iis新建站点指向Demo.Web目录，运行查看
``` cmd
cd Demo\src\Services\Demo.Api
dotnet build
dotnet run       
```

#### 6、进入app
``` cmd  
cd Demo\app
yarn install
yarn serve
```

## 效果图
#### 用户
!["用户"](/imgs/user.png "用户")
!["创建用户"](/imgs/user.create.png "创建用户")

#### 角色
!["角色"](/imgs/role.png "角色")
!["创建角色"](/imgs/role.create.png "创建角色")

#### 权限
!["权限"](/imgs/permission.png "权限")