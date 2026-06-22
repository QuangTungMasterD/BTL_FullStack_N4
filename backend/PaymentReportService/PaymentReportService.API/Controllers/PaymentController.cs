using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaymentReportService.API.DTOs;
using PaymentReportService.API.Helpers;
using PaymentReportService.API.Services;

namespace PaymentReportService.API.Controllers;

[ApiController]
[Route("api/payments")]
[Authorize]
[Produces("application/json")]
public class PaymentController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    public PaymentController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    // ===================== INVOICES =====================

    /// <summary>
    /// Lấy tất cả phiếu thu. Admin và GiaoVien.
    /// </summary>
    [HttpGet("invoices")]
    [Authorize(Roles = "Admin,Lecturer")]
    public async Task<IActionResult> GetAllInvoices()
    {
        var invoices = await _paymentService.GetAllInvoicesAsync();
        return Ok(ApiResponse<List<InvoiceResponse>>.Ok(invoices, $"Có {invoices.Count} phiếu thu"));
    }

    /// <summary>
    /// Lấy 1 phiếu thu theo ID
    /// </summary>
    [HttpGet("invoices/{id:int}")]
    public async Task<IActionResult> GetInvoiceById(int id)
    {
        var invoice = await _paymentService.GetInvoiceByIdAsync(id);
        if (invoice == null)
            return NotFound(ApiResponse<string>.Fail($"Không tìm thấy phiếu thu #{id}"));

        return Ok(ApiResponse<InvoiceResponse>.Ok(invoice));
    }

    /// <summary>
    /// Lấy phiếu thu theo học viên
    /// </summary>
    [HttpGet("invoices/student/{studentId:int}")]
    public async Task<IActionResult> GetInvoicesByStudent(int studentId)
    {
        var invoices = await _paymentService.GetInvoicesByStudentAsync(studentId);
        return Ok(ApiResponse<List<InvoiceResponse>>.Ok(invoices, $"Học viên #{studentId} có {invoices.Count} phiếu"));
    }

    /// <summary>
    /// Lấy phiếu thu theo khóa học
    /// </summary>
    [HttpGet("invoices/course/{courseId:int}")]
    [Authorize(Roles = "ADMIN,LECTURER")]
    public async Task<IActionResult> GetInvoicesByCourse(int courseId)
    {
        var invoices = await _paymentService.GetInvoicesByCourseAsync(courseId);
        return Ok(ApiResponse<List<InvoiceResponse>>.Ok(invoices, $"Khóa #{courseId} có {invoices.Count} phiếu"));
    }

    /// <summary>
    /// Lọc phiếu thu theo ngày, khóa học, trạng thái
    /// </summary>
    [HttpGet("invoices/filter")]
    [Authorize(Roles = "ADMIN,LECTURER")]
    public async Task<IActionResult> GetFilteredInvoices(
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        [FromQuery] int? courseId,
        [FromQuery] string? status)
    {
        var filter = new ReportFilterRequest
        {
            FromDate = fromDate,
            ToDate = toDate,
            CourseId = courseId,
            Status = status
        };

        var invoices = await _paymentService.GetInvoicesFilteredAsync(filter);
        return Ok(ApiResponse<List<InvoiceResponse>>.Ok(invoices, $"Tìm thấy {invoices.Count} phiếu"));
    }

    /// <summary>
    /// Tạo phiếu thu học phí mới. Chỉ Admin.
    /// </summary>
    [HttpPost("invoices")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> CreateInvoice([FromBody] CreateInvoiceRequest request)
    {
        // Lấy userId của người tạo từ JWT
        var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdStr, out var userId))
            return Unauthorized(ApiResponse<string>.Fail("Không xác định được người dùng"));

        var (success, message, invoice) = await _paymentService.CreateInvoiceAsync(request, userId);

        if (!success)
            return BadRequest(ApiResponse<string>.Fail(message));

        return CreatedAtAction(
            nameof(GetInvoiceById),
            new { id = invoice!.Id },
            ApiResponse<InvoiceResponse>.Ok(invoice, message)
        );
    }

    /// <summary>
    /// Ghi nhận thanh toán thêm cho phiếu đã có. Chỉ Admin.
    /// </summary>
    [HttpPatch("invoices/{id:int}/pay")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> AddPayment(int id, [FromBody] UpdatePaymentRequest request)
    {
        var (success, message, invoice) = await _paymentService.AddPaymentAsync(id, request);

        if (!success)
            return BadRequest(ApiResponse<string>.Fail(message));

        return Ok(ApiResponse<InvoiceResponse>.Ok(invoice!, message));
    }

    // ===================== DEBTS =====================

    /// <summary>
    /// Lấy tất cả công nợ. Admin và GiaoVien.
    /// </summary>
    [HttpGet("debts")]
    [Authorize(Roles = "ADMIN,LECTURER")]
    public async Task<IActionResult> GetAllDebts()
    {
        var debts = await _paymentService.GetAllDebtsAsync();
        return Ok(ApiResponse<List<DebtRecordResponse>>.Ok(debts, $"Có {debts.Count} bản ghi công nợ"));
    }

    /// <summary>
    /// Lấy công nợ của 1 học viên
    /// </summary>
    [HttpGet("debts/student/{studentId:int}")]
    public async Task<IActionResult> GetDebtsByStudent(int studentId)
    {
        var debts = await _paymentService.GetDebtsByStudentAsync(studentId);
        return Ok(ApiResponse<List<DebtRecordResponse>>.Ok(debts, $"Học viên #{studentId} có {debts.Count} khoản nợ"));
    }

    /// <summary>
    /// Lấy danh sách công nợ quá hạn. Admin.
    /// </summary>
    [HttpGet("debts/overdue")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> GetOverdueDebts()
    {
        var debts = await _paymentService.GetOverdueDebtsAsync();
        return Ok(ApiResponse<List<DebtRecordResponse>>.Ok(debts, $"Có {debts.Count} khoản nợ quá hạn"));
    }
}