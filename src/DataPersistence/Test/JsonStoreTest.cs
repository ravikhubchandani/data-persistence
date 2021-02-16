using JsonStore;
using System;
using Test.NewFolder;

namespace Test
{
    public static class JsonStoreTest
    {
        public static void Test()
        {
            var factory = new JsonStoreFactory();
            var animalStore = factory.GetStore<Animal>();
            var animals = animalStore.Get(true);
            var myDog = new Animal { Name = "Johnson" };
            animalStore.SaveOrUpdate(myDog);

            Console.WriteLine($"{myDog.Name}'s ID: {myDog.Id}");
            animalStore.Delete(myDog);

            // This factory and store can be moved to dependency injectors
        }

    }
}
