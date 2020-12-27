using StoreEntities;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var animalStore = GetStore<Animal>();
            var animals = animalStore.Get();


            //var factory = new JsonStore.JsonStoreFactory();
            //var personStore = new BaseJsonStore<Person>("");
            //factory.SetJsonStore(personStore);
            //var caca = factory.GetJsonStore<Person>();
        }

        private static IEntityStore<T> GetStore<T>() where T : IEntity
        {
            var factory = new JsonStore.JsonStoreFactory();
            return factory.GetJsonStore<T>();
        }
    }
}
