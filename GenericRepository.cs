using Hospital.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Repositories.Implementation
{
    public class GenericRepository<T> : IDisposable, IGenericRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        internal DbSet<T> dbSet;

        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
            dbSet = _context.Set<T>();
        }

        public void Add(T entity)
        {
            dbSet.Add(entity);
            _context.SaveChanges();  // Save changes to persist the addition
        }

        public async Task AddAsync(T entity)
        {
            await dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();  // Save changes to persist the addition
        }

        public void Delete(T entity)
        {
            dbSet.Remove(entity);
            _context.SaveChanges();  // Save changes to persist the deletion
        }

        public async Task DeleteAsync(T entity)
        {
            dbSet.Remove(entity);
            await _context.SaveChangesAsync();  // Save changes to persist the deletion
        }

        public void Dispose()
        {
            _context.Dispose();  // Dispose of the DbContext
        }

        public IEnumerable<T> GetAll(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeProperties = "")
        {
            IQueryable<T> query = dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (includeProperties != null)
            {
                foreach (var includeProperty in includeProperties.Split(',', StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty);
                }
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            return query.AsEnumerable();
        }

        public T GetById(object id)
        {
            return dbSet.Find(id);  // Find entity by primary key
        }

        public async Task<T> GetByIdAsync(object id)
        {
            return await dbSet.FindAsync(id);  // Find entity by primary key asynchronously
        }

        public void Update(T entity)
        {
            dbSet.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
            _context.SaveChanges();  // Save changes to persist the update
        }

        public async Task UpdateAsync(T entity)
        {
            dbSet.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();  // Save changes to persist the update asynchronously
        }
    }
}