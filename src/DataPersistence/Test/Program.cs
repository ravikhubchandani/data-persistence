using JsonStore;
using System;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new JsonStoreFactory();

            var animalStore = factory.GetStore<Animal>();
            var animals = animalStore.Get(true);
            var myDog = new Animal { Name = "Johnson" };
            animalStore.SaveOrUpdate(myDog);
            Console.WriteLine($"{myDog.Name}'s ID: {myDog.Id}");
            animalStore.Delete(myDog);
            
            //var personStore = new BaseJsonStore<Person>();
            //factory.SetStore(personStore);
            //var test = factory.GetStore<Person>();
        }
    }
}
