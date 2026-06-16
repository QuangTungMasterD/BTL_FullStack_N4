using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaymentReportService.API.DTOs;
using PaymentReportService.API.Helpers;
using PaymentReportService.API.Services;

namespace PaymentReportService.API.Controllers;

[ApiController]
[Route("api/auth")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Đăng nhập — trả về JWT token.
    /// Demo accounts: admin/Password123, giaovien01/Password123, hocvien01/Password123
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            return BadRequest(ApiResponse<string>.Fail("Username và Password không được để trống"));

        var (success, message, response) = await _authService.LoginAsync(request);

        if (!success)
            return Unauthorized(ApiResponse<string>.Fail(message));

        return Ok(ApiResponse<LoginResponse>.Ok(response!, message));
    }

    /// <summary>
    /// Tạo tài khoản mới. Chỉ Admin mới được thực hiện.
    /// </summary>
    [HttpPost("register")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            return BadRequest(ApiResponse<string>.Fail("Username và Password không được để trống"));

        var (success, message, user) = await _authService.RegisterAsync(request);

        if (!success)
            return BadRequest(ApiResponse<string>.Fail(message));

        return CreatedAtAction(nameof(GetAllUsers), ApiResponse<UserResponse>.Ok(user!, message));
    }

    /// <summary>
    /// Lấy danh sách tất cả tài khoản. Chỉ Admin.
    /// </summary>
    [HttpGet("users")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _authService.GetAllUsersAsync();
        return Ok(ApiResponse<List<UserResponse>>.Ok(users, $"Có {users.Count} tài khoản"));
    }

    /// <summary>
    /// Lấy thông tin tài khoản đang đăng nhập
    /// </summary>
    [HttpGet("me")]
    [Authorize]
    public IActionResult GetMe()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var username = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;
        var fullName = User.FindFirst(System.Security.Claims.ClaimTypes.GivenName)?.Value;
        var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

        return Ok(ApiResponse<object>.Ok(new
        {
            UserId = userId,
            Username = username,
            FullName = fullName,
            Role = role
        }));
    }
}