using LaptopStore.Data.Contexts;
using LaptopStore.Data.Entities;

namespace LaptopStore.Data.Contracts.Base
{
    public interface IUnitOfWork : IDisposable
    {
        ApplicationDbContext Context { get; }
        IGenericRepository<Product> ProductRepository { get; }
        IGenericRepository<Brand> BrandRepository { get; }
        IGenericRepository<T> GenericRepository<T>() where T : class;
        int SaveChanges();
        Task<int> SaveAsync();

        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();

    }
}
