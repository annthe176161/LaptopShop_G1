using LaptopStore.Data.Contracts.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaptopStore.Business.Services.Base
{
    public class BaseService<T> : IBaseService<T> where T : class
    {
        private readonly IUnitOfWork _unitOfWork;

        public BaseService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public virtual int Add(T entity)
        {
            if (entity != null)
            {
                _unitOfWork.GenericRepository<T>().Add(entity);
                return _unitOfWork.SaveChanges();
            }
            else
            {
                throw new ArgumentNullException(nameof(entity), "Entity null");
            }
        }

        public virtual int Delete(int id)
        {
            _unitOfWork.GenericRepository<T>().Delete(id);

            return _unitOfWork.SaveChanges();
        }

        public IEnumerable<T> GetAll()
        {
            return _unitOfWork.GenericRepository<T>().GetAll();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _unitOfWork.GenericRepository<T>().GetAllAsync();
        }

        public T GetById(int id)
        {
            return _unitOfWork.GenericRepository<T>().GetById(id);
        }

        public int Update(T entity)
        {
            _unitOfWork.GenericRepository<T>().Update(entity);
            return _unitOfWork.SaveChanges();
        }
    }
}
