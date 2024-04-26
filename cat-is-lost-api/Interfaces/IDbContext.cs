using cat_is_lost_api.Models;
using Microsoft.EntityFrameworkCore;

namespace cat_is_lost_api.Interfaces
{
    public interface IDbContext
    {
        DbSet<User> Users { get; }

        DbSet<Post> Posts { get; }

        /// <summary>
        /// Saves the changes with the DbContext parent
        /// </summary>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Saves the changes with the DbContext parent
        /// </summary>
        int SaveChanges(bool acceptAllChangesOnSuccess = true);
    }
}
