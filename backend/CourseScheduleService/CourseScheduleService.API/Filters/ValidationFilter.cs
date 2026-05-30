using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CourseScheduleService.Application.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CourseScheduleService.API.Filters
{
    public class ValidationFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {

                var errors = context.ModelState.Values
                    .SelectMany(x => x.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                
                context.Result = new BadRequestObjectResult(
                    ApiResponse<String>.ErrorResponse(
                        "Dữ liệu không hợp lệ", 
                        errors
                    )
                );
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // No-op
        }
    }
}
