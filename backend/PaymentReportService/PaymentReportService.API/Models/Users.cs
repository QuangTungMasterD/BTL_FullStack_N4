namespace PaymentReportService.API.Models;

/// <summary>
/// Tài khoản người dùng hệ thống (Admin, GiaoVien, HocVien)
/// </summary>
public class User
{
    public int Id { get; set; }

    public string Username { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Vai trò: "Admin" | "GiaoVien" | "HocVien"
    /// </summary>
    public string Role { get; set; } = "STUDENT, LECTURER";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public bool IsActive { get; set; } = true;

    // Navigation
    public ICollection<PaymentInvoice> Invoices { get; set; } = new List<PaymentInvoice>();
    public ICollection<DebtRecord> DebtRecords { get; set; } = new List<DebtRecord>();
}