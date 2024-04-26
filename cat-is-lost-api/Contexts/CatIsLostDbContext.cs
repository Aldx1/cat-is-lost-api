using cat_is_lost_api.Interfaces;
using cat_is_lost_api.Models;
using Microsoft.EntityFrameworkCore;

namespace cat_is_lost_api.Contexts
{
    public class CatIsLostDbContext: DbContext, IDbContext
    {
        public CatIsLostDbContext(DbContextOptions<CatIsLostDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Post> Posts { get; set; } = null!;

        /// <summary>
        /// Saves the changes with the DbContext parent
        /// </summary>
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await base.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Saves the changes with the DbContext parent
        /// </summary>
        public override int SaveChanges(bool acceptAllChangesOnSuccess = true)
        {
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }
    }
}
