using PaymentReportService.API.DTOs;
using PaymentReportService.API.Models;
using PaymentReportService.API.Repositories;

namespace PaymentReportService.API.Services;

public interface IPaymentService
{
    Task<(bool Success, string Message, InvoiceResponse? Invoice)> CreateInvoiceAsync(
        CreateInvoiceRequest request, int createdByUserId);
    Task<(bool Success, string Message, InvoiceResponse? Invoice)> AddPaymentAsync(
        int invoiceId, UpdatePaymentRequest request);
    Task<InvoiceResponse?> GetInvoiceByIdAsync(int id);
    Task<List<InvoiceResponse>> GetAllInvoicesAsync();
    Task<List<InvoiceResponse>> GetInvoicesByStudentAsync(int studentId);
    Task<List<InvoiceResponse>> GetInvoicesByCourseAsync(int courseId);
    Task<List<InvoiceResponse>> GetInvoicesFilteredAsync(ReportFilterRequest filter);
    Task<List<DebtRecordResponse>> GetAllDebtsAsync();
    Task<List<DebtRecordResponse>> GetDebtsByStudentAsync(int studentId);
    Task<List<DebtRecordResponse>> GetOverdueDebtsAsync();
}

public class PaymentService : IPaymentService
{
    private readonly IPaymentRepository _paymentRepo;

    public PaymentService(IPaymentRepository paymentRepo)
    {
        _paymentRepo = paymentRepo;
    }

    /// <summary>
    /// Tạo phiếu thu học phí mới.
    /// Đồng thời tạo hoặc cập nhật DebtRecord tương ứng.
    /// </summary>
    public async Task<(bool Success, string Message, InvoiceResponse? Invoice)> CreateInvoiceAsync(
        CreateInvoiceRequest request, int createdByUserId)
    {
        // Validate
        if (request.TotalAmount <= 0)
            return (false, "Học phí phải lớn hơn 0", null);
        if (request.PaidAmount < 0 || request.PaidAmount > request.TotalAmount)
            return (false, "Số tiền thanh toán không hợp lệ", null);

        // Xác định trạng thái
        string status = DetermineInvoiceStatus(request.PaidAmount, request.TotalAmount);

        var invoice = new PaymentInvoice
        {
            StudentId = request.StudentId,
            CourseId = request.CourseId,
            ClassId = request.ClassId,
            CourseName = request.CourseName,
            StudentName = request.StudentName,
            TotalAmount = request.TotalAmount,
            PaidAmount = request.PaidAmount,
            Status = status,
            PaymentMethod = request.PaymentMethod,
            Note = request.Note,
            InvoiceDate = DateTime.UtcNow,
            PaidDate = request.PaidAmount > 0 ? DateTime.UtcNow : null,
            CreatedByUserId = createdByUserId
        };

        var created = await _paymentRepo.CreateInvoiceAsync(invoice);

        // Tạo hoặc cập nhật DebtRecord
        await UpsertDebtRecordAsync(
            request.StudentId, request.StudentName,
            request.CourseId, request.CourseName,
            request.TotalAmount, request.PaidAmount,
            request.DueDate ?? DateTime.UtcNow.AddMonths(1),
            created.Id
        );

        return (true, "Tạo phiếu thu thành công", MapToInvoiceResponse(created));
    }

    /// <summary>
    /// Ghi nhận thanh toán thêm cho phiếu đã có
    /// </summary>
    public async Task<(bool Success, string Message, InvoiceResponse? Invoice)> AddPaymentAsync(
        int invoiceId, UpdatePaymentRequest request)
    {
        var invoice = await _paymentRepo.GetInvoiceByIdAsync(invoiceId);
        if (invoice == null)
            return (false, "Không tìm thấy phiếu thu", null);

        if (invoice.Status == "DaThanhToan")
            return (false, "Phiếu này đã thanh toán đủ", null);

        var remaining = invoice.TotalAmount - invoice.PaidAmount;
        if (request.AdditionalPayment <= 0 || request.AdditionalPayment > remaining)
            return (false, $"Số tiền thanh toán không hợp lệ. Còn phải đóng: {remaining:N0}đ", null);

        // Cập nhật invoice
        invoice.PaidAmount += request.AdditionalPayment;
        invoice.PaymentMethod = request.PaymentMethod;
        invoice.Note = request.Note;
        invoice.Status = DetermineInvoiceStatus(invoice.PaidAmount, invoice.TotalAmount);
        invoice.PaidDate = DateTime.UtcNow;

        var updated = await _paymentRepo.UpdateInvoiceAsync(invoice);

        // Cập nhật DebtRecord
        var debt = await _paymentRepo.GetDebtByStudentAndCourseAsync(invoice.StudentId, invoice.CourseId);
        if (debt != null)
        {
            debt.TotalPaid += request.AdditionalPayment;
            debt.Status = debt.TotalPaid >= debt.TotalAmount ? "DaThanhToan" : "ConNo";
            debt.UpdatedAt = DateTime.UtcNow;
            debt.LatestInvoiceId = invoiceId;
            await _paymentRepo.UpdateDebtAsync(debt);
        }

        return (true, "Cập nhật thanh toán thành công", MapToInvoiceResponse(updated));
    }

    public async Task<InvoiceResponse?> GetInvoiceByIdAsync(int id)
    {
        var invoice = await _paymentRepo.GetInvoiceByIdAsync(id);
        return invoice == null ? null : MapToInvoiceResponse(invoice);
    }

    public async Task<List<InvoiceResponse>> GetAllInvoicesAsync()
    {
        var invoices = await _paymentRepo.GetAllInvoicesAsync();
        return invoices.Select(MapToInvoiceResponse).ToList();
    }

    public async Task<List<InvoiceResponse>> GetInvoicesByStudentAsync(int studentId)
    {
        var invoices = await _paymentRepo.GetInvoicesByStudentAsync(studentId);
        return invoices.Select(MapToInvoiceResponse).ToList();
    }

    public async Task<List<InvoiceResponse>> GetInvoicesByCourseAsync(int courseId)
    {
        var invoices = await _paymentRepo.GetInvoicesByCourseAsync(courseId);
        return invoices.Select(MapToInvoiceResponse).ToList();
    }

    public async Task<List<InvoiceResponse>> GetInvoicesFilteredAsync(ReportFilterRequest filter)
    {
        var invoices = await _paymentRepo.GetInvoicesFilteredAsync(
            filter.FromDate, filter.ToDate, filter.CourseId, filter.Status);
        return invoices.Select(MapToInvoiceResponse).ToList();
    }

    public async Task<List<DebtRecordResponse>> GetAllDebtsAsync()
    {
        var debts = await _paymentRepo.GetAllDebtsAsync();
        return debts.Select(MapToDebtResponse).ToList();
    }

    public async Task<List<DebtRecordResponse>> GetDebtsByStudentAsync(int studentId)
    {
        var debts = await _paymentRepo.GetDebtsByStudentAsync(studentId);
        return debts.Select(MapToDebtResponse).ToList();
    }

    public async Task<List<DebtRecordResponse>> GetOverdueDebtsAsync()
    {
        var debts = await _paymentRepo.GetOverdueDebtsAsync();
        return debts.Select(MapToDebtResponse).ToList();
    }

    // ==================== PRIVATE HELPERS ====================

    private static string DetermineInvoiceStatus(decimal paid, decimal total)
    {
        if (paid <= 0) return "ChuaThanhToan";
        if (paid >= total) return "DaThanhToan";
        return "ThanhToanMotPhan";
    }

    private async Task UpsertDebtRecordAsync(
        int studentId, string studentName,
        int courseId, string courseName,
        decimal totalAmount, decimal paidAmount,
        DateTime dueDate, int invoiceId)
    {
        var existing = await _paymentRepo.GetDebtByStudentAndCourseAsync(studentId, courseId);

        if (existing == null)
        {
            // Tạo mới
            var debt = new DebtRecord
            {
                StudentId = studentId,
                StudentName = studentName,
                CourseId = courseId,
                CourseName = courseName,
                TotalAmount = totalAmount,
                TotalPaid = paidAmount,
                Status = paidAmount >= totalAmount ? "DaThanhToan" : "ConNo",
                DueDate = dueDate,
                UpdatedAt = DateTime.UtcNow,
                LatestInvoiceId = invoiceId,
                UserId = studentId // gắn userId tương ứng
            };
            await _paymentRepo.CreateDebtAsync(debt);
        }
        else
        {
            // Cộng dồn
            existing.TotalPaid += paidAmount;
            existing.Status = existing.TotalPaid >= existing.TotalAmount ? "DaThanhToan" : "ConNo";
            existing.UpdatedAt = DateTime.UtcNow;
            existing.LatestInvoiceId = invoiceId;
            await _paymentRepo.UpdateDebtAsync(existing);
        }
    }

    private static InvoiceResponse MapToInvoiceResponse(PaymentInvoice p)
        => new()
        {
            Id = p.Id,
            StudentId = p.StudentId,
            StudentName = p.StudentName,
            CourseId = p.CourseId,
            ClassId = p.ClassId,
            CourseName = p.CourseName,
            TotalAmount = p.TotalAmount,
            PaidAmount = p.PaidAmount,
            Status = p.Status,
            PaymentMethod = p.PaymentMethod,
            Note = p.Note,
            InvoiceDate = p.InvoiceDate,
            PaidDate = p.PaidDate,
            CreatedByUserName = p.CreatedByUser?.FullName ?? ""
        };

    private static DebtRecordResponse MapToDebtResponse(DebtRecord d)
        => new()
        {
            Id = d.Id,
            StudentId = d.StudentId,
            StudentName = d.StudentName,
            CourseId = d.CourseId,
            CourseName = d.CourseName,
            TotalAmount = d.TotalAmount,
            TotalPaid = d.TotalPaid,
            RemainingDebt = d.RemainingDebt,
            Status = d.Status,
            DueDate = d.DueDate,
            UpdatedAt = d.UpdatedAt
        };
}