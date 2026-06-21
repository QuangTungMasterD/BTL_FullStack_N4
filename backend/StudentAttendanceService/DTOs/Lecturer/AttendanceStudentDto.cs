namespace StudentAttendanceService.DTOs.Lecturer
{
    public class AttendanceStudentDto
    {
        public int Id { get; set; }
        public string StudentId { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Status { get; set; } = "present";
        public string CheckInTime { get; set; } = string.Empty;
        public string Note { get; set; } = string.Empty;
    }
}