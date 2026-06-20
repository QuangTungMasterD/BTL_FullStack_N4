using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AuthService.DTOs;
using AuthService.Models;
using AuthService.Services;
using AuthService.Data;  // THÊM DÒNG NÀY

namespace AuthService.Controllers
{
    [ApiController]
    // [Route("api/[controller]")]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly AppDbContext _db;

        public AuthController(IAuthService authService, AppDbContext db)
        {
            _authService = authService;
            _db = db;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _db.Users
                .FirstOrDefaultAsync(u => u.Email == request.Email && u.Password == request.Password);
            
            if (user == null)
                return Unauthorized(new { message = "Invalid email or password" });

            var token = _authService.GenerateToken(user.Id, user.Email, user.FullName, user.Role);

            var response = new LoginResponseDto
            {
                Token = token,
                UserId = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                Role = user.Role,
                StudentId = user.StudentId,
                Phone = user.Phone,
                Faculty = user.Faculty,
                Major = user.Major,
                Class = user.Class,
                LecturerId = user.LecturerId,
                Title = user.Title
            };

            return Ok(response);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingUser = await _db.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (existingUser != null)
                return BadRequest(new { message = "Email already registered" });

            var user = new User
            {
                Email = request.Email,
                Password = request.Password,
                FullName = request.FullName,
                Role = "STUDENT",
                StudentId = request.StudentId,
                Phone = request.Phone,
                Faculty = request.Faculty,
                Major = request.Major,
                Class = request.Class,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            var response = new RegisterResponseDto
            {
                UserId = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role,
                Message = "Registration successful! Please login."
            };

            return Ok(response);
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                return Unauthorized(new { message = "Invalid token" });

            var user = await _db.Users.FindAsync(userId);
            if (user == null)
                return NotFound(new { message = "User not found" });

            var response = new UserInfoDto
            {
                UserId = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                Role = user.Role,
                StudentId = user.StudentId,
                Phone = user.Phone,
                Faculty = user.Faculty,
                Major = user.Major,
                Class = user.Class,
                LecturerId = user.LecturerId,
                Title = user.Title
            };

            return Ok(response);
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            if (string.IsNullOrEmpty(request.Token))
                return BadRequest("Token required");

            var defaultUser = await _db.Users.FirstOrDefaultAsync();
            if (defaultUser == null) 
                return BadRequest("No users configured");
            
            var newToken = _authService.GenerateToken(defaultUser.Id, defaultUser.Email, defaultUser.FullName, defaultUser.Role);
            return Ok(new { token = newToken });
        }
    }

    public class RefreshTokenRequest
    {
        public string Token { get; set; } = string.Empty;
    }
}