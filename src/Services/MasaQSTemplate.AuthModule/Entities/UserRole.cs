namespace MasaQSTemplate.AuthModule.Entities
{
    [SugarTable("UserRoles")]
    public class UserRole : FullEntity<Guid, Guid>
    {
        public Guid UserId { get; set; }
        public Guid RoleId { get; set; }
    }
}
