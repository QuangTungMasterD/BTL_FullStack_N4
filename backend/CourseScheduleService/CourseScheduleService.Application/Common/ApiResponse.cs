using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseScheduleService.Application.Common
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public String Message { get; set; } = String.Empty;
        public T? Data { get; set; }
        public object? Errors { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        // public ApiResponse(bool success, String msg, T? Data, List<String>? es, DateTime time)
        // {
        //     this.Success = success;
        //     this.Message = msg;
        //     this.Data = Data;
        //     this.Errors = es;
        //     this.Timestamp = time;
        // }

        public static ApiResponse<T> SuccessResponse(T data, string message = "Success")
        {
            return new ApiResponse<T>
            {
                Success = true,
                Message = message,
                Data = data,
                Timestamp = DateTime.Now
            };
        }

        public static ApiResponse<T> ErrorResponse(string message, object? errors = null)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = message,
                Errors = errors,
                Timestamp = DateTime.Now
            };
        }
    }
}