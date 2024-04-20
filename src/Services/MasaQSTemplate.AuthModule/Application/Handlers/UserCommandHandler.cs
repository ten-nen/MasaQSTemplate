using Masa.Utils.Security.Cryptography;

namespace MasaQSTemplate.AuthModule.Application.Handlers
{
    public class UserCommandHandler
    {
        readonly IMapper _mapper;
        readonly AuthDbContext _db;
        public UserCommandHandler(AuthDbContext db, IMapper mapper)
        {
            _mapper = mapper;
            _db = db;
        }

        [EventHandler]
        public async Task CreateAsync(UserCreateCommand command)
        {
            var user = _mapper.Map<User>(command.dto);
            user.Password = MD5Utils.EncryptRepeat(command.dto.Password);
            await _db.BeginTranAsync();
            await _db.Insertable(user).ExecuteCommandAsync();
            if (command.dto.RoleIds.Any())
                await _db.Insertable(command.dto.RoleIds.Select(x => new UserRole() { RoleId = x, UserId = user.Id }).ToList()).ExecuteCommandAsync();
            if (command.dto.PermissionIds.Any())
                await _db.Insertable(command.dto.PermissionIds.Select(x => new PermissionGrant() { GrantType = PermissionGrantTypes.User, RelationId = user.Id }).ToList()).ExecuteCommandAsync();
            await _db.CommitTranAsync();
            command.Result = user.Id;
        }

        [EventHandler]
        public async Task UpdateAsync(UserUpdateCommand command)
        {
            if (await _db.Queryable<User>().AnyAsync(x => x.Id != command.dto.Id && x.Account == command.dto.Account))
                throw new UserFriendlyException("数据已存在");
            var user = await _db.Queryable<User>().FirstAsync(x => x.Id == command.dto.Id);
            if (user == null)
                throw new UserFriendlyException("数据不存在");
            command.dto.Password = string.IsNullOrWhiteSpace(command.dto.Password) ? user.Password : MD5Utils.EncryptRepeat(command.dto.Password);
            await _db.BeginTranAsync();
            await _db.Updateable(_mapper.Map<User>(command.dto)).ExecuteCommandAsync();

            await _db.Updateable<UserRole>().SetColumns(x => x.IsDeleted == true).Where(x => x.UserId == command.dto.Id).ExecuteCommandAsync();
            await _db.Insertable(command.dto.RoleIds.Select(x => new UserRole() { RoleId = x, UserId = command.dto.Id }).ToList()).ExecuteCommandAsync();

            await _db.Updateable<PermissionGrant>().SetColumns(x => x.IsDeleted == true).Where(x => x.GrantType == PermissionGrantTypes.Role && x.RelationId == command.dto.Id).ExecuteCommandAsync();
            await _db.Insertable(command.dto.RoleIds.Select(x => new PermissionGrant() { GrantType = PermissionGrantTypes.User, PermissionId = x, RelationId = command.dto.Id }).ToList()).ExecuteCommandAsync();
            await _db.CommitTranAsync();
        }

        [EventHandler]
        public async Task DeleteAsync(UserDeleteCommand command)
        {
            await _db.BeginTranAsync();
            await _db.Updateable<User>().SetColumns(x => x.IsDeleted == true).Where(x => x.Id == command.id).ExecuteCommandAsync();
            await _db.Updateable<UserRole>().SetColumns(x => x.IsDeleted == true).Where(x => x.UserId == command.id).ExecuteCommandAsync();
            await _db.Updateable<PermissionGrant>().SetColumns(x => x.IsDeleted == true).Where(x => x.GrantType == PermissionGrantTypes.User && x.RelationId == command.id).ExecuteCommandAsync();
            await _db.CommitTranAsync();
        }
    }
}
