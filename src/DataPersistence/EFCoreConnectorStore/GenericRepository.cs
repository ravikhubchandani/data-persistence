using Microsoft.EntityFrameworkCore;
using StoreEntities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EFCoreConnectorStore
{
    public class GenericRepository<T> : IEntityStore<T> where T : class, IEntity
    {
        protected readonly IDbContextFactory dbContextFactory;

        public GenericRepository(IDbContextFactory ctxFactory)
        {
            this.dbContextFactory = ctxFactory;
        }

        public void Delete(int id)
        {
            T item = null;
            using (var ctx = dbContextFactory.GetDbContext())
            {
                item = ctx.Set<T>().Find(id);
                if (item == null)
                    return;
            }
            Delete(item);
        }

        public void Delete(T item)
        {
            SetAsDeleted(item);
            SaveOrUpdate(item);
        }

        public void DeleteAll()
        {
            Delete(x => true);
        }

        public void Delete(Func<T, bool> query)
        {
            List<T> items = null;

            using (var ctx = dbContextFactory.GetDbContext())
            {
                items = ctx.Set<T>().Where(query).ToList();
                if (items == null || !items.Any())
                    return;
            }

            items.ForEach(x => SetAsDeleted(x));
            SaveOrUpdate(items);
        }

        private void SetAsDeleted(T item)
        {
            item.Deleted = true;
            item.DeletedBy = string.Empty;
            item.DeletedOn = DateTime.Now;
        }

        public T Get(int id)
        {
            using (var ctx = dbContextFactory.GetDbContext())
            {
                return ctx.Set<T>().Find(id);
            }
        }

        public IEnumerable<T> Get(Func<T, bool> query, bool includeDeleted = false)
        {
            using (var ctx = dbContextFactory.GetDbContext())
            {
                var items = ctx.Set<T>().Where(query);
                if (!includeDeleted)
                    items = items.Where(x => !x.Deleted);
                return items.ToList();
            }
        }

        public IEnumerable<T> Get(bool includeDeleted = false)
        {
            using (var ctx = dbContextFactory.GetDbContext())
            {
                if (includeDeleted)
                    return ctx.Set<T>().ToList();
                else
                    return ctx.Set<T>().Where(x => !x.Deleted).ToList();
            }
        }

        public void SaveOrUpdate(IEnumerable<T> items)
        {
            SaveOrUpdateMultiple(items.ToArray());
        }

        public void SaveOrUpdate(T item)
        {
            SaveOrUpdateMultiple(item);
        }

        private void SaveOrUpdateMultiple(params T[] items)
        {
            using (var ctx = dbContextFactory.GetDbContext())
            {
                foreach (var item in items)
                {
                    item.UpdatedOn = DateTime.Now;
                    item.UpdatedBy = string.Empty;

                    var dbItem = ctx.Set<T>().Find(item.Id);
                    if (dbItem == null)
                    {
                        item.Version = 1;
                        ctx.Set<T>().Add(item);
                    }
                    else
                    {
                        if (item.Version != dbItem.Version)
                            throw new StaleDataException($"Cannot overwrite {typeof(T).Name} with Id {item.Id}, version {item.Version}. Current version is {dbItem.Version}. Exception thrown to prevent data loss.");

                        item.Version += 1;
                        
                        ctx.Entry(dbItem).State = EntityState.Detached;
                        ctx.Entry(item).State = EntityState.Modified;
                        ctx.Set<T>().Update(item);
                    }
                }
                ctx.SaveChanges();
            }
        }
    }
}
