using EFCoreConnectorStore;
using StoreEntities;
using System;
using Test.NewFolder;

namespace Test
{
    public static class DatabaseStoreTest
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

            var notClone = personStore.Get(me.Id);
            notClone.Age = 18;
            personStore.SaveOrUpdate(notClone);

            try
            {
                me.Age = 17;
                personStore.SaveOrUpdate(me);
            }
            catch (StaleDataException e)
            {
                // Message explains a more recent item exists (bigger Version number) so updating might lose data
                Console.WriteLine(e.Message);
            }

            personStore.Delete(notClone);
            personStore.DeleteAll();

            // This factory and store can be moved to dependency injectors
        }
    }
}
