using AdvertisingAgency.BLL.Interfaces;
using AdvertisingAgency.BLL.DTOs;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Б_121_23_2_ПІ_1_6.Models;

namespace AdvertisingAgency.WebApi.Controllers
{
    [ApiController]
    [Route("api/orders")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IMapper _mapper;

        public OrdersController(IOrderService orderService, IMapper mapper)
        {
            _orderService = orderService;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult<int>> Create([FromBody] OrderCreateRequest req, CancellationToken ct)
        {
            var itemsDto = req.Items.Select(item => new OrderItemDto(item.ServiceId, item.Quantity)).ToList();
            var dto = new CreateOrderDto(req.UserId, itemsDto);
            var id = await _orderService.CreateAsync(dto, ct);
            return Created($"api/orders/{id}", id);
        }

        [HttpGet("user/{userId:int}")]
        public async Task<ActionResult> GetUserOrders(int userId, CancellationToken ct)
        {
            var orders = await _orderService.GetUserOrdersAsync(userId, ct);
            return Ok(orders);
        }

        [HttpPatch("{orderId:int}/status/{status}")]
        public async Task<IActionResult> ChangeStatus(int orderId, string status, [FromHeader(Name = "X-User-Id")] int actorId, CancellationToken ct)
        {
            await _orderService.ChangeStatusAsync(orderId, status, actorId, ct);
            return NoContent();
        }
    }
}