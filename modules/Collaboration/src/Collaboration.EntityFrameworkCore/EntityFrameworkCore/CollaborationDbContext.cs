using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace Collaboration.EntityFrameworkCore;

[ConnectionStringName(CollaborationDbProperties.ConnectionStringName)]
public class CollaborationDbContext : AbpDbContext<CollaborationDbContext>, ICollaborationDbContext
{
    /* Add DbSet for each Aggregate Root here. Example:
     * public DbSet<Question> Questions { get; set; }
     */

    public CollaborationDbContext(DbContextOptions<CollaborationDbContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ConfigureCollaboration();
    }
}
