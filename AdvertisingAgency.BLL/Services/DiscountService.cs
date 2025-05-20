using AdvertisingAgency.BLL.DTOs;
using AdvertisingAgency.BLL.Interfaces;
using AdvertisingAgency.DAL.Abstractions;
using AdvertisingAgency.DAL.Entities;
using AutoMapper;

namespace AdvertisingAgency.BLL.Services
{
    public class DiscountService : IDiscountService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public DiscountService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<DiscountDto> GetAsync(int id, CancellationToken ct)
        {
            var discount = await _unitOfWork.Discounts.GetByIdWithIncludesAsync(id, ct);
            if (discount == null)
            {
                throw new KeyNotFoundException($"Discount with ID {id} not found");
            }
            return _mapper.Map<DiscountDto>(discount);
        }

        public async Task<IEnumerable<DiscountDto>> GetAllAsync(CancellationToken ct)
        {
            var discounts = await _unitOfWork.Discounts.GetAllAsync(ct);
            return _mapper.Map<IEnumerable<DiscountDto>>(discounts);
        }

        public async Task<int> CreateAsync(CreateDiscountDto dto, int userId, CancellationToken ct)
        {
            var discount = _mapper.Map<Discount>(dto);
            await _unitOfWork.Discounts.AddAsync(discount, ct);
            await _unitOfWork.SaveChangesAsync(ct);
            return discount.Id;
        }

        public async Task UpdateAsync(int id, UpdateDiscountDto dto, int userId, CancellationToken ct)
        {
            var discount = await _unitOfWork.Discounts.GetByIdAsync(id, ct);
            if (discount == null)
            {
                throw new KeyNotFoundException($"Discount with ID {id} not found");
            }

            _mapper.Map(dto, discount);
            await _unitOfWork.Discounts.UpdateAsync(discount, ct);
            await _unitOfWork.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(int id, int userId, CancellationToken ct)
        {
            var discount = await _unitOfWork.Discounts.GetByIdAsync(id, ct);
            if (discount == null)
            {
                throw new KeyNotFoundException($"Discount with ID {id} not found");
            }

            await _unitOfWork.Discounts.DeleteAsync(discount.Id, ct);
            await _unitOfWork.SaveChangesAsync(ct);
        }

        public async Task<IEnumerable<DiscountDto>> GetActiveDiscountsAsync(CancellationToken ct)
        {
            var now = DateTime.UtcNow;
            var discounts = await _unitOfWork.Discounts.GetAllAsync(ct);
            var activeDiscounts = discounts
                .Where(d => d.StartDate <= now && d.EndDate >= now)
                .ToList();

            return _mapper.Map<IEnumerable<DiscountDto>>(activeDiscounts);
        }
    }
}