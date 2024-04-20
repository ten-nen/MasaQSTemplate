namespace MasaQSTemplate.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PermissionController : ControllerBase
    {
        private readonly IEventBus _eventBus;
        public PermissionController(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        #region Permission
        [HttpGet]
        public async Task<PaginatedListBase<PermissionDto>> GetAsync([FromQuery] PermissionQueryDto dto)
        {
            var @event = new PermissionQuery(dto);
            await _eventBus.PublishAsync(@event);
            return @event.Result;
        }

        [HttpPost]
        public async Task<Guid> CreateAsync(PermissionCreateDto dto)
        {
            var @event = new PermissionCreateCommand(dto);
            await _eventBus.PublishAsync(@event );
            return @event.Result;
        }

        [HttpPut]
        public async Task UpdateAsync(PermissionUpdateDto dto) => await _eventBus.PublishAsync(new PermissionUpdateCommand(dto));

        [HttpDelete]
        public async Task DeleteAsync(Guid id) => await _eventBus.PublishAsync(new PermissionDeleteCommand(id));
        #endregion
    }
}
