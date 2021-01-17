using Microsoft.EntityFrameworkCore;
using System;

namespace EFCoreConnectorStore
{
    public class DbContextFactory<T> : IDbContextFactory where T : DbContext
    {
        private readonly DbContextOptions<T> _options;

        public DbContextFactory(DbContextOptions<T> options)
        {
            _options = options;
        }

        public DbContext GetDbContext()
        {
            return (T)Activator.CreateInstance(typeof(T), _options);
        }
    }
}
