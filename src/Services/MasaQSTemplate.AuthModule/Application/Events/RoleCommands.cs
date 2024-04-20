namespace MasaQSTemplate.AuthModule.Application.Events;

#region Role
public record RoleCreateCommand(RoleCreateDto dto) : Command
{
    public Guid Result { get; set; }
}

public record RoleUpdateCommand(RoleUpdateDto dto) : Command
{
}

public record RoleDeleteCommand(Guid id) : Command
{
}

#endregion

