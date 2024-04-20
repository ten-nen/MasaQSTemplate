using Masa.BuildingBlocks.Dispatcher.Events;
using Masa.Utils.Security.Cryptography;
using MasaQSTemplate.Contracts.Auth.Dtos;

namespace MasaQSTemplate.AuthModule.Application.Handlers
{
    public class UserQueryHandler
    {
        readonly IMapper _mapper;
        readonly AuthDbContext _db;
        readonly IEventBus _eventBus;

        public UserQueryHandler(AuthDbContext db, IMapper mapper, IEventBus eventBus)
        {
            _mapper = mapper;
            _db = db;
            _eventBus = eventBus;
        }

        #region User
        [EventHandler]
        public async Task HandleAsync(UserQuery @event)
        {
            if (!string.IsNullOrWhiteSpace(@event.dto.Password))
                @event.dto.Password = MD5Utils.EncryptRepeat(@event.dto.Password);

            List<Guid> userIds = null;
            if (@event.dto.RoleId.HasValue)
                userIds = await _db.Queryable<UserRole>()
                    .Where(x => x.RoleId == @event.dto.RoleId.Value)
                    .Select(x => x.UserId)
                    .ToListAsync();

            RefAsync<int> total = 0;
            var query = _db.Queryable<User>()
                .WhereIF(@event.dto.Id.HasValue, x => x.Id == @event.dto.Id.Value)
                .WhereIF(userIds != null, x => userIds.Contains(x.Id))
                .WhereIF(!string.IsNullOrEmpty(@event.dto.Filter), x => (x.UserName != null && x.UserName.Contains(@event.dto.Filter)) || (x.Account != null && x.Account.Contains(@event.dto.Filter)) || (x.PhoneNumber != null && x.PhoneNumber.Contains(@event.dto.Filter)))
                .WhereIF(!string.IsNullOrEmpty(@event.dto.Account), x => x.Account == @event.dto.Account)
                .WhereIF(!string.IsNullOrEmpty(@event.dto.Password), x => x.Password == @event.dto.Password)
                .OrderByDescending(x => x.CreationTime);
            var list = @event.dto.PageSize > 0 ? await query.ToPageListAsync(@event.dto.Page, @event.dto.PageSize, total) : await query.ToListAsync();
            @event.Result = new PaginatedListBase<UserDto>()
            {
                Result = _mapper.Map<List<UserDto>>(list),
                Total = @event.dto.PageSize > 0 ? total : list.Count,
                TotalPages = @event.dto.PageSize > 0 ? (int)Math.Ceiling((double)total / @event.dto.PageSize) : 0
            };
            List<UserRole> userRoles = null;
            var userids = list.Select(x => x.Id).Distinct();
            if ((@event.dto.IncludeRoles || @event.dto.IncludePermissions) && list.Any())
            {
                userRoles = await _db.Queryable<UserRole>().Where(x => userids.Contains(x.UserId)).ToListAsync();
                if (userRoles.Any())
                {
                    var roleIds = userRoles.Select(x => x.RoleId).ToList();
                    var roleQueryEvent = new RoleQuery(new RoleQueryDto() { Ids = roleIds });
                    await _eventBus.PublishAsync(roleQueryEvent);
                    @event.Result.Result.ForEach(x => x.Roles = roleQueryEvent.Result.Result.Where(r => userRoles.Any(ur => ur.UserId == x.Id && ur.RoleId == r.Id)).ToList());
                }
            }
            if (@event.dto.IncludePermissions && list.Any())
            {
                var userPermissions = await _db.Queryable<PermissionGrant>().Where(x => x.GrantType == PermissionGrantTypes.User && userids.Contains(x.RelationId)).ToListAsync();
                var rolePermissions = await _db.Queryable<PermissionGrant>().Where(x => x.GrantType == PermissionGrantTypes.Role && userRoles.Select(x => x.RoleId).ToList().Distinct().Contains(x.RelationId)).ToListAsync();
                userPermissions.AddRange(rolePermissions.Where(x => !userPermissions.Any(p => p.PermissionId == x.PermissionId)));
                if (userPermissions.Any())
                {
                    var permissionQueryEvent = new PermissionQuery(new PermissionQueryDto() { Ids = userPermissions.Select(x => x.PermissionId).Distinct().ToList() });
                    await _eventBus.PublishAsync(permissionQueryEvent);
                    @event.Result.Result.ForEach(x =>
                    {
                        var userPermissionIds = userPermissions.Where(p => p.GrantType == PermissionGrantTypes.User && p.RelationId == x.Id).Select(x => x.PermissionId).ToList();
                        var userRoleIds = userRoles.Where(r => r.UserId == x.Id).Select(x => x.RoleId).ToList();
                        userPermissionIds.AddRange(userPermissions.Where(p => p.GrantType == PermissionGrantTypes.Role && userRoleIds.Contains(p.RelationId)).Select(p => p.PermissionId));
                        x.Permissions = permissionQueryEvent.Result.Result.Where(p => userPermissionIds.Contains(p.Id)).ToList();
                    });
                }
            }
        }
        #endregion
    }

}