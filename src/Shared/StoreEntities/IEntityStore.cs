using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace StoreEntities
{
    public interface IEntityStore<T> where T : IEntity
    {
        IEnumerable<T> Get(bool includeDeleted = false);
        IEnumerable<T> Get(Expression<Func<T, bool>> query, bool includeDeleted = false);
        T Get(int Id);
        void Save(T item);
        void Save(IEnumerable<T> items);
        void Update(T item);
        void DeleteAll();
        void Delete(T item);
        void Delete(Expression<Func<T, bool>> query);        
    }
}
