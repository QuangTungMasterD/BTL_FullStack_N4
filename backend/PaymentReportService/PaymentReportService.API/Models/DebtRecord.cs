namespace PaymentReportService.API.Models;

/// <summary>
/// Bản ghi công nợ — theo dõi số tiền còn nợ của từng học viên
/// Mỗi khi tạo invoice hoặc thanh toán, cập nhật bản ghi này
/// </summary>
public class DebtRecord
{
    public int Id { get; set; }

    /// <summary>Id học viên (tham chiếu sang StudentDB của N2)</summary>
    public int StudentId { get; set; }

    public string StudentName { get; set; } = string.Empty;

    /// <summary>Id khóa học</summary>
    public int CourseId { get; set; }

    public string CourseName { get; set; } = string.Empty;

    /// <summary>Tổng học phí khóa học</summary>
    public decimal TotalAmount { get; set; }

    /// <summary>Tổng đã thanh toán (cộng dồn)</summary>
    public decimal TotalPaid { get; set; }

    /// <summary>Còn nợ = TotalAmount - TotalPaid</summary>
    public decimal RemainingDebt => TotalAmount - TotalPaid;

    /// <summary>Trạng thái: "ConNo" | "DaThanhToan" | "QuaHan"</summary>
    public string Status { get; set; } = "ConNo";

    public DateTime DueDate { get; set; }

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public int? LatestInvoiceId { get; set; }
    public PaymentInvoice? LatestInvoice { get; set; }

    public int UserId { get; set; }
    public User? User { get; set; }
}