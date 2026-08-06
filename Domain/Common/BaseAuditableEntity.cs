namespace Domain.Common;

public abstract class BaseAuditableEntity<TU> : BaseEntity
{
    public string CreatedById { get; set; }
    public DateTime DateCreated { get; set; }

    public string LastModifiedById { get; set; }
    public DateTime? DateLastModified { get; set; }
}