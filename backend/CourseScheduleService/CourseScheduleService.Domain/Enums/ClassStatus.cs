namespace CourseScheduleService.Domain.Enums
{
    public enum ClassStatus
    {
        Pending = 1,      // Chờ khai giảng
        Active = 2,       // Đang học
        Completed = 3,    // Đã kết thúc
        Cancelled = 4,    // Đã hủy
        Suspended = 5     // Tạm dừng
    }
}