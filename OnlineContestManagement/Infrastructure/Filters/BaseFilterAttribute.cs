using Microsoft.AspNetCore.Mvc.Filters;

namespace OnlineContestManagement.Infrastructure.Filters
{
    public abstract class BaseFilterAttribute : IAsyncActionFilter
    {
        private IAsyncActionFilter _asyncActionFilterImplementation;

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // Add common filter logic here if needed
            await next();
        }
    }
}