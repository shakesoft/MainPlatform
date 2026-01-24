using Microsoft.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace OPAC.EntityFrameworkCore;

public class OPACHttpApiHostMigrationsDbContext : AbpDbContext<OPACHttpApiHostMigrationsDbContext>
{
    public OPACHttpApiHostMigrationsDbContext(DbContextOptions<OPACHttpApiHostMigrationsDbContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ConfigureOPAC();
    }
}
