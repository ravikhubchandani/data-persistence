using StoreEntities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace JsonStore
{
    public class BaseJsonStore<T> : IEntityStore<T> where T : IEntity
    {
        protected string entityStoreDirectory;
        protected string entityName;
        protected string entityStorePath;

        public BaseJsonStore(string entityStoreDirectory)
        {
            this.entityStoreDirectory = entityStoreDirectory;
        }

        public virtual IEnumerable<T> Get(bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public virtual IEnumerable<T> Get(Expression<Func<T, bool>> query, bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public virtual T Get(int Id)
        {
            throw new NotImplementedException();
        }

        public virtual void Save(T item)
        {
            throw new NotImplementedException();
        }

        public virtual void Save(IEnumerable<T> items)
        {
            throw new NotImplementedException();
        }

        public virtual void Update(T item)
        {
            throw new NotImplementedException();
        }

        public virtual void Delete(T item)
        {
            throw new NotImplementedException();
        }

        public virtual void Delete(Expression<Func<T, bool>> query)
        {
            throw new NotImplementedException();
        }

        public virtual void DeleteAll()
        {
            throw new NotImplementedException();
        }
    }
}
