namespace MasaQSTemplate.Contracts.Auth.Dtos;

public class UserDto
{
    public Guid Id { get; set; }
    
    public string Account { get; set; }

    public string UserName { get; set; }

    public string Avatar { get; set; }

    public bool Gender { get; set; }

    public string IdCard { get; set; }

    public string PhoneNumber { get; set; }

    public string Email { get; set; }

    public string Address { get; set; }

    public string CompanyName { get; set; }

    public string Department { get; set; }

    public bool Enabled { get; set; }

    public DateTime CreationTime { get; set; }

    public List<RoleDto> Roles { get; set; } = new List<RoleDto>();

    public List<PermissionDto> Permissions { get; set; } = new List<PermissionDto>();
}

public class UserQueryDto : RequestPageBase
{
    public Guid? Id { get; set; }
    public string Filter { get; set; }
    public Guid? RoleId { get; set; }
    public string Account { get; set; }
    public string Password { get; set; }

    public bool IncludeRoles { get; set; }
    public bool IncludePermissions { get; set; }
}

public class UserAuthDto
{
    public string Account { get; set; }
    public string Password { get; set; }
}
public class UserTokenDto
{
    [JsonPropertyName("token_type")]
    public string TokenType { get; set; }
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; }
    [JsonPropertyName("refresh_token")]
    public string RefreshToken { get; set; }
    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }
}

public class UserCreateDto
{
    public string Account { get; set; }

    public string Password { get; set; }

    public string UserName { get; set; }

    public string Avatar { get; set; }

    public bool Gender { get; set; }

    public string IdCard { get; set; }

    public string PhoneNumber { get; set; }

    public string Email { get; set; }

    public string Address { get; set; }

    public string CompanyName { get; set; }

    public string Department { get; set; }

    public bool Enabled { get; set; }

    public List<Guid> RoleIds { get; set; } = new List<Guid>();

    public List<Guid> PermissionIds { get; set; } = new List<Guid>();
}

public class UserUpdateDto
{
    public Guid Id { get; set; }

    public string Account { get; set; }

    public string Password { get; set; }

    public string UserName { get; set; }

    public string Avatar { get; set; }

    public bool Gender { get; set; }

    public string IdCard { get; set; }

    public string PhoneNumber { get; set; }

    public string Email { get; set; }

    public string Address { get; set; }

    public string CompanyName { get; set; }

    public string Department { get; set; }

    public bool Enabled { get; set; }

    public List<Guid> RoleIds { get; set; } = new List<Guid>();

    public List<Guid> PermissionIds { get; set; } = new List<Guid>();
}
