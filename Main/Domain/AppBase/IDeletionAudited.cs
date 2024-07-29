namespace Main.Domain.AppBase;

public interface IDeletionAudited : IHasDeletionTime, ISoftDelete
{
    long? DeleterUserId { get; set; }
}
