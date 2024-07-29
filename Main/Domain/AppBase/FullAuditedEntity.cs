namespace Main.Domain.AppBase;

public class FullAuditedEntity<TKey> : AuditedEntity<TKey>, IFullAudited
{
    public long? DeleterUserId { get; set; }
    public DateTime? DeletionTime { get; set; }
    public bool IsDeleted { get; set; }
}

public class FullAuditedEntity : FullAuditedEntity<int>
{

}