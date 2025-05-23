using AdvertisingAgency.BLL.DTOs;
using AdvertisingAgency.BLL.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Б_121_23_2_ПІ_1_6.Models;

namespace Б_121_23_2_ПІ_1_6.Controllers
{
    [ApiController]
    [Route("api/discounts")]
    public class DiscountsController : ControllerBase
    {
        private readonly IDiscountService _discountService;
        private readonly IMapper _mapper;

        public DiscountsController(IDiscountService discountService, IMapper mapper)
        {
            _discountService = discountService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DiscountResponse>>> GetAll(CancellationToken ct)
        {
            var items = await _discountService.GetAllAsync(ct);
            var response = _mapper.Map<IEnumerable<DiscountResponse>>(items);
            return Ok(response);
        }

        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<DiscountResponse>>> GetActive(CancellationToken ct)
        {
            var items = await _discountService.GetActiveDiscountsAsync(ct);
            var response = _mapper.Map<IEnumerable<DiscountResponse>>(items);
            return Ok(response);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<DiscountResponse>> Get(int id, CancellationToken ct)
        {
            var item = await _discountService.GetAsync(id, ct);
            var response = _mapper.Map<DiscountResponse>(item);
            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult<int>> Create([FromBody] DiscountCreateRequest req, [FromHeader(Name = "X-User-Id")] int userId, CancellationToken ct)
        {
            var dto = _mapper.Map<CreateDiscountDto>(req);
            var newId = await _discountService.CreateAsync(dto, userId, ct);
            return CreatedAtAction(nameof(Get), new { id = newId }, newId);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] DiscountUpdateRequest req, [FromHeader(Name = "X-User-Id")] int userId, CancellationToken ct)
        {
            var dto = _mapper.Map<UpdateDiscountDto>(req);
            await _discountService.UpdateAsync(id, dto, userId, ct);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, [FromHeader(Name = "X-User-Id")] int userId, CancellationToken ct)
        {
            await _discountService.DeleteAsync(id, userId, ct);
            return NoContent();
        }
    }
}