namespace MasaQSTemplate.AuthModule.Application.Events;

#region Permission
public record PermissionQuery(PermissionQueryDto dto) : Query<PaginatedListBase<PermissionDto>>
{
    public override PaginatedListBase<PermissionDto> Result { get; set; } = default;
}

#endregion
