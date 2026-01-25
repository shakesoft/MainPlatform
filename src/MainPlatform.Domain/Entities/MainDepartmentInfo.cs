using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace MainPlatform.Entities;

public class MainDepartmentInfo:FullAuditedEntity<Guid>
{
    public string NameAr { get; set; }
    public string NameEn { get; set; }
    public string Code { get; set; }
    public Guid? ParentId { get; set; }
    public bool? HasItem { get; set; }

}
