using StoreEntities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EFCoreConnectorStore
{
    public class GenericRepository<T> : IEntityStore<T> where T : class, IEntity
    {
        protected readonly IDbContextFactory dbContextFactory;

        public GenericRepository(IDbContextFactory dbContextFactory)
        {
            this.dbContextFactory = dbContextFactory;
        }

        public void Delete(int id)
        {
            using (var dbContext = dbContextFactory.GetDbContext())
            {
                var entity = dbContext.Set<T>().Find(id);
                if (entity == null)
                    return;

                dbContext.Set<T>().Remove(entity);
                dbContext.SaveChanges();
            }
        }

        public void Delete(T item)
        {
            using (var dbContext = dbContextFactory.GetDbContext())
            {
                dbContext.Set<T>().Remove(item);
                dbContext.SaveChanges();
            }
        }

        public void DeleteAll()
        {
            Delete(x => true);
        }

        public void Delete(Func<T, bool> query)
        {
            using (var dbContext = dbContextFactory.GetDbContext())
            {
                var entities = dbContext.Set<T>().Where(query);
                if (entities == null)
                    return;

                dbContext.Set<T>().RemoveRange(entities);
                dbContext.SaveChanges();
            }
        }

        public T Get(int id)
        {
            using (var dbContext = dbContextFactory.GetDbContext())
            {
                return dbContext.Set<T>().Find(id);
            }
        }

        public IEnumerable<T> Get(Func<T, bool> query, bool includeDeleted = false)
        {
            return Get(includeDeleted).Where(query).ToList();
        }

        public IEnumerable<T> Get(bool includeDeleted = false)
        {
            using (var dbContext = dbContextFactory.GetDbContext())
            {
                if (includeDeleted)
                    return dbContext.Set<T>().ToList();
                else
                    return dbContext.Set<T>().Where(x => !x.Deleted).ToList();
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
            using (var dbContext = dbContextFactory.GetDbContext())
            {
                foreach (var item in items)
                {
                    if (dbContext.Set<T>().Find(item.Id) == null)
                        dbContext.Set<T>().Add(item);
                    else
                        dbContext.Set<T>().Update(item);
                }
                dbContext.SaveChanges();
            }
        }
    }
}
