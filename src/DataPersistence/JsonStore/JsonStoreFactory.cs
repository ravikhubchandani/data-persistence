using StoreEntities;
using System;
using System.Collections.Generic;

namespace JsonStore
{
    public class JsonStoreFactory
    {
        private readonly string _entityStoreDirectory;
        private readonly Dictionary<Type, object> _storeCache;

        public JsonStoreFactory(string entityStoreDirectory = @"%userprofile%\JsonStore")
        {
            _entityStoreDirectory = entityStoreDirectory;
            _storeCache = new Dictionary<Type, object>();
        }

        public BaseJsonStore<T> GetBaseJsonStore<T>() where T : IEntity
        {
            return (BaseJsonStore<T>)GetJsonStore<T>();
        }

        public IEntityStore<T> GetJsonStore<T>() where T : IEntity
        {
            if (!_storeCache.TryGetValue(typeof(T), out object store))
            {
                store = new BaseJsonStore<T>(_entityStoreDirectory);
                _storeCache.Add(typeof(T), store);
            }
            return (IEntityStore<T>)store;
        }

        public void SetJsonStore<T>(IEntityStore<T> store) where T : IEntity
        {
            _storeCache[typeof(T)] = store;
        }
    }
}
