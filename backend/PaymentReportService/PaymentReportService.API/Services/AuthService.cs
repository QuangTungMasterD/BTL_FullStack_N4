using PaymentReportService.API.DTOs;
using PaymentReportService.API.Helpers;
using PaymentReportService.API.Models;
using PaymentReportService.API.Repositories;

namespace PaymentReportService.API.Services;

public interface IAuthService
{
    Task<(bool Success, string Message, LoginResponse? Response)> LoginAsync(LoginRequest request);
    Task<(bool Success, string Message, UserResponse? User)> RegisterAsync(RegisterRequest request);
    Task<List<UserResponse>> GetAllUsersAsync();
}

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepo;
    private readonly JwtHelper _jwtHelper;

    public AuthService(IUserRepository userRepo, JwtHelper jwtHelper)
    {
        _userRepo = userRepo;
        _jwtHelper = jwtHelper;
    }

    /// <summary>
    /// Đăng nhập: kiểm tra username/password, trả về JWT token
    /// </summary>
    public async Task<(bool Success, string Message, LoginResponse? Response)> LoginAsync(LoginRequest request)
    {
        // 1. Tìm user theo username
        var user = await _userRepo.GetByUsernameAsync(request.Username);
        if (user == null)
            return (false, "Tên đăng nhập không tồn tại", null);

        // 2. Kiểm tra password với BCrypt
        bool isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
        if (!isPasswordValid)
            return (false, "Mật khẩu không đúng", null);

        // 3. Tạo JWT token
        var token = _jwtHelper.GenerateToken(user);
        var expiresAt = _jwtHelper.GetTokenExpiry();

        var response = new LoginResponse
        {
            Token = token,
            Username = user.Username,
            FullName = user.FullName,
            Role = user.Role,
            ExpiresAt = expiresAt
        };

        return (true, "Đăng nhập thành công", response);
    }

    /// <summary>
    /// Đăng ký tài khoản mới (chỉ Admin được tạo)
    /// </summary>
    public async Task<(bool Success, string Message, UserResponse? User)> RegisterAsync(RegisterRequest request)
    {
        // Kiểm tra username trùng
        if (await _userRepo.UsernameExistsAsync(request.Username))
            return (false, $"Tên đăng nhập '{request.Username}' đã tồn tại", null);

        // Validate role
        var validRoles = new[] { "ADMIN", "LECTURER", "STUDENT" };
        if (!validRoles.Contains(request.Role))
            return (false, "Vai trò không hợp lệ. Chỉ chấp nhận: ADMIN, LECTURER, STUDENT", null);

        // Hash password
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var user = new User
        {
            Username = request.Username,
            PasswordHash = passwordHash,
            FullName = request.FullName,
            Email = request.Email,
            Role = request.Role,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        var created = await _userRepo.CreateAsync(user);

        var userResponse = new UserResponse
        {
            Id = created.Id,
            Username = created.Username,
            FullName = created.FullName,
            Email = created.Email,
            Role = created.Role,
            CreatedAt = created.CreatedAt,
            IsActive = created.IsActive
        };

        return (true, "Tạo tài khoản thành công", userResponse);
    }

    public async Task<List<UserResponse>> GetAllUsersAsync()
    {
        var users = await _userRepo.GetAllAsync();
        return users.Select(u => new UserResponse
        {
            Id = u.Id,
            Username = u.Username,
            FullName = u.FullName,
            Email = u.Email,
            Role = u.Role,
            CreatedAt = u.CreatedAt,
            IsActive = u.IsActive
        }).ToList();
    }
}