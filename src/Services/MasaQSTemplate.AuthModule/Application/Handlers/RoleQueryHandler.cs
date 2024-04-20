using Masa.BuildingBlocks.Dispatcher.Events;

namespace MasaQSTemplate.AuthModule.Application.Handlers
{
    public class RoleQueryHandler
    {
        readonly IMapper _mapper;
        readonly AuthDbContext _db;
        readonly IEventBus _eventBus;

        public RoleQueryHandler(AuthDbContext db, IMapper mapper, IEventBus eventBus)
        {
            _mapper = mapper;
            _db = db;
            _eventBus = eventBus;
        }

        #region Role
        [EventHandler]
        public async Task HandleAsync(RoleQuery @event)
        {
            RefAsync<int> total = 0;
            var query = _db.Queryable<Role>()
               .WhereIF(@event.dto.Id.HasValue, x => x.Id == @event.dto.Id.Value)
               .WhereIF(!string.IsNullOrWhiteSpace(@event.dto.Filter), x =>x.Name!=null&&x.Name.Contains(@event.dto.Filter))
               .WhereIF(@event.dto.Ids != null && @event.dto.Ids.Any(), x => @event.dto.Ids.Contains(x.Id))
               .OrderByDescending(x => x.CreationTime);
            var list = @event.dto.PageSize > 0 ? await query.ToPageListAsync(@event.dto.Page, @event.dto.PageSize, total) : await query.ToListAsync();
            @event.Result = new PaginatedListBase<RoleDto>()
            {
                Result = _mapper.Map<List<RoleDto>>(list),
                Total = @event.dto.PageSize > 0 ? total : list.Count,
                TotalPages = @event.dto.PageSize > 0 ? (int)Math.Ceiling((double)total / @event.dto.PageSize) : 0
            };
            if (@event.dto.IncludePermissions && list.Any())
            {
                var roleIds = list.Select(x => x.Id).ToList();
                var permissionGrants = await _db.Queryable<PermissionGrant>().Where(x => x.GrantType == PermissionGrantTypes.Role && roleIds.Contains(x.RelationId)).ToListAsync();
                var permissionQueryEvent = new PermissionQuery(new PermissionQueryDto() { Ids = permissionGrants.Select(x => x.PermissionId).ToList() });
                await _eventBus.PublishAsync(permissionQueryEvent);
                @event.Result.Result.ForEach(x =>
                {
                    var rolePermissionIds = permissionGrants.Where(p => p.RelationId == x.Id).Select(x => x.PermissionId);
                    x.Permissions = _mapper.Map<List<PermissionDto>>(permissionQueryEvent.Result.Result.Where(p => rolePermissionIds.Contains(p.Id)));
                });
            }
        }
        #endregion
    }

}