using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace GainBargain.DAL.Interfaces
{
    public interface IRepository<T> : IDisposable where T : class
    {
        T Get(int id);
        IEnumerable<T> GetAll();

        IEnumerable<T> Find(Expression<Func<T, bool>> predicate);

        void Add(T entity);
        void AddRange(IEnumerable<T> entities);

        void Remove(int id);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);

        void Update(T entity);

        void Save();
    }
}
