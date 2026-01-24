using Volo.Abp;
using Volo.Abp.MongoDB;

namespace OPAC.MongoDB;

public static class OPACMongoDbContextExtensions
{
    public static void ConfigureOPAC(
        this IMongoModelBuilder builder)
    {
        Check.NotNull(builder, nameof(builder));
    }
}
