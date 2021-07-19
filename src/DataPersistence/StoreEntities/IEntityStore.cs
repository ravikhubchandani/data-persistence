using System;
using System.Collections.Generic;

namespace StoreEntities
{
    public interface IEntityStore<T> where T : IEntity
    {
        IEnumerable<T> Get();
        IEnumerable<T> Get(Func<T, bool> query);
        T Get(int id);
        void SaveOrUpdate(T item);
        void SaveOrUpdate(IEnumerable<T> items);
        void DeleteAll();
        void Delete(int id);
        void Delete(T item);
        void Delete(Func<T, bool> query);        
    }
}
