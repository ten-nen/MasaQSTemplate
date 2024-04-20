namespace MasaQSTemplate.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IEventBus _eventBus;
        public RoleController(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        #region Role
        [HttpGet]
        public async Task<PaginatedListBase<RoleDto>> GetAsync([FromQuery] RoleQueryDto dto)
        {
            var @event = new RoleQuery(dto);
            await _eventBus.PublishAsync(@event);
            return @event.Result;
        }

        [HttpPost]
        public async Task<Guid> CreateAsync(RoleCreateDto dto)
        {
            var @event = new RoleCreateCommand(dto);
            await _eventBus.PublishAsync(@event);
            return @event.Result;
        }

        [HttpPut]
        public async Task UpdateAsync(RoleUpdateDto dto) => await _eventBus.PublishAsync(new RoleUpdateCommand(dto));

        [HttpDelete]
        public async Task DeleteAsync(Guid id) => await _eventBus.PublishAsync(new RoleDeleteCommand(id));
        #endregion
    }
}
