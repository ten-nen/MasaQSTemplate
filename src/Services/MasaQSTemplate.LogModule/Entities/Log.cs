using Masa.BuildingBlocks.Ddd.Domain.Entities;

namespace MasaQSTemplate.LogModule.Entities
{
    [SugarTable("Logs")]
    public class Log : Entity<Guid>
    {
        public string TraceId { get; set; }
        [SugarColumn(IsNullable = true)]
        public Guid? UserId { get; set; }
        [SugarColumn(IsNullable = true)]
        public string UserName { get; set; }
        [SugarColumn(IsNullable = true, Length = 512)]
        public string BrowserInfo { get; set; }
        [SugarColumn(IsNullable = true)]
        public string ClientIpAddress { get; set; }
        [SugarColumn(IsNullable = true, Length = 256)]
        public string Url { get; set; }
        [SugarColumn(IsNullable = true)]
        public string HttpMethod { get; set; }
        [SugarColumn(IsNullable = true)]
        public string ServiceName { get; set; }
        [SugarColumn(IsNullable = true)]
        public string MethodName { get; set; }
        [SugarColumn(IsNullable = true, ColumnDataType = "VARCHAR(MAX)")]
        public string HttpBody { get; set; }
        public DateTime ExecutionTime { get; set; }
        public int ExecutionDuration { get; set; }
        [SugarColumn(IsNullable = true, ColumnDataType = "VARCHAR(MAX)")]
        public string Exception { get; set; }
        public int HttpStatusCode { get; set; }
    }
}
