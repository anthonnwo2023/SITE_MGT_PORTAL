using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Project.V1.Data.Interfaces
{
    public interface IGenericRepo<T> where T : class
    {
        Task<List<T>> Get();
        Task<List<T>> Get(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeProperties = "");
        Task<T> Create(T item);
        Task Save();
        Task<T> GetById(Expression<Func<T, bool>> IdFilter = null, Expression<Func<T, bool>> filter = null);
        Task<T> Update(T item, Expression<Func<T, bool>> IdFilter);
        Task<T> Delete(T item, Expression<Func<T, bool>> IdFilter, Expression<Func<T, bool>> filter = null);
    }
}
