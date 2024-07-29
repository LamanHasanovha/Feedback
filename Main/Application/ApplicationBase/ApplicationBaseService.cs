using Main.Application.Mapper;

namespace Main.Application.ApplicationBase;

public class ApplicationBaseService : IApplicationBaseService
{
    public IMapper Mapper { get; }

    public ApplicationBaseService()
    {
        Mapper = MappingProvider.CreateDefaultInstance();
    }

}
