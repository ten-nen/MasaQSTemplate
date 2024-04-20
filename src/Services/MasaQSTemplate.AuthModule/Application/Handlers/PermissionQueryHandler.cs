using MasaQSTemplate.Contracts.Auth.Dtos;

namespace MasaQSTemplate.AuthModule.Application.Handlers
{
    public class PermissionQueryHandler
    {
        readonly IMapper _mapper;
        readonly AuthDbContext _db;

        public PermissionQueryHandler(AuthDbContext db, IMapper mapper)
        {
            _mapper = mapper;
            _db = db;
        }

        #region Permission
        [EventHandler]
        public async Task HandleAsync(PermissionQuery @event)
        {
            RefAsync<int> total = 0;
            var query = _db.Queryable<Permission>()
                .WhereIF(@event.dto.Id.HasValue, x => x.Id == @event.dto.Id.Value)
                .WhereIF(@event.dto.Ids != null && @event.dto.Ids.Any(), x => @event.dto.Ids.Contains(x.Id))
                .WhereIF(!@event.dto.Id.HasValue && @event.dto.Ids == null, x => x.ParentId == null)
                .WhereIF(!string.IsNullOrWhiteSpace(@event.dto.Filter), x => (x.Name != null && x.Name.Contains(@event.dto.Filter)) || (x.Code != null && x.Code.Contains(@event.dto.Filter)))
                .OrderByDescending(x => x.Order)
                .OrderByDescending(x => x.CreationTime);
            var list = @event.dto.PageSize > 0 ? await query.ToPageListAsync(@event.dto.Page, @event.dto.PageSize, total) : await query.ToListAsync();
            @event.Result = new PaginatedListBase<PermissionDto>()
            {
                Result = _mapper.Map<List<PermissionDto>>(list),
                Total = @event.dto.PageSize > 0 ? total : 0,
                TotalPages = @event.dto.PageSize > 0 ? (int)Math.Ceiling((double)total / @event.dto.PageSize) : 0
            };
            if (@event.dto.IncludeChildren)
            {
                var ids = @event.Result.Result.Select(x => x.Id).Distinct().ToList();
                var childrens = await _db.Queryable<Permission>().Where(x => x.ParentId != null && ids.Contains(x.ParentId.Value)).ToListAsync();
                @event.Result.Result.ForEach(x => x.Children = _mapper.Map<List<PermissionDto>>(childrens.Where(c => c.ParentId == x.Id)));
            }
        }
        #endregion
    }

}