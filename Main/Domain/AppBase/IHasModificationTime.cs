namespace Main.Domain.AppBase;

public interface IHasModificationTime
{
    DateTime? LastModificationTime { get; set; }
}
