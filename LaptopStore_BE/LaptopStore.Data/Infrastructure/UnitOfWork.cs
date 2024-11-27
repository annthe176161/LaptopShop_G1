using LaptopStore.Data.Contexts;
using LaptopStore.Data.Contracts;
using LaptopStore.Data.Contracts.Base;
using LaptopStore.Data.Entities;
using LaptopStore.Data.Repositories.Base;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaptopStore.Data.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IDbContextTransaction _transaction;
        private readonly IGenericRepository<Brand> _brandRepository;
        private readonly IGenericRepository<Product> _productRepository;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }
        public ApplicationDbContext Context => _context;

        public IGenericRepository<Product> ProductRepository => _productRepository ?? new GenericRepository<Product>(_context);
        public IGenericRepository<Brand> BrandRepository => _brandRepository ?? new GenericRepository<Brand>(_context);



        public async Task BeginTransactionAsync()
        {
            await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            await _context.Database.CommitTransactionAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public async Task RollbackTransactionAsync()
        {
            await _context.Database.RollbackTransactionAsync();
        }

        public async Task<int> SaveAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public int SaveChanges()
        {
            return _context.SaveChanges();
        }

        public IGenericRepository<T> GenericRepository<T>() where T : class
        {
            if (_context.Set<T>() == null)
            {
                throw new ArgumentNullException($"Repository for {typeof(T)} not found.");
            }
            return new GenericRepository<T>(_context);
        }
    }
}
