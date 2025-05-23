using AdvertisingAgency.BLL.Interfaces;
using AdvertisingAgency.BLL.DTOs;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Б_121_23_2_ПІ_1_6.Models;

namespace AdvertisingAgency.WebApi.Controllers
{
    [ApiController]
    [Route("api/services")]
    public class ServicesController : ControllerBase
    {
        private readonly IServiceService _serviceService;
        private readonly IMapper _mapper;

        public ServicesController(IServiceService serviceService, IMapper mapper)
        {
            _serviceService = serviceService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ServiceResponse>>> GetAll([FromQuery]ServiceRequest request, CancellationToken ct)
        {
            var filter = _mapper.Map<ServiceFilterDto>(request);

            var items = await _serviceService.GetAllAsync(filter, ct);
            var response = _mapper.Map<IEnumerable<ServiceResponse>>(items);

            return Ok(response);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ServiceResponse>> Get(int id, CancellationToken ct)
        {
            var item = await _serviceService.GetAsync(id, ct);
            var response = _mapper.Map<ServiceResponse>(item);
            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult<int>> Create([FromBody] ServiceCreateRequest req, [FromHeader(Name = "X-User-Id")] int userId, CancellationToken ct)
        {
            var dto = _mapper.Map<CreateServiceDto>(req);
            var newId = await _serviceService.CreateAsync(dto, userId, ct);
            return CreatedAtAction(nameof(Get), new { id = newId }, newId);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] ServiceUpdateRequest req, [FromHeader(Name = "X-User-Id")] int userId, CancellationToken ct)
        {
            var dto = _mapper.Map<UpdateServiceDto>(req);
            await _serviceService.UpdateAsync(id, dto, userId, ct);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, [FromHeader(Name = "X-User-Id")] int userId, CancellationToken ct)
        {
            await _serviceService.DeleteAsync(id, userId, ct);
            return NoContent();
        }
    }
}