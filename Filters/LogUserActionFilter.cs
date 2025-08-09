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

            // Get user ID from JWT claims - use the custom "userId" claim
            var userIdClaim = context.HttpContext.User.FindFirst("userId");
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                try
                {
                    await _logService.CreateLogAsync(userId, endpoint ?? "Unknown");
                }
                catch (Exception ex)
                {
                    // Log the error but don't break the request
                    Console.WriteLine($"Error creating log: {ex.Message}");
                }
            }
            else
            {
                // Debug: Log when user ID claim is not found
                Console.WriteLine("User ID claim not found or invalid");
            }
        }
    }
}