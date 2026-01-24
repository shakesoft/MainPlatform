using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace OPAC.EntityFrameworkCore;

[ConnectionStringName(OPACDbProperties.ConnectionStringName)]
public interface IOPACDbContext : IEfCoreDbContext
{
    /* Add DbSet for each Aggregate Root here. Example:
     * DbSet<Question> Questions { get; }
     */
}
