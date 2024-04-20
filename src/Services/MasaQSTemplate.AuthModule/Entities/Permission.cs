namespace MasaQSTemplate.AuthModule.Entities
{
    [SugarTable("Permissions")]
    public class Permission : FullEntity<Guid, Guid>
    {
        public string Name { get; set; }
        public string Code { get; set; }
        [SugarColumn(IsNullable = true)]
        public Guid? ParentId { get; set; }
        public int Order { get; set; }
        [SugarColumn(IsNullable = true)]
        public string Description { get; set; }
    }
}
