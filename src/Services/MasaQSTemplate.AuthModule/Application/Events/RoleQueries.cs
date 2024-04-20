namespace MasaQSTemplate.AuthModule.Application.Events;

#region Role
public record RoleQuery(RoleQueryDto dto) : Query<PaginatedListBase<RoleDto>>
{
    public override PaginatedListBase<RoleDto> Result { get; set; } = default;
}

#endregion
