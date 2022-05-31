using Microsoft.EntityFrameworkCore;
using Project.V1.Lib.Helpers;
using Project.V1.Lib.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Project.V1.Data
{
    public class MTNISGenericRepo<T> : IDisposable, IMTNISGenericRepo<T> where T : class, new()
    {
        private readonly MTNISDbContext _context = null;
        private readonly ICLogger _logger;
        private readonly DbSet<T> entity = null;
        private bool disposed = false;
        private readonly string _KeyString;

        public MTNISGenericRepo(MTNISDbContext context, string KeyString, ICLogger logger)
        {
            _context = context;
            _KeyString = KeyString;
            entity = _context.Set<T>();
            _logger = logger;
        }

        public async Task<IQueryable<T>> Get()
        {
            try
            {
                return await Task.FromResult(entity.AsNoTracking());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, new { }, ex);
                return Array.Empty<T>().AsQueryable();
            }
        }

        public async Task<IQueryable<T>> Get(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeProperties = "")
        {
            try
            {
                IQueryable<T> query = entity;

                foreach (string includeProperty in includeProperties.Split
                    (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty);
                }

                if (filter != null)
                {
                    query = query.Where(filter);
                }

                if (orderBy != null)
                {
                    return orderBy(query);
                }

                return await Task.FromResult(query);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, new { }, ex);
                return Array.Empty<T>().AsQueryable();
            }
        }

        public async Task<List<T>> GetTracked(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeProperties = "")
        {
            try
            {
                IQueryable<T> query = entity;

                foreach (string includeProperty in includeProperties.Split
                    (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty);
                }

                if (filter != null)
                {
                    query = query.Where(filter);
                }

                await query.LoadAsync();

                if (orderBy != null)
                {
                    return await orderBy(query).ToListAsync();
                }

                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, new { }, ex);
                return new();
            }
        }

        public async Task<int> Count(Expression<Func<T, bool>> filter = null)
        {
            try
            {
                IQueryable<T> query = entity;

                if (filter != null)
                {
                    query = query.Where(filter);
                }

                return await query.CountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, new { }, ex);
                return new();
            }
        }

        private async void ReloadEntry(T item)
        {
            await entity.LoadAsync();

            _context.Entry(item).Collections.ToList().ForEach(collection =>
            {
                if (collection.EntityEntry.State == EntityState.Detached)
                {
                    collection.Load();
                }
            });
            _context.Entry(item).References.ToList().ForEach(reference =>
            {
                if (reference.EntityEntry.State == EntityState.Detached)
                {
                    reference.Load();
                }
            });
        }

        public async Task<T> GetById(Expression<Func<T, bool>> IdFilter, Expression<Func<T, bool>> filter = null, string includeProperties = "")
        {
            try
            {
                IQueryable<T> query = entity;

                if (!string.IsNullOrWhiteSpace(includeProperties))
                {
                    foreach (string includeProperty in includeProperties.Split
                        (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        query = query.Include(includeProperty);
                    }
                }

                if (filter != null)
                {
                    query = query.Where(filter);
                }

                var item = await query.FirstOrDefaultAsync(IdFilter);

                //if (item != null && (item as dynamic).RequestType == "SA")
                //{
                //    //ReloadEntry(item);
                //}

                return item;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, new { }, ex);
                return new();
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    //_context.Dispose();
                }
            }
            this.disposed = true;
        }

        void IDisposable.Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
