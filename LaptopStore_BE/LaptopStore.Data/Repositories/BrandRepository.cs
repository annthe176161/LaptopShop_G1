using LaptopStore.Data.Contexts;
using LaptopStore.Data.Contracts;
using LaptopStore.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaptopStore.Data.Repositories
{
    public class BrandRepository : IBrandRepository
    {
        private readonly ApplicationDbContext _context;

        public BrandRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public void Add(Brand entity)
        {
           _context.Brands.Add(entity);
        }

        public void Delete(int id)
        {
           var brand = _context.Brands.Find(id);
            if (brand != null) {
                _context.Remove(brand);
            }
        }

        public IEnumerable<Brand> GetAll()
        {
           return _context.Brands.ToList();
        }

        public async Task<IEnumerable<Brand>> GetAllAsync()
        {
            return await _context.Brands.ToListAsync();
        }

        public Brand GetById(int id)
        {
            return _context.Brands.FirstOrDefault(b => b.BrandID == id);
        }

        public async Task<Brand> GetByNameAsync(string brandName)
        {
            return await _context.Brands.FirstOrDefaultAsync(b => b.BrandName == brandName);
        }

        public IQueryable<Brand> GetQuery()
        {
            return _context.Brands;
        }

        public void Update(Brand entity)
        {
           _context.Brands.Update(entity);
        }
    }
}
