namespace Main.Domain.AppBase;

public interface IHasDeletionTime : ISoftDelete
{
    DateTime? DeletionTime { get; set; }
}
