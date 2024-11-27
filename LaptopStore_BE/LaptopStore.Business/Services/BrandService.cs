using LaptopStore.Business.DTOs;
using LaptopStore.Business.Services.Base;
using LaptopStore.Business.Services.Contracts;
using LaptopStore.Data.Contracts.Base;
using LaptopStore.Data.Entities;
using LaptopStore.Data.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LaptopStore.Business.Services
{
    public class BrandService : BaseService<Brand>, IBrandService
    {
        protected readonly IUnitOfWork _unitOfWork;
        public BrandService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // Thêm mới một thương hiệu
        public int Add(BrandDTO entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var brand = new Brand
            {
                BrandName = entity.BrandName,
                IsDeleted = entity.IsDeleted
            };

            _unitOfWork.BrandRepository.Add(brand);
            return _unitOfWork.SaveChanges();
        }

        // Lấy danh sách các thương hiệu dựa trên điều kiện isDeleted
        public async Task<IEnumerable<BrandDTO>> GetBrandsByConditionAsync(bool isDeleted)
        {
            var brands = await _unitOfWork.BrandRepository.GetAllAsync();
            return brands.Where(b => b.IsDeleted == isDeleted)
                         .Select(b => new BrandDTO
                         {
                             BrandID = b.BrandID,
                             BrandName = b.BrandName,
                             IsDeleted = b.IsDeleted
                         });
        }

        // Cập nhật thông tin thương hiệu
        public int Update(BrandDTO entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var brand = _unitOfWork.BrandRepository.GetById(entity.BrandID);
            if (brand == null)
                throw new KeyNotFoundException("Brand not found");

            brand.BrandName = entity.BrandName;
            brand.IsDeleted = entity.IsDeleted;

            _unitOfWork.BrandRepository.Update(brand);
            return _unitOfWork.SaveChanges();
        }

        // Lấy tất cả các thương hiệu
        public IEnumerable<BrandDTO> GetAll()
        {
            var brands = _unitOfWork.BrandRepository.GetAll();
            return brands.Select(b => new BrandDTO
            {
                BrandID = b.BrandID,
                BrandName = b.BrandName,
                IsDeleted = b.IsDeleted
            });
        }

        // Lấy tất cả các thương hiệu bất đồng bộ
        public async Task<IEnumerable<BrandDTO>> GetAllAsync()
        {
            var brands = await _unitOfWork.BrandRepository.GetAllAsync();
            return brands.Select(b => new BrandDTO
            {
                BrandID = b.BrandID,
                BrandName = b.BrandName,
                IsDeleted = b.IsDeleted
            });
        }

        // Lấy thương hiệu theo ID
        public BrandDTO GetById(int id)
        {
            var brand = _unitOfWork.BrandRepository.GetById(id);
            if (brand == null)
                throw new KeyNotFoundException("Brand not found");

            return new BrandDTO
            {
                BrandID = brand.BrandID,
                BrandName = brand.BrandName,
                IsDeleted = brand.IsDeleted
            };
        }
    }
}
