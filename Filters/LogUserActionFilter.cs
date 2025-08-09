using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;
using DataVision.Services;

namespace DataVision.Filters
{
    public class LogUserActionFilter : IAsyncActionFilter
    {
        private readonly LogService _logService;

        public LogUserActionFilter(LogService logService)
        {
            _logService = logService;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var endpoint = context.HttpContext.Request.Path.Value;

            // Skip logging for login and register endpoints
            if (endpoint != null &&
                (endpoint.Contains("login", StringComparison.OrdinalIgnoreCase) ||
                 endpoint.Contains("register", StringComparison.OrdinalIgnoreCase)))
            {
                await next();
                return;
            }

            // Proceed with action execution
            var resultContext = await next();

            // Get user ID from JWT claims
            var userIdClaim = context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                await _logService.CreateLogAsync(userId, endpoint ?? "Unknown");
            }
        }
    }
}
