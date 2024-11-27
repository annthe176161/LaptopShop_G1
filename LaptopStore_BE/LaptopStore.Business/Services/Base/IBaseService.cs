using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaptopStore.Business.Services.Base
{
    public interface IBaseService<T> where T : class
    {
        IEnumerable<T> GetAll();
        Task<IEnumerable<T>> GetAllAsync();
        T GetById(int id);
        int Add(T entity);
        int Update(T entity);
        int Delete(int id);
    }
}
