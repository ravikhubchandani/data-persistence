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

        public DbContextFactory(DatabaseEnums dbType, string connectionString)
        {
            var optBuilder = new DbContextOptionsBuilder<T>();
            switch(dbType)
            {
                case DatabaseEnums.SQLITE:
                    optBuilder.UseSqlite(connectionString);
                    break;
                case DatabaseEnums.SQLSERVER:
                    optBuilder.UseSqlServer(connectionString);
                    break;
            }
            _options = optBuilder.Options;
        }

        public DbContext GetDbContext()
        {
            return (T)Activator.CreateInstance(typeof(T), _options);
        }
    }
}
