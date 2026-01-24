using Volo.Abp.Data;
using Volo.Abp.MongoDB;

namespace OPAC.MongoDB;

[ConnectionStringName(OPACDbProperties.ConnectionStringName)]
public class OPACMongoDbContext : AbpMongoDbContext, IOPACMongoDbContext
{
    /* Add mongo collections here. Example:
     * public IMongoCollection<Question> Questions => Collection<Question>();
     */

    protected override void CreateModel(IMongoModelBuilder modelBuilder)
    {
        base.CreateModel(modelBuilder);

        modelBuilder.ConfigureOPAC();
    }
}
