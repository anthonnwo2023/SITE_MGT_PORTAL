using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Project.V1.Lib.Interfaces
{
    public interface IGenericRepo<T> where T : class
    {
        Task<List<T>> Get();
        Task<List<T>> Get(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeProperties = "");
        Task<(T, string)> Create(T item);
        Task Save();
        Task<T> GetById(Expression<Func<T, bool>> IdFilter = null, Expression<Func<T, bool>> filter = null, string includeProperties = "");
        Task<(T, string)> Update(T item, Expression<Func<T, bool>> IdFilter);
        Task<(T, string)> Delete(T item, Expression<Func<T, bool>> IdFilter, Expression<Func<T, bool>> filter = null);
        Task<(bool, string)> CreateRequest(T item);
        Task<bool> CreateBulkRequest(List<T> requestObjs);
        Task<bool> UpdateRequest(T item, Expression<Func<T, bool>> IdFilter);
        Task<int> Count(Expression<Func<T, bool>> filter = null);
    }
}
