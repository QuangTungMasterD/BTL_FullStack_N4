using PaymentReportService.API.Models;

namespace PaymentReportService.API.Repositories;

public interface IPaymentRepository
{
    // Invoice
    Task<PaymentInvoice?> GetInvoiceByIdAsync(int id);
    Task<List<PaymentInvoice>> GetAllInvoicesAsync();
    Task<List<PaymentInvoice>> GetInvoicesByStudentAsync(int studentId);
    Task<List<PaymentInvoice>> GetInvoicesByCourseAsync(int courseId);
    Task<List<PaymentInvoice>> GetInvoicesFilteredAsync(DateTime? from, DateTime? to, int? courseId, string? status);
    Task<PaymentInvoice> CreateInvoiceAsync(PaymentInvoice invoice);
    Task<PaymentInvoice> UpdateInvoiceAsync(PaymentInvoice invoice);

    // Debt
    Task<DebtRecord?> GetDebtByStudentAndCourseAsync(int studentId, int courseId);
    Task<List<DebtRecord>> GetDebtsByStudentAsync(int studentId);
    Task<List<DebtRecord>> GetAllDebtsAsync();
    Task<List<DebtRecord>> GetOverdueDebtsAsync();
    Task<DebtRecord> CreateDebtAsync(DebtRecord debt);
    Task<DebtRecord> UpdateDebtAsync(DebtRecord debt);

    // Report aggregation
    Task<decimal> GetTotalRevenueAsync();
    Task<decimal> GetRevenueThisMonthAsync();
    Task<int> GetPaidInvoicesCountAsync();
    Task<int> GetUnpaidInvoicesCountAsync();
    Task<decimal> GetTotalDebtAsync();
}