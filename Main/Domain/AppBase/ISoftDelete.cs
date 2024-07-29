namespace Main.Domain.AppBase;

public interface ISoftDelete
{
    bool IsDeleted { get; set; }
}
