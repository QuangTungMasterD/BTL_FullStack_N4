using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CourseScheduleService.API.Middlewares
{
    public class HeaderClaimsMiddleware
    {
        private readonly RequestDelegate _next;

        public HeaderClaimsMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var userId = context.Request.Headers["X-User-Id"].FirstOrDefault();
            var rolesHeader = context.Request.Headers["X-Roles"].FirstOrDefault();

            if (!string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(rolesHeader))
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, userId),
                    new Claim(ClaimTypes.Name, userId)
                };

                // Thêm từng role
                var roles = rolesHeader.Split(',', StringSplitOptions.RemoveEmptyEntries);
                foreach (var role in roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role.Trim()));
                }

                var identity = new ClaimsIdentity(claims, "Gateway");
                context.User = new ClaimsPrincipal(identity);
            }

            await _next(context);
        }
    }
}