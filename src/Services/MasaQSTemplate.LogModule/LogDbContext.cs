using Masa.BuildingBlocks.Data;
using MasaQSTemplate.BaseModule;
using Microsoft.Extensions.DependencyInjection;

namespace MasaQSTemplate.LogModule
{
    [ConnectionStringName("Log")]
    public class LogDbContext : SqlSugarDbContext, IScopedDependency
    {
        public LogDbContext(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}
