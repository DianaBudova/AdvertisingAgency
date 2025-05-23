using AdvertisingAgency.BLL.Interfaces;
using AdvertisingAgency.BLL.DTOs;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Б_121_23_2_ПІ_1_6.Models;
using System.Text.RegularExpressions;

namespace AdvertisingAgency.WebApi.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;

        public AuthController(IAuthService authService, IMapper mapper)
        {
            _authService = authService;
            _mapper = mapper;
        }

        [HttpPost("register")]
        public async Task<ActionResult<int>> Register([FromBody] RegisterRequest req, CancellationToken ct)
        {
            if (req is null)
                return BadRequest("Body is missing or malformed JSON.");

            if (!IsValidEmail(req.Email))
                return BadRequest("Invalid email format.");

            var dto = _mapper.Map<RegisterDto>(req);
            var id = await _authService.RegisterAsync(dto, ct);
            return Ok();
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResultDto>> Login([FromBody] LoginRequest req, CancellationToken ct)
        {
            var result = await _authService.LoginAsync(new LoginDto(req.Email, req.Password), ct);
            return Ok(result);
        }

        private bool IsValidEmail(string email)
        {
            var emailRegex = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, emailRegex);
        }
    }
}