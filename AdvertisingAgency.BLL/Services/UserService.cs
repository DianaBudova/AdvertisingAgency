namespace AdvertisingAgency.BLL.Services
{
    using AdvertisingAgency.BLL.DTOs;
    using AdvertisingAgency.BLL.Exceptions;
    using AdvertisingAgency.BLL.Interfaces;
    using AdvertisingAgency.DAL.Abstractions;
    using AdvertisingAgency.DAL.Entities;
    using AutoMapper;

    public class UserService : IUserService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public UserService(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<UserDto> GetAsync(int id, CancellationToken ct = default)
        {
            var entity = await _uow.Users.GetByIdAsync(id, ct) ??
                         throw new EntityNotFoundException(nameof(User), id);
            return _mapper.Map<UserDto>(entity);
        }
    }
}