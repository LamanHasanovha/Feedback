namespace Main.Domain.AppBase;

public interface IModificationAudited : IHasModificationTime
{
    long? LastModifierId { get; set; }
}
