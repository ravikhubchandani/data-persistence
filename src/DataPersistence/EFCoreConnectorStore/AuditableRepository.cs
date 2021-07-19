using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using StoreEntities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EFCoreConnectorStore
{
    public class AuditableRepository<T> : EntityRepository<T> where T : class, IAuditableEntity
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuditableRepository(IDbContextFactory ctxFactory) : base(ctxFactory)
        {
        }

        public AuditableRepository(IDbContextFactory ctxFactory, IHttpContextAccessor httpContextAccessor) : base(ctxFactory)
        {
            this._httpContextAccessor = httpContextAccessor;
        }

        public override void Delete(int id)
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

        public override void Delete(T item)
        {
            SetAsDeleted(item);
            SaveOrUpdate(item);
        }

        public override void DeleteAll()
        {
            Delete(x => !x.Deleted);
        }

        public override void Delete(Func<T, bool> query)
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
            item.DeletedBy = GetUserName();
            item.DeletedOn = DateTime.Now;
        }

        public override IEnumerable<T> Get(Func<T, bool> query)
        {
            using (var ctx = dbContextFactory.GetDbContext())
            {
                var items = ctx.Set<T>().Where(query);
                return items.ToList();
            }
        }

        public override IEnumerable<T> Get()
        {
            using (var ctx = dbContextFactory.GetDbContext())
            {
                return ctx.Set<T>().Where(x => !x.Deleted).ToList();
            }
        }

        public override void SaveOrUpdate(IEnumerable<T> items)
        {
            SaveOrUpdateMultiple(items.ToArray());
        }

        public override void SaveOrUpdate(T item)
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
                    item.UpdatedBy = GetUserName();

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

        private string GetUserName()
        {
            if (_httpContextAccessor != null)
                return _httpContextAccessor.HttpContext.User.Identity.Name;
            else
                return $"{Environment.UserDomainName}.{Environment.UserName}";
        }
    }
}
