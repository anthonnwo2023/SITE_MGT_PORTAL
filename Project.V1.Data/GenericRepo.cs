using Microsoft.EntityFrameworkCore;
using Project.V1.Lib.Interfaces;
using Project.V1.Lib.Helpers;
using Project.V1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Serilog.Core;

namespace Project.V1.Data
{
    public class GenericRepo<T> : IDisposable, IGenericRepo<T> where T : class
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
                return await entity.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, new { }, ex);
                return default;
            }
        }

        public async Task<List<T>> Get(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeProperties = "")
        {
            try
            {
                IQueryable<T> query = entity;

                if (filter != null)
                {
                    query = query.Where(filter);
                }

                foreach (string includeProperty in includeProperties.Split
                    (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty);
                }

                if (orderBy != null)
                {
                    return await orderBy(query).ToListAsync();
                }

                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, new { }, ex);
                return default;
            }
        }

        public async Task<T> GetById(Expression<Func<T, bool>> IdFilter, Expression<Func<T, bool>> filter = null)
        {
            try
            {
                IQueryable<T> query = entity;

                if (filter != null)
                {
                    query = query.Where(filter);
                }

                return await query.FirstOrDefaultAsync(IdFilter);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, new { }, ex);
                return default;
            }
        }

        public async Task<T> Create(T item)
        {
            try
            {
                if (item == null)
                {
                    throw new ArgumentNullException(nameof(item));
                }
                else
                {
                    ((dynamic)item).Id = Guid.NewGuid().ToString();
                    ((dynamic)item).DateCreated = DateTime.Now;
                    await entity.AddAsync(item);

                    await Save();
                }

                return item;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, new { }, ex);
                return default;
            }
        }

        public async Task<(bool, string)> CreateRequest(T item)
        {
            try
            {
                //requestObj.Id = Guid.NewGuid().ToString();
                ((dynamic)item).UniqueId = CheckUniqueness(HelperFunctions.GenerateIDUnique(_KeyString));

                entity.Add(item);

                await _context.SaveChangesAsync();

                return (true, "");
            }
            catch (Exception ex)
            {
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

                return false;
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
            return entity.Cast<RequestViewModel>().Any(e => e.UniqueId == UniqueId);
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

        public async Task<T> Delete(T item, Expression<Func<T, bool>> IdFilter, Expression<Func<T, bool>> filter = null)
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

                return item;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, new { }, ex);
                return default;
            }
        }

        public async Task<T> Update(T item, Expression<Func<T, bool>> IdFilter)
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

                    return itemObj;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, new { }, ex);
                return default;
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
