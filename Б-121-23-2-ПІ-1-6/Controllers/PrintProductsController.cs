using AdvertisingAgency.BLL.Interfaces;
using AdvertisingAgency.BLL.DTOs;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Б_121_23_2_ПІ_1_6.Models;

namespace AdvertisingAgency.WebApi.Controllers
{
    [ApiController]
    [Route("api/print-products")]
    public class PrintProductsController : ControllerBase
    {
        private readonly IPrintProductService _printProductService;
        private readonly IMapper _mapper;

        public PrintProductsController(IPrintProductService printProductService, IMapper mapper)
        {
            _printProductService = printProductService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PrintProductResponse>>> GetAll([FromQuery] PrintProductRequest request, CancellationToken ct)
        {
            var filter = _mapper.Map<PrintProductFilterDto>(request);

            var items = await _printProductService.GetAllAsync(filter, ct);
            var response = _mapper.Map<IEnumerable<PrintProductResponse>>(items);

            return Ok(response);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<PrintProductResponse>> Get(int id, CancellationToken ct)
        {
            var item = await _printProductService.GetAsync(id, ct);
            var response = _mapper.Map<PrintProductResponse>(item);
            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult<int>> Create([FromBody] PrintProductCreateRequest req, [FromHeader(Name = "X-User-Id")] int userId, CancellationToken ct)
        {
            var dto = _mapper.Map<CreatePrintProductDto>(req);
            var newId = await _printProductService.CreateAsync(dto, userId, ct);
            return CreatedAtAction(nameof(Get), new { id = newId }, newId);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] PrintProductUpdateRequest req, [FromHeader(Name = "X-User-Id")] int userId, CancellationToken ct)
        {
            var dto = _mapper.Map<UpdatePrintProductDto>(req);
            await _printProductService.UpdateAsync(id, dto, userId, ct);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, [FromHeader(Name = "X-User-Id")] int userId, CancellationToken ct)
        {
            await _printProductService.DeleteAsync(id, userId, ct);
            return NoContent();
        }
    }
}