using Microsoft.EntityFrameworkCore;
using Project.V1.Lib.Helpers;
using Project.V1.Lib.Interfaces;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Project.V1.Data
{
    public class GenericRepo<T> : IDisposable, IGenericRepo<T> where T : class, new()
    {
        private readonly ApplicationDbContext _context = null;
        private readonly Logger _logger;
        private readonly DbSet<T> entity = null;
        private bool disposed = false;
        private readonly string _KeyString;

        public GenericRepo(ApplicationDbContext context, string KeyString)
        {
            _context = context;
            _logger = HelperFunctions.GetSerilogLogger();
            _KeyString = KeyString;
            entity = _context.Set<T>();
        }

        public async Task<List<T>> Get()
        {
            try
            {
                _context.Database.CloseConnection();
                _context.Database.OpenConnection();

                await entity.LoadAsync();

                //await entity.ForEachAsync(x =>
                //{
                //    ReloadEntry(x);
                //});

                return await entity.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, new { }, ex);
                return new();
            }
        }

        public async Task<List<T>> Get(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeProperties = "")
        {
            try
            {
                _context.Database.CloseConnection();
                _context.Database.OpenConnection();

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

                //await query.ForEachAsync(x =>
                //{
                //    ReloadEntry(x);
                //});

                if (orderBy != null)
                {
                    return await orderBy(query).ToListAsync();
                }

                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, new { }, ex);
                return new();
            }
        }

        public async Task<int> Count(Expression<Func<T, bool>> filter = null)
        {
            try
            {
                _context.Database.CloseConnection();
                _context.Database.OpenConnection();

                await entity.LoadAsync();

                IQueryable<T> query = entity;

                if (filter != null)
                {
                    query = query.Where(filter);
                }

                return await query.CountAsync();
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, new { }, ex);
                return new();
            }
        }

        private void ReloadEntry(T item, string includeProperties = "")
        {
            _context.Entry(item).Reload();
            _context.Entry(item).GetDatabaseValues();

            _context.Entry(item).Collections.ToList().ForEach(collection =>
            {
                collection.Load();
            });

            foreach (string includeProperty in includeProperties.Split
                    (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                _context.Entry(item).Collection<T>(includeProperties).Query();
            }
        }

        public async Task<T> GetById(Expression<Func<T, bool>> IdFilter, Expression<Func<T, bool>> filter = null, string includeProperties = "")
        {
            try
            {
                _context.Database.CloseConnection();
                _context.Database.OpenConnection();

                await entity.LoadAsync();

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

                var item = query.FirstOrDefault(IdFilter);

                ReloadEntry(item);

                return item;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, new { }, ex);
                return new();
            }
        }

        public async Task<(T, string)> Create(T item)
        {
            try
            {
                if (item == null)
                {
                    throw new ArgumentNullException(nameof(item));
                }
                else
                {
                    ((dynamic)item).Id = ((dynamic)item).Id ?? Guid.NewGuid().ToString();
                    ((dynamic)item).DateCreated = DateTime.Now;

                    await entity.AddAsync(item);

                    await Save();
                }

                return (item, "");
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, new { }, ex);
                return (default, ex.Message);
            }
        }

        public async Task<(bool, string)> CreateRequest(T item)
        {
            try
            {
                //(item as dynamic).Id = (item as dynamic).Id ?? Guid.NewGuid().ToString();
                (item as dynamic).UniqueId = CheckUniqueness(HelperFunctions.GenerateIDUnique(_KeyString));

                entity.Add(item);

                await _context.SaveChangesAsync();

                return (true, "");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"{ex.InnerException} - {ex.Message}");
                return (false, (ex.InnerException.Message.Contains("unique")) ? $"Duplicate entry already exists" : ex.Message);
            }
        }

        public async Task<bool> UpdateRequest(T item, Expression<Func<T, bool>> IdFilter)
        {
            try
            {
                if (item == null)
                {
                    throw new ArgumentNullException(nameof(item));
                }
                else
                {
                    T itemObj = await entity.FirstOrDefaultAsync(IdFilter);
                    _context.Entry(itemObj).CurrentValues.SetValues(item);

                    _context.Entry(itemObj).State = EntityState.Modified;

                    entity.Update(itemObj);
                    await Save();

                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);

                throw;
            }
        }

        private string CheckUniqueness(string UniqueId)
        {
            string uniqueId = UniqueId;

            if (UniqueIdExists(UniqueId))
            {
                uniqueId = HelperFunctions.GenerateIDUnique(_KeyString);

                CheckUniqueness(uniqueId);
            }

            return uniqueId;
        }

        private bool UniqueIdExists(string UniqueId)
        {

            var parameter = Expression.Parameter(typeof(T), "p");
            var predicate = Expression.Lambda<Func<T, bool>>(
                Expression.Equal(Expression.PropertyOrField(parameter, "UniqueId"), Expression.Constant(UniqueId)),
                parameter);

            return entity.Cast<T>().Any(predicate);
        }

        public async Task<bool> CreateBulkRequest(List<T> requestObjs)
        {
            try
            {
                requestObjs.ForEach((item) =>
                {
                    ((dynamic)item).UniqueId = CheckUniqueness(HelperFunctions.GenerateIDUnique(_KeyString));
                });

                entity.AddRange(requestObjs);

                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);

                return false;
            }
        }

        public async Task<(T, string)> Delete(T item, Expression<Func<T, bool>> IdFilter, Expression<Func<T, bool>> filter = null)
        {
            try
            {
                if (item == null)
                {
                    throw new ArgumentNullException(nameof(item));
                }
                else
                {
                    T itemObj = await entity.FirstOrDefaultAsync(IdFilter);

                    _context.Entry(itemObj).State = EntityState.Detached;
                    entity.Remove(itemObj);

                    await Save();
                }

                return (item, "");
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, new { }, ex);
                return (default, ex.Message);
            }
        }

        public async Task<(T, string)> Update(T item, Expression<Func<T, bool>> IdFilter)
        {
            try
            {
                if (item == null)
                {
                    throw new ArgumentNullException(nameof(item));
                }
                else
                {
                    T itemObj = await entity.FirstOrDefaultAsync(IdFilter);
                    _context.Entry(itemObj).CurrentValues.SetValues(item);
                    //entity.Attach(item);
                    _context.Entry(itemObj).State = EntityState.Modified;

                    entity.Update(itemObj);
                    await Save();

                    return (itemObj, "");
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, new { }, ex);
                return (default, ex.Message);
            }
        }

        public async Task Save()
        {
            await _context.SaveChangesAsync();
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
