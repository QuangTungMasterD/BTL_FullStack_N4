namespace PaymentReportService.API.DTOs;

// ===================== REQUEST =====================

public class CreateInvoiceRequest
{
    public int StudentId { get; set; }
    public int CourseId { get; set; }
    public int ClassId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public string StudentName { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }

    /// <summary>"TienMat" | "ChuyenKhoan" | "The"</summary>
    public string PaymentMethod { get; set; } = "TienMat";

    public string? Note { get; set; }
    public DateTime? DueDate { get; set; }
}

public class UpdatePaymentRequest
{
    /// <summary>Số tiền thanh toán thêm lần này</summary>
    public decimal AdditionalPayment { get; set; }

    /// <summary>"TienMat" | "ChuyenKhoan" | "The"</summary>
    public string PaymentMethod { get; set; } = "TienMat";

    public string? Note { get; set; }
}

// ===================== RESPONSE =====================

public class InvoiceResponse
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public int CourseId { get; set; }
    public int ClassId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal RemainingAmount => TotalAmount - PaidAmount;
    public string Status { get; set; } = string.Empty;
    public string PaymentMethod { get; set; } = string.Empty;
    public string? Note { get; set; }
    public DateTime InvoiceDate { get; set; }
    public DateTime? PaidDate { get; set; }
    public string CreatedByUserName { get; set; } = string.Empty;
}

public class DebtRecordResponse
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public int CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public decimal TotalPaid { get; set; }
    public decimal RemainingDebt { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime DueDate { get; set; }
    public DateTime UpdatedAt { get; set; }
}