namespace CourseScheduleService.Domain.Enums
{
    public enum ClassSessionStatus
    {
        Scheduled = 1,    // Đã lên lịch (chưa diễn ra)
        InProgress = 2,   // Đang diễn ra
        Completed = 3,    // Đã kết thúc
        Cancelled = 4,    // Đã hủy
    }
}