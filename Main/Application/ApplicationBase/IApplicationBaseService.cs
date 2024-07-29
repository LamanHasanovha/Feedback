using Main.Application.Mapper;

namespace Main.Application.ApplicationBase;

public interface IApplicationBaseService
{
    IMapper Mapper { get; }
}
