namespace MasaQSTemplate.Contracts.Auth.Dtos;

public class RoleDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime CreationTime { get; set; }
    public List<PermissionDto> Permissions { get; set; } = new List<PermissionDto>();
}

public class RoleQueryDto : RequestPageBase
{
    public Guid? Id { get; set; }
    public string Filter { get; set; }
    public List<Guid> Ids { get; set; }

    public bool IncludePermissions { get; set; }
}

public class RoleCreateDto
{
    public string Name { get; set; }
    public string Description { get; set; }
    public List<Guid> PermissionIds { get; set; } = new List<Guid>();
}

public class RoleUpdateDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public List<Guid> PermissionIds { get; set; } = new List<Guid>();
}