namespace PaymentReportService.API.DTOs;

// ===================== RESPONSE =====================

/// <summary>Dashboard tổng quan doanh thu</summary>
public class DashboardResponse
{
    /// <summary>Tổng doanh thu (tất cả thời gian)</summary>
    public decimal TotalRevenue { get; set; }

    /// <summary>Doanh thu tháng hiện tại</summary>
    public decimal RevenueThisMonth { get; set; }

    /// <summary>Tổng số phiếu thu</summary>
    public int TotalInvoices { get; set; }

    /// <summary>Số phiếu đã thanh toán đủ</summary>
    public int PaidInvoices { get; set; }

    /// <summary>Số phiếu còn nợ</summary>
    public int UnpaidInvoices { get; set; }

    /// <summary>Tổng công nợ chưa thu</summary>
    public decimal TotalDebt { get; set; }

    /// <summary>Số học viên còn nợ</summary>
    public int StudentsWithDebt { get; set; }

    /// <summary>Doanh thu theo tháng (12 tháng gần nhất)</summary>
    public List<MonthlyRevenueItem> MonthlyRevenue { get; set; } = new();

    /// <summary>Top khóa học theo doanh thu</summary>
    public List<CourseRevenueItem> TopCourses { get; set; } = new();
}

public class MonthlyRevenueItem
{
    public int Year { get; set; }
    public int Month { get; set; }
    public string MonthLabel { get; set; } = string.Empty; // "T1/2024"
    public decimal Revenue { get; set; }
    public int InvoiceCount { get; set; }
}

public class CourseRevenueItem
{
    public int CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public decimal TotalRevenue { get; set; }
    public int StudentCount { get; set; }
    public int InvoiceCount { get; set; }
}

/// <summary>Báo cáo chi tiết theo khóa học/lớp</summary>
public class CourseReportResponse
{
    public int CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public decimal TotalExpected { get; set; }    // Tổng học phí phải thu
    public decimal TotalCollected { get; set; }   // Đã thu được
    public decimal TotalRemaining { get; set; }   // Còn lại
    public int TotalStudents { get; set; }
    public int PaidStudents { get; set; }
    public int UnpaidStudents { get; set; }
    public List<InvoiceResponse> Invoices { get; set; } = new();
}

/// <summary>Tham số lọc báo cáo</summary>
public class ReportFilterRequest
{
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public int? CourseId { get; set; }
    public int? ClassId { get; set; }
    public string? Status { get; set; }
}

/// <summary>Wrapper phân trang chung</summary>
public class PagedResult<T>
{
    public List<T> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}