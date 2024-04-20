using Masa.BuildingBlocks.Data;
using Masa.Utils.Security.Cryptography;
using MasaQSTemplate.BaseModule;
using Microsoft.Extensions.DependencyInjection;

namespace MasaQSTemplate.AuthModule
{
    [ConnectionStringName("Auth")]
    public class AuthDbContext : SqlSugarDbContext, IScopedDependency
    {
        public AuthDbContext(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override SqlSugarDbContext SeedData()
        {
            #region Role
            var adminRole = Queryable<Role>().First(x => x.Name.Equals("admin"));
            if (adminRole == null)
            {
                adminRole = new Role() { Name = "admin" };
                Insertable(adminRole).ExecuteCommand();
            }
            #endregion

            #region User
            var adminUser = Queryable<User>().First(x => x.Account.Equals("admin"));
            if (adminUser == null)
            {
                adminUser = new User() { Account = "admin", UserName = "admin", Password = MD5Utils.EncryptRepeat("123456"), Enabled = true };
                Insertable(adminUser).ExecuteCommand();
                var adminUserRole = new UserRole() { RoleId = adminRole.Id, UserId = adminUser.Id };
                Insertable(adminUserRole).ExecuteCommand();
            }
            #endregion

            #region Permission
            if (!Queryable<Permission>().Any())
            {
                var userPermission = new Permission() { Name = "用户管理", Code = "user" };
                Insertable(userPermission).ExecuteCommand();
                var userChildrenPermissions = new[] {
                    new Permission() {ParentId=userPermission.Id, Name = "查询", Code = "user.get" },
                    new Permission() {ParentId=userPermission.Id, Name = "新增", Code = "user.create" },
                    new Permission() {ParentId=userPermission.Id, Name = "编辑", Code = "user.update" },
                    new Permission() {ParentId=userPermission.Id, Name = "删除", Code = "user.delete" },
                };
                Insertable(userChildrenPermissions).ExecuteCommand();

                var rolePermission = new Permission() { Name = "角色管理", Code = "role" };
                Insertable(rolePermission).ExecuteCommand();
                var roleChildrenPermissions = new[] {
                    new Permission() {ParentId=rolePermission.Id, Name = "查询", Code = "role.get" },
                    new Permission() {ParentId=rolePermission.Id, Name = "新增", Code = "role.create" },
                    new Permission() {ParentId=rolePermission.Id, Name = "编辑", Code = "role.update" },
                    new Permission() {ParentId=rolePermission.Id, Name = "删除", Code = "role.delete" },
                };
                Insertable(roleChildrenPermissions).ExecuteCommand();

                var permissionPermission = new Permission() { Name = "权限管理", Code = "permission" };
                Insertable(permissionPermission).ExecuteCommand();
                var permissionChildrenPermissions = new[] {
                    new Permission() {ParentId=permissionPermission.Id, Name = "查询", Code = "permission.get" },
                    new Permission() {ParentId=permissionPermission.Id, Name = "新增", Code = "permission.create" },
                    new Permission() {ParentId=permissionPermission.Id, Name = "编辑", Code = "permission.update" },
                    new Permission() {ParentId=permissionPermission.Id, Name = "删除", Code = "permission.delete" },
                };
                Insertable(permissionChildrenPermissions).ExecuteCommand();
            }
            #endregion

            return this;
        }
    }
}
