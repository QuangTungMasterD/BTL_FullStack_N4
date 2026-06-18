namespace CourseScheduleService.Domain.Enums
{
    public enum RoomStatus
    {
        Available = 1,    // Có thể sử dụng
        Occupied = 2,     // Đang được sử dụng
        Maintenance = 3,  // Đang bảo trì
        Closed = 4        // Đóng cửa
    }
}