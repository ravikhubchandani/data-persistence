using EFCoreConnectorStore;
using Microsoft.EntityFrameworkCore;
using StoreEntities;
using Test.NewFolder;

namespace Test
{
    public static class DatabaseStore
    {
        public static void Test()
        {
            // ---------------------------------------------------------------------------------------------------
            // If you want to build a DB Context from an existing database:
            // Install-Package Microsoft.EntityFrameworkCore.Tools
            // Scaffold-DbContext "DataSource=test.db" Microsoft.EntityFrameworkCore.Sqlite -OutputDir "NewFolder"
            // ---------------------------------------------------------------------------------------------------

            /* Option 1 - Pass options using builder
            var optBuilder = new DbContextOptionsBuilder<testContext>();
            optBuilder.UseSqlite("DataSource=test.db");
            var dbCtxFactory = new DbContextFactory<testContext>(optBuilder.Options);*/

            // Option 2 - Pass connection string
            var dbCtxFactory = new DbContextFactory<testContext>(DatabaseEnums.SQLITE, "DataSource=test.db");

            IEntityStore<Person> personStore = new GenericRepository<Person>(dbCtxFactory);
            var me = new Person { Age = 30 };
            personStore.SaveOrUpdate(me);

            // This factory and store can be moved to dependency injectors
        }
    }
}
