using JsonStore;
using StoreEntities;
using System;
using Test.NewFolder;

namespace Test
{
    public static class JsonStoreTest
    {
        public static void Test()
        {
            // This factory and store can be moved to dependency injectors
            var factory = new JsonStoreFactory();
            var animalStore = factory.GetStore<Animal>();
            //var animalStore = factory.GetStore<Animal>(keepMostRecentItem: false);
            var animals = animalStore.Get(true);
            var myDog = new Animal { Name = "Johnson" };
            animalStore.SaveOrUpdate(myDog);

            Console.WriteLine($"{myDog.Name}'s ID: {myDog.Id}, path: {factory.EntityStoreDirectory}");


            var notClone = animalStore.Get(myDog.Id);
            notClone.Name = "Other Johnson";
            animalStore.SaveOrUpdate(notClone);

            try
            {
                myDog.Name = "Original Johnson";
                animalStore.SaveOrUpdate(myDog);
                // If keepMostRecentItem (Store constructure) was false, this exception would not be thrown
            }
            catch(StaleDataException e)
            {
                // Message explains a more recent item exists (bigger Version number) so updating might lose data
                Console.WriteLine(e.Message);
            }

            animalStore.Delete(myDog);
        }

    }
}
