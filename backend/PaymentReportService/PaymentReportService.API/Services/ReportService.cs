using Microsoft.EntityFrameworkCore;
using PaymentReportService.API.Data;
using PaymentReportService.API.DTOs;

namespace PaymentReportService.API.Services;

public interface IReportService
{
    Task<DashboardResponse> GetDashboardAsync();
    Task<CourseReportResponse> GetCourseReportAsync(int courseId);
    Task<List<CourseRevenueItem>> GetTopCoursesAsync(int top = 5);
    Task<List<MonthlyRevenueItem>> GetMonthlyRevenueAsync(int months = 12);
}

public class ReportService : IReportService
{
    private readonly PaymentDbContext _db;

    public ReportService(PaymentDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Dashboard tổng quan: doanh thu, số phiếu, công nợ, biểu đồ tháng
    /// </summary>
    public async Task<DashboardResponse> GetDashboardAsync()
    {
        var now = DateTime.UtcNow;

        var totalRevenue = await _db.PaymentInvoices.SumAsync(p => p.PaidAmount);

        var revenueThisMonth = await _db.PaymentInvoices
            .Where(p => p.PaidDate.HasValue
                        && p.PaidDate.Value.Year == now.Year
                        && p.PaidDate.Value.Month == now.Month)
            .SumAsync(p => p.PaidAmount);

        var totalInvoices = await _db.PaymentInvoices.CountAsync();
        var paidInvoices = await _db.PaymentInvoices.CountAsync(p => p.Status == "DaThanhToan");
        var unpaidInvoices = totalInvoices - paidInvoices;

        var totalDebt = await _db.DebtRecords
            .Where(d => d.Status == "ConNo")
            .SumAsync(d => d.TotalAmount - d.TotalPaid);

        var studentsWithDebt = await _db.DebtRecords
            .Where(d => d.Status == "ConNo")
            .Select(d => d.StudentId)
            .Distinct()
            .CountAsync();

        var monthlyRevenue = await GetMonthlyRevenueAsync(12);
        var topCourses = await GetTopCoursesAsync(5);

        return new DashboardResponse
        {
            TotalRevenue = totalRevenue,
            RevenueThisMonth = revenueThisMonth,
            TotalInvoices = totalInvoices,
            PaidInvoices = paidInvoices,
            UnpaidInvoices = unpaidInvoices,
            TotalDebt = totalDebt,
            StudentsWithDebt = studentsWithDebt,
            MonthlyRevenue = monthlyRevenue,
            TopCourses = topCourses
        };
    }

    /// <summary>
    /// Báo cáo chi tiết theo 1 khóa học: tổng thu, số học viên, danh sách phiếu
    /// </summary>
    public async Task<CourseReportResponse> GetCourseReportAsync(int courseId)
    {
        var invoices = await _db.PaymentInvoices
            .Include(p => p.CreatedByUser)
            .Where(p => p.CourseId == courseId)
            .ToListAsync();

        if (!invoices.Any())
        {
            return new CourseReportResponse
            {
                CourseId = courseId,
                CourseName = "Không tìm thấy dữ liệu"
            };
        }

        var courseName = invoices.First().CourseName;
        var totalExpected = invoices.Sum(p => p.TotalAmount);
        var totalCollected = invoices.Sum(p => p.PaidAmount);
        var studentIds = invoices.Select(p => p.StudentId).Distinct().ToList();
        var paidStudentIds = invoices
            .Where(p => p.Status == "DaThanhToan")
            .Select(p => p.StudentId).Distinct().ToList();

        return new CourseReportResponse
        {
            CourseId = courseId,
            CourseName = courseName,
            TotalExpected = totalExpected,
            TotalCollected = totalCollected,
            TotalRemaining = totalExpected - totalCollected,
            TotalStudents = studentIds.Count,
            PaidStudents = paidStudentIds.Count,
            UnpaidStudents = studentIds.Count - paidStudentIds.Count,
            Invoices = invoices.Select(p => new InvoiceResponse
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
            }).ToList()
        };
    }

    /// <summary>
    /// Top N khóa học theo doanh thu
    /// </summary>
    public async Task<List<CourseRevenueItem>> GetTopCoursesAsync(int top = 5)
    {
        return await _db.PaymentInvoices
            .GroupBy(p => new { p.CourseId, p.CourseName })
            .Select(g => new CourseRevenueItem
            {
                CourseId = g.Key.CourseId,
                CourseName = g.Key.CourseName,
                TotalRevenue = g.Sum(p => p.PaidAmount),
                StudentCount = g.Select(p => p.StudentId).Distinct().Count(),
                InvoiceCount = g.Count()
            })
            .OrderByDescending(x => x.TotalRevenue)
            .Take(top)
            .ToListAsync();
    }

    /// <summary>
    /// Doanh thu theo tháng (N tháng gần nhất)
    /// </summary>
    public async Task<List<MonthlyRevenueItem>> GetMonthlyRevenueAsync(int months = 12)
    {
        var now = DateTime.UtcNow;
        var startDate = now.AddMonths(-months + 1);
        var startFirstDay = new DateTime(startDate.Year, startDate.Month, 1, 0, 0, 0, DateTimeKind.Utc);

        var rawData = await _db.PaymentInvoices
            .Where(p => p.PaidDate.HasValue && p.PaidDate.Value >= startFirstDay)
            .GroupBy(p => new { p.PaidDate!.Value.Year, p.PaidDate.Value.Month })
            .Select(g => new
            {
                g.Key.Year,
                g.Key.Month,
                Revenue = g.Sum(p => p.PaidAmount),
                Count = g.Count()
            })
            .ToListAsync();

        // Tạo danh sách đầy đủ N tháng, kể cả tháng không có doanh thu
        var result = new List<MonthlyRevenueItem>();
        for (int i = months - 1; i >= 0; i--)
        {
            var date = now.AddMonths(-i);
            var match = rawData.FirstOrDefault(r => r.Year == date.Year && r.Month == date.Month);
            result.Add(new MonthlyRevenueItem
            {
                Year = date.Year,
                Month = date.Month,
                MonthLabel = $"T{date.Month}/{date.Year}",
                Revenue = match?.Revenue ?? 0,
                InvoiceCount = match?.Count ?? 0
            });
        }

        return result;
    }
}