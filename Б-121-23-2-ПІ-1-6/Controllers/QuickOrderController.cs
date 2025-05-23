using AdvertisingAgency.BLL.DTOs;
using AdvertisingAgency.BLL.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Б_121_23_2_ПІ_1_6.Models;

namespace Б_121_23_2_ПІ_1_6.Controllers
{
    [ApiController]
    [Route("api/quick-orders")]
    public class QuickOrderController : Controller
    {
        private readonly IQuickOrderService _quickOrderService;
        private readonly IMapper _mapper;

        public QuickOrderController(IQuickOrderService orderService, IMapper mapper)
        {
            _quickOrderService = orderService;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult<int>> Create([FromBody] QuickOrderCreateRequest req, CancellationToken ct)
        {
            var dto = _mapper.Map<CreateQuickOrderDto>(req);
            var id = await _quickOrderService.CreateAsync(dto, ct);
            return Created($"api/quick-orders/{id}", id);
        }

        [HttpGet("user/{customerName}")]
        public async Task<ActionResult> GetUserOrders(string customerName, CancellationToken ct)
        {
            var orders = await _quickOrderService.GetUserOrdersAsync(customerName, ct);
            return Ok(orders);
        }
    }
}
