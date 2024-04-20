namespace MasaQSTemplate.AuthModule.Application.Handlers
{
    public class PermissionCommandHandler
    {
        readonly IMapper _mapper;
        readonly AuthDbContext _db;
        public PermissionCommandHandler(AuthDbContext db, IMapper mapper)
        {
            _mapper = mapper;
            _db = db;
        }

        [EventHandler]
        public async Task CreateAsync(PermissionCreateCommand command)
        {
            var permission = _mapper.Map<Permission>(command.dto);
            await _db.Insertable(permission).ExecuteCommandAsync();
            command.Result = permission.Id;
        }

        [EventHandler]
        public async Task UpdateAsync(PermissionUpdateCommand command)
        {
            if (!await _db.Queryable<Permission>().AnyAsync(x => x.Id == command.dto.Id))
                throw new UserFriendlyException("数据不存在");
            await _db.Updateable(_mapper.Map<Permission>(command.dto)).ExecuteCommandAsync();
        }

        [EventHandler]
        public async Task DeleteAsync(PermissionDeleteCommand command)
        {
            await _db.Updateable<Permission>().SetColumns(x => x.IsDeleted == true).Where(x => x.Id == command.id || x.ParentId == command.id).ExecuteCommandAsync();
        }
    }
}
