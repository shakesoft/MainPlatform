using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace Collaboration.EntityFrameworkCore;

[ConnectionStringName(CollaborationDbProperties.ConnectionStringName)]
public interface ICollaborationDbContext : IEfCoreDbContext
{
    /* Add DbSet for each Aggregate Root here. Example:
     * DbSet<Question> Questions { get; }
     */
}
