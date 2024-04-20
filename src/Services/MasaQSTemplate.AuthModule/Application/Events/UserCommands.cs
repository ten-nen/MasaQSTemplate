namespace MasaQSTemplate.AuthModule.Application.Events;

public record UserCreateCommand(UserCreateDto dto) : Command
{
    public Guid Result { get; set; }
}

public record UserUpdateCommand(UserUpdateDto dto) : Command
{
}

public record UserDeleteCommand(Guid id) : Command
{
}

