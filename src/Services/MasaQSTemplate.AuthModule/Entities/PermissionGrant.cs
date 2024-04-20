using MasaQSTemplate.Contracts.Auth.Enums;

namespace MasaQSTemplate.AuthModule.Entities
{
    [SugarTable("PermissionGrants")]
    public class PermissionGrant : FullEntity<Guid, Guid>
    {
        public PermissionGrantTypes GrantType { get; set; }
        public Guid PermissionId { get; set; }
        public Guid RelationId { get; set; }
    }
}
