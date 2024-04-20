namespace MasaQSTemplate.Contracts.Auth.Dtos;

public class PermissionDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Code { get; set; }
    public Guid? ParentId { get; set; }
    public int Order { get; set; }
    public string Description { get; set; }
    public DateTime CreationTime { get; set; }

    public List<PermissionDto> Children { get; set; }
}

public class PermissionQueryDto : RequestPageBase
{
    public Guid? Id { get; set; }
    public string Filter { get; set; }
    public List<Guid> Ids { get; set; }

    public bool IncludeChildren { get; set; }
}

public class PermissionCreateDto
{
    public string Name { get; set; }
    public string Code { get; set; }
    public Guid? ParentId { get; set; }
    public int Order { get; set; }
    public string Description { get; set; }
}

public class PermissionUpdateDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Code { get; set; }
    public Guid? ParentId { get; set; }
    public int Order { get; set; }
    public string Description { get; set; }
}
