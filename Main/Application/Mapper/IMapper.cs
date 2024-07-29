namespace Main.Application.Mapper;

public interface IMapper
{
    TTarget Map<TCurrent, TTarget>(TCurrent source);

    List<TTarget> Map<TCurrent, TTarget>(List<TCurrent> source);
}
