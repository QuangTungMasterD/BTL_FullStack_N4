namespace PaymentReportService.API.Models;

/// <summary>
/// Phiếu thu học phí — ghi nhận 1 lần thanh toán của học viên
/// </summary>
public class PaymentInvoice
{
    public int Id { get; set; }

    /// <summary>Id học viên (tham chiếu sang StudentDB của N2)</summary>
    public int StudentId { get; set; }

    /// <summary>Id khóa học (tham chiếu sang CourseDB của N1)</summary>
    public int CourseId { get; set; }

    /// <summary>Id lớp học (tham chiếu sang CourseDB của N1)</summary>
    public int ClassId { get; set; }

    /// <summary>Tên khóa học (lưu snapshot để báo cáo không bị mất khi xóa)</summary>
    public string CourseName { get; set; } = string.Empty;

    /// <summary>Tên học viên (snapshot)</summary>
    public string StudentName { get; set; } = string.Empty;

    /// <summary>Tổng học phí cần đóng</summary>
    public decimal TotalAmount { get; set; }

    /// <summary>Số tiền đã thanh toán lần này</summary>
    public decimal PaidAmount { get; set; }

    /// <summary>Trạng thái: "ChuaThanhToan" | "ThanhToanMot Phan" | "DaThanhToan"</summary>
    public string Status { get; set; } = "ChuaThanhToan";

    /// <summary>Hình thức: "TienMat" | "ChuyenKhoan" | "The"</summary>
    public string PaymentMethod { get; set; } = "TienMat";

    public string? Note { get; set; }

    public DateTime InvoiceDate { get; set; } = DateTime.UtcNow;

    public DateTime? PaidDate { get; set; }

    /// <summary>Nhân viên/Admin tạo phiếu thu</summary>
    public int CreatedByUserId { get; set; }
    public User? CreatedByUser { get; set; }

    // Navigation
    public ICollection<DebtRecord> DebtRecords { get; set; } = new List<DebtRecord>();
}