using Masa.BuildingBlocks.Authentication.Identity;
using Masa.BuildingBlocks.Data;
using Masa.BuildingBlocks.Ddd.Domain.Entities;
using Masa.BuildingBlocks.Ddd.Domain.Entities.Auditing;
using Masa.BuildingBlocks.Ddd.Domain.Entities.Full;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SqlSugar;
using System.Reflection;
using System.Text;

namespace MasaQSTemplate.BaseModule
{
    public abstract class SqlSugarDbContext : SqlSugarClient
    {
        protected const DbType DatabaseType = DbType.SqlServer;
        protected readonly ILogger<SqlSugarDbContext> Logger;
        protected readonly IConfiguration Configuration;
        protected readonly IHostEnvironment Environment;
        protected SqlSugarDbContext(IServiceProvider serviceProvider) : base(new ConnectionConfig()
        {
            DbType = DatabaseType,
            ConfigureExternalServices = new ConfigureExternalServices()
            {
                EntityService = (t, column) =>
                {
                    if (column.PropertyName.ToLower() == nameof(IEntity<Guid>.Id).ToLower()) //是id的设为主键
                    {
                        column.IsPrimarykey = true;
                        if (column.PropertyInfo.PropertyType == typeof(int) || column.PropertyInfo.PropertyType == typeof(long)) //是id并且是int的是自增
                        {
                            column.IsIdentity = true;
                        }
                    }
                    else if (column.PropertyName.ToLower() == nameof(IFullEntity<Guid>.CreationTime).ToLower() || column.PropertyName.ToLower() == nameof(IFullEntity<Guid>.Creator).ToLower())
                    {
                        column.IsOnlyIgnoreUpdate = true;
                    }
                }
            }
        })
        {
            Configuration = serviceProvider.GetRequiredService<IConfiguration>();
            Logger = serviceProvider.GetRequiredService<ILogger<SqlSugarDbContext>>();
            Environment = serviceProvider.GetRequiredService<IHostEnvironment>();
            var connectionName = this.GetType().GetCustomAttribute<ConnectionStringNameAttribute>()?.Name;
            CurrentConnectionConfig.ConnectionString = Configuration.GetConnectionString(connectionName);
            //Aop.OnLogExecuting = (sql, pars) =>
            //{
            //};
            Aop.OnLogExecuted = (sql, pars) =>
            {
                var log = new StringBuilder($"【sql.executed】sql：{UtilMethods.GetSqlString(DatabaseType, sql, pars)}");
                var seconds = 5;
                if (Ado.SqlExecutionTime.TotalSeconds > seconds)
                {
                    log.Append($"\t【sql.executed.slow】文件：${Ado.SqlStackTrace.FirstFileName}|行号：${Ado.SqlStackTrace.FirstLine}|方法：{Ado.SqlStackTrace.FirstMethodName}|大于{seconds}秒");
                }
                Logger.LogInformation(log.ToString());
            };
            Aop.OnError = ex =>
            {
                Logger.LogInformation($"【sql.error】exception：{ex}");
            };
            Aop.DataExecuting = (oldValue, entityInfo) =>
            {
                var userContext = serviceProvider.GetService<IUserContext>();
                var userid = userContext?.GetUserId<Guid>();
                if (entityInfo.OperationType == DataFilterType.InsertByObject)
                {
                    if (entityInfo.PropertyName == nameof(IAuditEntity<Guid>.Creator))
                    {
                        entityInfo.SetValue(userid);
                    }
                    else if (entityInfo.PropertyName == nameof(IAuditEntity<Guid>.CreationTime))
                    {
                        entityInfo.SetValue(DateTime.Now);
                    }
                }
                if (entityInfo.PropertyName == nameof(IAuditEntity<Guid>.Modifier))
                {
                    entityInfo.SetValue(userid);
                }
                else if (entityInfo.PropertyName == nameof(IAuditEntity<Guid>.ModificationTime))
                {
                    entityInfo.SetValue(DateTime.Now);
                }
            };

            //软删除过滤
            QueryFilter.AddTableFilter<ISoftDelete>(x => x.IsDeleted == false);
        }

        public SqlSugarDbContext Init(bool createDatabase = true, bool createTables = true, bool seedData = true)
        {
            if (createDatabase)
                DbMaintenance.CreateDatabase();
            if (createTables)
            {
                var type = this.GetType();
                var entityTypes = type.Assembly.GetTypes().Where(x => x.GetCustomAttributes<SugarTable>().Any());
                CodeFirst.SetStringDefaultLength(50).InitTables(entityTypes.ToArray());
            }
            if (seedData)
            {
                SeedData();
            }
            return this;
        }

        public virtual SqlSugarDbContext SeedData()
        {
            return this;
        }
    }
}
