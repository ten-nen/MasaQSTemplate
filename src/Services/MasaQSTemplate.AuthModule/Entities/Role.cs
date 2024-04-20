namespace MasaQSTemplate.AuthModule.Entities
{
    [SugarTable("Roles")]
    public class Role : FullEntity<Guid, Guid>
    {
        public string Name { get; set; }
        [SugarColumn(Length = 250, IsNullable = true)]
        public string Description { get; set; }
    }
}
