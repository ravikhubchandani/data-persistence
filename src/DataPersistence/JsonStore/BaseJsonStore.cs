using Newtonsoft.Json;
using StoreEntities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace JsonStore
{
    public class BaseJsonStore<T> : IEntityStore<T> where T : IEntity
    {
        private readonly string _entityStorePath;
        private readonly bool _keepMostRecentItem;

        /// <param name="keepMostRecentItem">If true, on item update will check if there is a more recent version for the same item Id.
        /// If there is a more recent, item will not be update to prevent data loss</param>
        public BaseJsonStore(string entityStorePath, bool keepMostRecentItem = true)
        {
            _entityStorePath = entityStorePath;
            _keepMostRecentItem = keepMostRecentItem;
        }

        public virtual T Get(int id)
        {
            string path = GetEntityFiles($"{id}.json").SingleOrDefault();
            return path == null ? default(T) : DeserializeFromPath(path);
        }

        public virtual IEnumerable<T> Get(Func<T, bool> query, bool includeDeleted = false)
        {
            return Get(includeDeleted).Where(query).ToArray();
        }

        public virtual IEnumerable<T> Get(bool includeDeleted = false)
        {
            string[] files = GetEntityFiles();
            if (files.Any())
            {
                var items = new T[files.Length];
                for(int i = 0; i < files.Length; i++)
                    items[i] = DeserializeFromPath(files[i]);

                if (!includeDeleted)
                    return items.Where(x => x.Deleted == false).ToArray();
                else return items;
            }
            else return new T[] { };
        }

        public virtual void Delete(int id)
        {
            var item = Get(id);
            Delete(item);
        }

        public virtual void DeleteAll()
        {
            Delete(x => true);
        }

        public virtual void Delete(Func<T, bool> query)
        {
            var items = Get(query);
            foreach (var item in items)
            {
                item.Deleted = true;
                Delete(item);
            }
        }

        public virtual void Delete(T item)
        {
            item.Deleted = true;
            item.DeletedBy = GetUserName();
            item.DeletedOn = DateTime.Now;
            SerializeToPath(item);
        }

        public virtual void SaveOrUpdate(IEnumerable<T> items)
        {
            foreach (var item in items)
                SaveOrUpdate(item);
        }

        public virtual void SaveOrUpdate(T item)
        {
            item.UpdatedOn = DateTime.Now;
            item.UpdatedBy = GetUserName();

            if (item.Id == default(int))
            {
                item.Id = GetEntityFiles().Length + 1;
                item.Version = 1;
            }
            else
            {
                if (_keepMostRecentItem)
                {
                    var currentItem = Get(item.Id);
                    if (currentItem.Version != item.Version)
                        throw new StaleDataException($"Cannot overwrite {typeof(T).Name} with Id {item.Id}, version {item.Version}. Current version is {currentItem.Version}. Exception thrown to prevent data loss.");
                }

                item.Version += 1;
            }
                        
            SerializeToPath(item);
        }        

        private string[] GetEntityFiles(string pattern = "*.json")
        {
            return Directory.GetFiles(_entityStorePath, pattern);
        }

        private T DeserializeFromPath(string path)
        {
            string json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<T>(json);
        }

        private void SerializeToPath(T item)
        {
            string json = JsonConvert.SerializeObject(item);
            string path = Path.Combine(_entityStorePath, $"{item.Id}.json");
            File.WriteAllText(path, json);
        }

        private string GetUserName()
        {
            return $"{Environment.UserDomainName}.{Environment.UserName}";
        }
    }
}
