namespace MasaQSTemplate.AuthModule.Application.Handlers
{
    public class RoleCommandHandler
    {
        readonly IMapper _mapper;
        readonly AuthDbContext _db;
        public RoleCommandHandler(AuthDbContext db, IMapper mapper)
        {
            _mapper = mapper;
            _db = db;
        }

        #region Role
        [EventHandler]
        public async Task CreateAsync(RoleCreateCommand command)
        {
            var role = _mapper.Map<Role>(command.dto);
            await _db.BeginTranAsync();
            await _db.Insertable(role).ExecuteCommandAsync();
            if (command.dto.PermissionIds.Any())
                await _db.Insertable(command.dto.PermissionIds.Select(x => new PermissionGrant() { GrantType = PermissionGrantTypes.Role, RelationId = role.Id, PermissionId = x }).ToList()).ExecuteCommandAsync();
            await _db.CommitTranAsync();
            command.Result = role.Id;
        }

        [EventHandler]
        public async Task UpdateAsync(RoleUpdateCommand command)
        {
            if (!await _db.Queryable<Role>().AnyAsync(x => x.Id == command.dto.Id))
                throw new UserFriendlyException("数据不存在");
            if (await _db.Queryable<Role>().AnyAsync(x => x.Id != command.dto.Id && x.Name == command.dto.Name))
                throw new UserFriendlyException("数据已存在");
            await _db.BeginTranAsync();
            await _db.Updateable(_mapper.Map<Role>(command.dto)).ExecuteCommandAsync();
            await _db.Updateable<PermissionGrant>().SetColumns(x => x.IsDeleted == true).Where(x => x.GrantType == PermissionGrantTypes.Role && x.RelationId == command.dto.Id).ExecuteCommandAsync();
            if (command.dto.PermissionIds.Any())
                await _db.Insertable(command.dto.PermissionIds.Select(x => new PermissionGrant() { GrantType = PermissionGrantTypes.Role, PermissionId = x, RelationId = command.dto.Id }).ToList()).ExecuteCommandAsync();
            await _db.CommitTranAsync();
        }

        [EventHandler]
        public async Task DeleteAsync(RoleDeleteCommand command)
        {
            await _db.BeginTranAsync();
            await _db.Updateable<Role>().SetColumns(x => x.IsDeleted == true).Where(x => x.Id == command.id).ExecuteCommandAsync();
            await _db.Updateable<UserRole>().SetColumns(x => x.IsDeleted == true).Where(x => x.RoleId == command.id).ExecuteCommandAsync();
            await _db.Updateable<PermissionGrant>().SetColumns(x => x.IsDeleted == true).Where(x => x.GrantType == PermissionGrantTypes.Role && x.RelationId == command.id).ExecuteCommandAsync();
            await _db.CommitTranAsync();
        }

        #endregion
    }
}
