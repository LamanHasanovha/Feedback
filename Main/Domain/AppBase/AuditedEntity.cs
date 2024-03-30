namespace Main.Domain.AppBase;

public class AuditedEntity<TKey> : CreationAuditedEntity<TKey>, IModificationAudited
{
    public long? LastModifierId { get; set; }
    public DateTime? LastModificationTime { get; set; }
}
