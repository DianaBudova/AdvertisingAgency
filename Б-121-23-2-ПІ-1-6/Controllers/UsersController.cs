using AdvertisingAgency.BLL.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AdvertisingAgency.WebApi.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public UsersController(IUserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }

        [HttpGet("{userId:int}")]
        public async Task<ActionResult> GetUserOrders(int userId, CancellationToken ct)
        {
            var orders = await _userService.GetAsync(userId, ct);
            return Ok(orders);
        }
    }
}