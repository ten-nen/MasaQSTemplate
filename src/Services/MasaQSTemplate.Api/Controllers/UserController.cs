using Masa.Utils.Security.Cryptography;
using Masa.Utils.Security.Token;
using System.Security.Claims;

namespace MasaQSTemplate.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IEventBus _eventBus;
        private readonly IMapper _mapper;
        private readonly IUserContext _userContext;
        public UserController(IEventBus eventBus, IMapper mapper, IUserContext userContext)
        {
            _eventBus = eventBus;
            _mapper = mapper;
            _userContext = userContext;
        }
        #region User
        [AllowAnonymous]
        [Route("Auth")]
        [HttpPost]
        public async Task<UserTokenDto> AuthAsync(UserAuthDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Account) || string.IsNullOrWhiteSpace(dto.Password))
                throw new UserFriendlyException("账号密码不能为空");
            var queryDto = _mapper.Map<UserQueryDto>(dto);
            queryDto.IncludeRoles = true;
            queryDto.IncludePermissions = true;
            var @event = new UserQuery(queryDto);
            await _eventBus.PublishAsync(@event);
            var user = @event.Result.Result.FirstOrDefault();
            if (user == null)
                throw new UserFriendlyException("账号密码错误");
            if (!user.Enabled)
                throw new UserFriendlyException("未启用");
            var claims = new List<Claim> {
                    new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new(ClaimTypes.Name, user.UserName??""),
                    new(ClaimTypes.Role, JsonSerializer.Serialize(user.Roles.Select(x=>x.Name))),
                };
            //claims.AddRange(user.Roles.Select(x => new Claim(ClaimTypes.Role, x.Name)));
            var tokenDto = new UserTokenDto();
            tokenDto.TokenType = "Bearer";
            tokenDto.ExpiresIn = 60 * 60 * 24 * 7;
            tokenDto.AccessToken = JwtUtils.CreateToken(claims.ToArray(), TimeSpan.FromSeconds(tokenDto.ExpiresIn));
            tokenDto.RefreshToken = MD5Utils.EncryptRepeat(tokenDto.AccessToken, encryptTimes: 3);
            return tokenDto;
        }

        [Authorize]
        [Route("Current")]
        [HttpGet]
        public async Task<UserDto> GetCurrentAsync()
        {
            var @event = new UserQuery(new UserQueryDto() { Id = _userContext.GetUserId<Guid>(), IncludeRoles = true, IncludePermissions = true });
            await _eventBus.PublishAsync(@event);
            return @event.Result.Result.FirstOrDefault();
        }

        [HttpGet]
        public async Task<PaginatedListBase<UserDto>> GetAsync([FromQuery] UserQueryDto dto)
        {
            var @event = new UserQuery(dto);
            await _eventBus.PublishAsync(@event);
            return @event.Result;
        }

        [HttpPost]
        public async Task<Guid> CreateAsync(UserCreateDto dto)
        {
            var @event = new UserCreateCommand(dto);
            await _eventBus.PublishAsync(@event);
            return @event.Result;
        }

        [HttpPut]
        public async Task UpdateAsync(UserUpdateDto dto) => await _eventBus.PublishAsync(new UserUpdateCommand(dto));

        [HttpDelete]
        public async Task DeleteAsync(Guid id) => await _eventBus.PublishAsync(new UserDeleteCommand(id));

        #endregion
    }
}
