using Microsoft.EntityFrameworkCore;
using PaymentReportService.API.Data;
using PaymentReportService.API.Models;

namespace PaymentReportService.API.Repositories;

public class PaymentRepository : IPaymentRepository
{
    private readonly PaymentDbContext _db;

    public PaymentRepository(PaymentDbContext db)
    {
        _db = db;
    }

    // ==================== INVOICE ====================

    public async Task<PaymentInvoice?> GetInvoiceByIdAsync(int id)
        => await _db.PaymentInvoices
            .Include(p => p.CreatedByUser)
            .FirstOrDefaultAsync(p => p.Id == id);

    public async Task<List<PaymentInvoice>> GetAllInvoicesAsync()
        => await _db.PaymentInvoices
            .Include(p => p.CreatedByUser)
            .OrderByDescending(p => p.InvoiceDate)
            .ToListAsync();

    public async Task<List<PaymentInvoice>> GetInvoicesByStudentAsync(int studentId)
        => await _db.PaymentInvoices
            .Include(p => p.CreatedByUser)
            .Where(p => p.StudentId == studentId)
            .OrderByDescending(p => p.InvoiceDate)
            .ToListAsync();

    public async Task<List<PaymentInvoice>> GetInvoicesByCourseAsync(int courseId)
        => await _db.PaymentInvoices
            .Include(p => p.CreatedByUser)
            .Where(p => p.CourseId == courseId)
            .OrderByDescending(p => p.InvoiceDate)
            .ToListAsync();

    public async Task<List<PaymentInvoice>> GetInvoicesFilteredAsync(
        DateTime? from, DateTime? to, int? courseId, string? status)
    {
        var query = _db.PaymentInvoices.Include(p => p.CreatedByUser).AsQueryable();

        if (from.HasValue)
            query = query.Where(p => p.InvoiceDate >= from.Value);
        if (to.HasValue)
            query = query.Where(p => p.InvoiceDate <= to.Value);
        if (courseId.HasValue)
            query = query.Where(p => p.CourseId == courseId.Value);
        if (!string.IsNullOrEmpty(status))
            query = query.Where(p => p.Status == status);

        return await query.OrderByDescending(p => p.InvoiceDate).ToListAsync();
    }

    public async Task<PaymentInvoice> CreateInvoiceAsync(PaymentInvoice invoice)
    {
        _db.PaymentInvoices.Add(invoice);
        await _db.SaveChangesAsync();
        return invoice;
    }

    public async Task<PaymentInvoice> UpdateInvoiceAsync(PaymentInvoice invoice)
    {
        _db.PaymentInvoices.Update(invoice);
        await _db.SaveChangesAsync();
        return invoice;
    }

    // ==================== DEBT ====================

    public async Task<DebtRecord?> GetDebtByStudentAndCourseAsync(int studentId, int courseId)
        => await _db.DebtRecords
            .FirstOrDefaultAsync(d => d.StudentId == studentId && d.CourseId == courseId);

    public async Task<List<DebtRecord>> GetDebtsByStudentAsync(int studentId)
        => await _db.DebtRecords
            .Where(d => d.StudentId == studentId)
            .OrderByDescending(d => d.UpdatedAt)
            .ToListAsync();

    public async Task<List<DebtRecord>> GetAllDebtsAsync()
        => await _db.DebtRecords
            .OrderBy(d => d.Status)
            .ThenByDescending(d => d.UpdatedAt)
            .ToListAsync();

    public async Task<List<DebtRecord>> GetOverdueDebtsAsync()
    {
        var today = DateTime.UtcNow;
        return await _db.DebtRecords
            .Where(d => d.Status == "ConNo" && d.DueDate < today)
            .OrderBy(d => d.DueDate)
            .ToListAsync();
    }

    public async Task<DebtRecord> CreateDebtAsync(DebtRecord debt)
    {
        _db.DebtRecords.Add(debt);
        await _db.SaveChangesAsync();
        return debt;
    }

    public async Task<DebtRecord> UpdateDebtAsync(DebtRecord debt)
    {
        _db.DebtRecords.Update(debt);
        await _db.SaveChangesAsync();
        return debt;
    }

    // ==================== REPORT ====================

    public async Task<decimal> GetTotalRevenueAsync()
        => await _db.PaymentInvoices.SumAsync(p => p.PaidAmount);

    public async Task<decimal> GetRevenueThisMonthAsync()
    {
        var now = DateTime.UtcNow;
        return await _db.PaymentInvoices
            .Where(p => p.PaidDate.HasValue
                        && p.PaidDate.Value.Year == now.Year
                        && p.PaidDate.Value.Month == now.Month)
            .SumAsync(p => p.PaidAmount);
    }

    public async Task<int> GetPaidInvoicesCountAsync()
        => await _db.PaymentInvoices.CountAsync(p => p.Status == "DaThanhToan");

    public async Task<int> GetUnpaidInvoicesCountAsync()
        => await _db.PaymentInvoices.CountAsync(p => p.Status != "DaThanhToan");

    public async Task<decimal> GetTotalDebtAsync()
        => await _db.DebtRecords
            .Where(d => d.Status == "ConNo")
            .SumAsync(d => d.TotalAmount - d.TotalPaid);
}