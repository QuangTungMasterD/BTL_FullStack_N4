using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using CourseScheduleService.Application.Common;

namespace CourseScheduleService.API.Middlewares
{
    public class ExceptionMiddleware
    {
            private readonly RequestDelegate _next;
        public ExceptionMiddleware(RequestDelegate next) => _next = next;

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = 500;
                context.Response.ContentType = "application/json";
                var response = ApiResponse<object>.ErrorResponse(ex.Message, null, 500);
                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            }
        }
    }
}