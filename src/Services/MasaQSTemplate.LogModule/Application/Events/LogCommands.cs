namespace MasaQSTemplate.LogModule.Application.Events;

public record LogCreateCommand(LogCreateDto dto) : Command
{
}

