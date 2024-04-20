namespace MasaQSTemplate.AuthModule.Application.Events;

public record PermissionCreateCommand(PermissionCreateDto dto) : Command
{
    public Guid Result { get; set; }
}

public record PermissionUpdateCommand(PermissionUpdateDto dto) : Command
{
}

public record PermissionDeleteCommand(Guid id) : Command
{
}

