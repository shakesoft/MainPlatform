using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace OPAC.EntityFrameworkCore;

[ConnectionStringName(OPACDbProperties.ConnectionStringName)]
public class OPACDbContext : AbpDbContext<OPACDbContext>, IOPACDbContext
{
    /* Add DbSet for each Aggregate Root here. Example:
     * public DbSet<Question> Questions { get; set; }
     */

    public OPACDbContext(DbContextOptions<OPACDbContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ConfigureOPAC();
    }
}
