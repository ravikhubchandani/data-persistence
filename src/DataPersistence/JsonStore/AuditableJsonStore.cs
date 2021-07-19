using StoreEntities;
using System;
using System.Collections.Generic;

namespace JsonStore
{
    public class AuditableJsonStore<T> : EntityJsonStore<T> where T : IAuditableEntity
    {
        private readonly bool _keepMostRecentItem;

        public AuditableJsonStore(string entityStorePath, bool keepMostRecentItem = true) : base(entityStorePath)
        {
            _keepMostRecentItem = keepMostRecentItem;
        }

        public override void Delete(Func<T, bool> query)
        {
            var items = Get(query);
            foreach (var item in items)
            {
                item.Deleted = true;
                Delete(item);
            }
        }

        public override void Delete(T item)
        {
            item.Deleted = true;
            item.DeletedBy = GetUserName();
            item.DeletedOn = DateTime.Now;
            SerializeToPath(item);
        }

        public override void SaveOrUpdate(IEnumerable<T> items)
        {
            foreach (var item in items)
                SaveOrUpdate(item);
        }

        public override void SaveOrUpdate(T item)
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
    }
}
