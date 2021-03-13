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

        /// <param name="keepMostRecentItem">If true, on item update will check if there is a more recent version for the same item Id.
        /// If there is a more recent, item will not be update to prevent data loss</param>
        public IEntityStore<T> GetStore<T>(string entityStoreDirectory = null, bool keepMostRecentItem = true) where T : IEntity
        {
            if (!_storeCache.TryGetValue(typeof(T), out object store))
            {
                if (string.IsNullOrWhiteSpace(entityStoreDirectory))
                    entityStoreDirectory = $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\\JsonStore";
                string entityName = typeof(T).FullName;
                string entityStorePath = Path.Combine(entityStoreDirectory, entityName);
                store = new BaseJsonStore<T>(entityStorePath, keepMostRecentItem);
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
