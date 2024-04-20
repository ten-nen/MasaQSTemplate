namespace MasaQSTemplate.AuthModule.Application.Events;

#region User
public record UserQuery(UserQueryDto dto) : Query<PaginatedListBase<UserDto>>
{
    public override PaginatedListBase<UserDto> Result { get; set; } = default;
}

#endregion
