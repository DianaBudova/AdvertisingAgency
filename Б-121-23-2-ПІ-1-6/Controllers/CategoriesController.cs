using AdvertisingAgency.BLL.Interfaces;
using AdvertisingAgency.BLL.DTOs;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Б_121_23_2_ПІ_1_6.Models;

namespace AdvertisingAgency.WebApi.Controllers
{
    [ApiController]
    [Route("api/categories")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly IMapper _mapper;

        public CategoriesController(ICategoryService categoryService, IMapper mapper)
        {
            _categoryService = categoryService;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult<int>> Create([FromBody] CategoryCreateRequest req, [FromHeader(Name = "X-User-Id")] int userId, CancellationToken ct)
        {
            var dto = new CreateCategoryDto(req.Name);
            var id = await _categoryService.CreateAsync(dto, userId, ct);
            return Created($"api/categories/{id}", id);
        }

        [HttpGet]
        public async Task<ActionResult> GetCategories(CancellationToken ct)
        {
            var categories = await _categoryService.GetCategoriesAsync(ct);
            return Ok(categories);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] CategoryUpdateRequest req, [FromHeader(Name = "X-User-Id")] int userId, CancellationToken ct)
        {
            var dto = _mapper.Map<UpdateCategoryDto>(req);
            await _categoryService.UpdateAsync(id, dto, userId, ct);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, [FromHeader(Name = "X-User-Id")] int userId, CancellationToken ct)
        {
            await _categoryService.DeleteAsync(id, userId, ct);
            return NoContent();
        }
    }
}