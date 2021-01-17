using EFCoreConnectorStore;
using JsonStore;
using Microsoft.EntityFrameworkCore;
using StoreEntities;
using System;
using Test.NewFolder;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            // These factories and stores can be moved to dependency injectors

            var factory = new JsonStoreFactory();
            var animalStore = factory.GetStore<Animal>();
            var animals = animalStore.Get(true);
            var myDog = new Animal { Name = "Johnson" };
            animalStore.SaveOrUpdate(myDog);

            Console.WriteLine($"{myDog.Name}'s ID: {myDog.Id}");
            animalStore.Delete(myDog);


            /*Install-Package Microsoft.EntityFrameworkCore.Tools
             *Install-Package Microsoft.EntityFrameworkCore.Sqlite
             *Scaffold-DbContext "DataSource=test.db" Microsoft.EntityFrameworkCore.Sqlite -OutputDir "NewFolder" */

            var dbCtxFactory = new DbContextFactory<testContext>(new DbContextOptions<testContext>());
            IEntityStore<Person> personStore = new GenericRepository<Person>(dbCtxFactory);
            var me = new Person { Age = 30 };
            personStore.SaveOrUpdate(me);
        }
    }
}
