namespace Main.Domain.AppBase;

public class CreationAuditedEntity<TKey> : Entity<TKey>, ICreationAudited
{
    public long? CreatorUserId { get; set; }

    public DateTime CreationTime { get; set; }
}

public class CreationAuditedEntity : CreationAuditedEntity<int>
{

}