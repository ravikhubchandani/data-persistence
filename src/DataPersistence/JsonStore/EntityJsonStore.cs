using Newtonsoft.Json;
using StoreEntities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace JsonStore
{
    public class EntityJsonStore<T> : IEntityStore<T> where T : IEntity
    {
        protected readonly string entityStorePath;   

        /// <param name="keepMostRecentItem">If true, on item update will check if there is a more recent version for the same item Id.
        /// If there is a more recent, item will not be update to prevent data loss</param>
        public EntityJsonStore(string entityStorePath)
        {
            this.entityStorePath = entityStorePath;
        }

        public virtual T Get(int id)
        {
            string path = GetEntityFiles($"{id}.json").SingleOrDefault();
            return path == null ? default(T) : DeserializeFromPath(path);
        }

        public virtual IEnumerable<T> Get(Func<T, bool> query)
        {
            return Get().Where(query).ToArray();
        }

        public virtual IEnumerable<T> Get()
        {
            string[] files = GetEntityFiles();
            if (files.Any())
            {
                var items = new T[files.Length];
                for (int i = 0; i < files.Length; i++)
                    items[i] = DeserializeFromPath(files[i]);
                return items;
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
                Delete(item);
            }
        }

        public virtual void Delete(T item)
        {
            SerializeToPath(item);
        }

        public virtual void SaveOrUpdate(IEnumerable<T> items)
        {
            foreach (var item in items)
                SaveOrUpdate(item);
        }

        public virtual void SaveOrUpdate(T item)
        {
            if (item.Id == default(int))
            {
                item.Id = GetEntityFiles().Length + 1;
            }
                        
            SerializeToPath(item);
        }        

        protected string[] GetEntityFiles(string pattern = "*.json")
        {
            return Directory.GetFiles(entityStorePath, pattern);
        }

        protected T DeserializeFromPath(string path)
        {
            string json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<T>(json);
        }

        protected void SerializeToPath(T item)
        {
            string json = JsonConvert.SerializeObject(item);
            string path = Path.Combine(entityStorePath, $"{item.Id}.json");
            File.WriteAllText(path, json);
        }

        protected string GetUserName()
        {
            return $"{Environment.UserDomainName}.{Environment.UserName}";
        }
    }
}
