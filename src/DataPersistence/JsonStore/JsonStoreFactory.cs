using StoreEntities;
using System;
using System.Collections.Generic;
using System.IO;

namespace JsonStore
{
    public class JsonStoreFactory
    {
        private readonly Dictionary<Type, object> _storeCache;

        public JsonStoreFactory()
        {
            _storeCache = new Dictionary<Type, object>();
        }

        public IEntityStore<T> GetStore<T>(string entityStoreDirectory = @"%localappdata%\JsonStore") where T : IEntity
        {
            if (!_storeCache.TryGetValue(typeof(T), out object store))
            {
                string entityName = typeof(T).FullName;
                string entityStorePath = Path.Combine(entityStoreDirectory, entityName);
                store = new BaseJsonStore<T>(entityStorePath);
                Directory.CreateDirectory(entityStorePath);
                _storeCache.Add(typeof(T), store);
            }
            return (IEntityStore<T>)store;
        }

        public void SetStore<T>(IEntityStore<T> store) where T : IEntity
        {
            _storeCache[typeof(T)] = store;
        }
    }
}
