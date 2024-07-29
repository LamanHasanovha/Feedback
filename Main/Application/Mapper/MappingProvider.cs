namespace Main.Application.Mapper;

public static class MappingProvider
{
    public static IMapper CreateDefaultInstance()
    {
        return new FeedbackMapper();
    }
}
