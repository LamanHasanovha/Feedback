namespace Main.Domain.AppBase;

public interface ICreationAudited : IHasCreationTime
{
    long? CreatorUserId { get; set; }
}
