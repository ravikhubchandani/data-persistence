using Microsoft.EntityFrameworkCore;
using StoreEntities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EFCoreConnectorStore
{
    public class EntityRepository<T> : IEntityStore<T> where T : class, IEntity
    {
        protected readonly IDbContextFactory dbContextFactory;        

        public EntityRepository(IDbContextFactory ctxFactory)
        {
            dbContextFactory = ctxFactory;
        }

        public virtual void Delete(T item)
        {
            using (var ctx = dbContextFactory.GetDbContext())
            {
                ctx.Set<T>().Remove(item);
                ctx.SaveChanges();
            }
        }

        public virtual void Delete(int id)
        {
            using (var ctx = dbContextFactory.GetDbContext())
            {
                var item = ctx.Set<T>().Find(id);
                if (item == null)
                    return;

                ctx.Set<T>().Remove(item);
                ctx.SaveChanges();
            }
        }

        public virtual void DeleteAll()
        {
            Delete(x => true);
        }

        public virtual void Delete(Func<T, bool> query)
        {
            List<T> items = null;

            using (var ctx = dbContextFactory.GetDbContext())
            {
                items = ctx.Set<T>().Where(query).ToList();
                if (items == null || !items.Any())
                    return;
            }

            items.ForEach(x => Delete(x));
        }

        public virtual T Get(int id)
        {
            using (var ctx = dbContextFactory.GetDbContext())
            {
                return ctx.Set<T>().Find(id);
            }
        }

        public virtual IEnumerable<T> Get(Func<T, bool> query)
        {
            using (var ctx = dbContextFactory.GetDbContext())
            {
                var items = ctx.Set<T>().Where(query);
                return items.ToList();
            }
        }

        public virtual IEnumerable<T> Get()
        {
            return Get(x => true);
        }

        public virtual void SaveOrUpdate(IEnumerable<T> items)
        {
            SaveOrUpdateMultiple(items.ToArray());
        }

        public virtual void SaveOrUpdate(T item)
        {
            SaveOrUpdateMultiple(item);
        }

        private void SaveOrUpdateMultiple(params T[] items)
        {
            using (var ctx = dbContextFactory.GetDbContext())
            {
                foreach (var item in items)
                {
                    var dbItem = ctx.Set<T>().Find(item.Id);
                    if (dbItem == null)
                    {
                        ctx.Set<T>().Add(item);
                    }
                    else
                    {
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
