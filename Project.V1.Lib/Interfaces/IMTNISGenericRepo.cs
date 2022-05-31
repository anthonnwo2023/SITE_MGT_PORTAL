using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Project.V1.Lib.Interfaces
{
    public interface IMTNISGenericRepo<T> where T : class
    {
        Task<IQueryable<T>> Get();
        Task<IQueryable<T>> Get(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeProperties = "");
        Task<T> GetById(Expression<Func<T, bool>> IdFilter = null, Expression<Func<T, bool>> filter = null, string includeProperties = "");
        Task<int> Count(Expression<Func<T, bool>> filter = null);
        Task<List<T>> GetTracked(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeProperties = "");
    }
}
