namespace MasaQSTemplate.LogModule.Application.Handlers
{
    public class LogCommandHandler
    {
        readonly IMapper _mapper;
        readonly LogDbContext _db;
        public LogCommandHandler(LogDbContext db, IMapper mapper)
        {
            _mapper = mapper;
            _db = db;
        }

        [EventHandler]
        public async Task CreateAsync(LogCreateCommand command)
        {
            await _db.Insertable(_mapper.Map<Log>(command.dto)).ExecuteCommandAsync();
        }
    }
}
