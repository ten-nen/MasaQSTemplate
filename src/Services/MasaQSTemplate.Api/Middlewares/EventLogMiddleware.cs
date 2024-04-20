namespace MasaQSTemplate.Api.Middlewares;

public class EventLogMiddleware<TEvent> : EventMiddleware<TEvent>
    where TEvent : notnull, IEvent
{
    private readonly ILogger<EventLogMiddleware<TEvent>> _logger;

    public EventLogMiddleware(ILogger<EventLogMiddleware<TEvent>> logger)
    {
        _logger = logger;
    }

    public override async Task HandleAsync(TEvent action, EventHandlerDelegate next)
    {
        var typeName = action.GetType().FullName;

        //_logger.LogInformation("【service】----- command {CommandType}", typeName);

        await next();
    }
}