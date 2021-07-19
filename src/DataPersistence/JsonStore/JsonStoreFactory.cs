using StoreEntities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace JsonStore
{
    public class JsonStoreFactory
    {
        private readonly Dictionary<Type, object> _storeCache;
        public string EntityStoreDirectory { get; private set; }

        public JsonStoreFactory()
        {
            _storeCache = new Dictionary<Type, object>();
        }

        /// <param name="keepMostRecentItem">If true, on item update will check if there is a more recent version for the same item Id.
        /// If there is a more recent, item will not be update to prevent data loss</param>
        public IEntityStore<T> GetAuditableStore<T>(string entityStoreDirectory = null, bool keepMostRecentItem = true) where T : IAuditableEntity
        {
            if (!_storeCache.TryGetValue(typeof(T), out object store))
            {
                EntityStoreDirectory = GetStoreDirectory<T>(entityStoreDirectory);
                store = new AuditableJsonStore<T>(EntityStoreDirectory, keepMostRecentItem);
                _storeCache.Add(typeof(T), store);
            }
            return (IEntityStore<T>)store;
        }

        public IEntityStore<T> GetEntityStore<T>(string entityStoreDirectory = null) where T : IEntity
        {
            if (!_storeCache.TryGetValue(typeof(T), out object store))
            {
                EntityStoreDirectory = GetStoreDirectory<T>(entityStoreDirectory);
                store = new EntityJsonStore<T>(EntityStoreDirectory);                
                _storeCache.Add(typeof(T), store);
            }
            return (IEntityStore<T>)store;
        }

        private string GetStoreDirectory<T>(string entityStoreDirectory = null)
        {
            var dir = string.IsNullOrWhiteSpace(entityStoreDirectory) ?
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "JsonStore", Assembly.GetCallingAssembly().GetName().Name) :
                entityStoreDirectory;
            string entityName = typeof(T).FullName;
            dir = Path.Combine(dir, entityName);
            Directory.CreateDirectory(dir);
            return dir;
        }

        public void SetStore<T>(IEntityStore<T> store) where T : IEntity
        {
            _storeCache[typeof(T)] = store;
        }
    }
}
