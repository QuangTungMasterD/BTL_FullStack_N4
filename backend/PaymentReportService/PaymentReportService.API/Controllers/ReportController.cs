using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaymentReportService.API.DTOs;
using PaymentReportService.API.Helpers;
using PaymentReportService.API.Services;

namespace PaymentReportService.API.Controllers;

[ApiController]
[Route("api/reports")]
[Authorize(Roles = "Admin,GiaoVien")]
[Produces("application/json")]
public class ReportController : ControllerBase
{
    private readonly IReportService _reportService;

    public ReportController(IReportService reportService)
    {
        _reportService = reportService;
    }

    /// <summary>
    /// Báo cáo doanh thu
    /// </summary>
    [HttpGet("revenue")]
    public async Task<IActionResult> GetRevenueReport(
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        [FromQuery] int? courseId)
    {
        var filter = new ReportFilterRequest
        {
            FromDate = fromDate,
            ToDate = toDate,
            CourseId = courseId
        };

        var result = await _reportService.GetRevenueReportAsync(filter);

        return Ok(
            ApiResponse<RevenueReportResponse>.Ok(
                result,
                "Lấy báo cáo doanh thu thành công"
            )
        );
    }

    /// <summary>
    /// Báo cáo công nợ
    /// </summary>
    [HttpGet("debts")]
    public async Task<IActionResult> GetDebtReport(
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        [FromQuery] int? courseId)
    {
        var filter = new ReportFilterRequest
        {
            FromDate = fromDate,
            ToDate = toDate,
            CourseId = courseId
        };

        var result = await _reportService.GetDebtReportAsync(filter);

        return Ok(
            ApiResponse<DebtReportResponse>.Ok(
                result,
                "Lấy báo cáo công nợ thành công"
            )
        );
    }

    /// <summary>
    /// Thống kê thanh toán theo khóa học
    /// </summary>
    [HttpGet("courses")]
    public async Task<IActionResult> GetCoursePaymentReport()
    {
        var result = await _reportService.GetCoursePaymentReportAsync();

        return Ok(
            ApiResponse<List<CoursePaymentReportResponse>>.Ok(
                result,
                $"Có {result.Count} khóa học"
            )
        );
    }

    /// <summary>
    /// Dashboard tổng quan
    /// </summary>
    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboard()
    {
        var dashboard = await _reportService.GetDashboardAsync();

        return Ok(
            ApiResponse<DashboardResponse>.Ok(
                dashboard,
                "Lấy dashboard thành công"
            )
        );
    }
}