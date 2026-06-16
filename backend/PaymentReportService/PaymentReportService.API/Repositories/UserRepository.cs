using Microsoft.EntityFrameworkCore;
using PaymentReportService.API.Data;
using PaymentReportService.API.Models;

namespace PaymentReportService.API.Repositories;

public class UserRepository : IUserRepository
{
    private readonly PaymentDbContext _db;

    public UserRepository(PaymentDbContext db)
    {
        _db = db;
    }

    public async Task<User?> GetByUsernameAsync(string username)
        => await _db.Users.FirstOrDefaultAsync(u => u.Username == username && u.IsActive);

    public async Task<User?> GetByIdAsync(int id)
        => await _db.Users.FindAsync(id);

    public async Task<List<User>> GetAllAsync()
        => await _db.Users.Where(u => u.IsActive).OrderBy(u => u.FullName).ToListAsync();

    public async Task<User> CreateAsync(User user)
    {
        _db.Users.Add(user);
        await _db.SaveChangesAsync();
        return user;
    }

    public async Task<bool> UsernameExistsAsync(string username)
        => await _db.Users.AnyAsync(u => u.Username == username);
}