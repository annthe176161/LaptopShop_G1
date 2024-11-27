using LaptopStore.Data.Contexts;
using LaptopStore.Data.Contracts.Base;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaptopStore.Data.Repositories.Base
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<TEntity> _dbSet;

        public GenericRepository(ApplicationDbContext Context) {
            _context = Context;
            _dbSet = _context.Set<TEntity>();

        }
        public void Add(TEntity entity)
        {
            _dbSet.Add(entity);
        }

        public void Delete(int id)
        {
            var entity = _dbSet.Find(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);

            }
        }

        public IEnumerable<TEntity> GetAll()
        {
            return _dbSet.ToList();
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public TEntity GetById(int id)
        {
            return _dbSet.Find(id);
        }

        public IQueryable<TEntity> GetQuery()
        {
            return _dbSet;
        }

        public void Update(TEntity entity)
        {
            _dbSet.Update(entity);
        }
    }
}
