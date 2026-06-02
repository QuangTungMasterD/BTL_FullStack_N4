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
    public int StatusCode { get; set; }
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

    public static ApiResponse<T> SuccessResponse(T data, string message = "Success", int statusCode = 200)
    {
      return new ApiResponse<T>
      {
        Success = true,
        Message = message,
        Data = data,
        StatusCode = statusCode,
        Timestamp = DateTime.Now
      };
    }

    public static ApiResponse<T> ErrorResponse(string message, object? errors = null, int statusCode = 404)
    {
      return new ApiResponse<T>
      {
        Success = false,
        Message = message,
        Errors = errors,
        StatusCode = statusCode,
        Timestamp = DateTime.Now
      };
    }
  }
}