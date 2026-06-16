using PaymentReportService.API.Models;

namespace PaymentReportService.API.Repositories;

public interface IUserRepository
{
    Task<User?> GetByUsernameAsync(string username);
    Task<User?> GetByIdAsync(int id);
    Task<List<User>> GetAllAsync();
    Task<User> CreateAsync(User user);
    Task<bool> UsernameExistsAsync(string username);
}