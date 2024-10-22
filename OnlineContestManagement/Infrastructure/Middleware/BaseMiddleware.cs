using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace OnlineContestManagement.Infrastructure
{
    public abstract class BaseMiddleware
    {
        private readonly RequestDelegate _next;
        
        public BaseMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        // Add common middleware logic here if needed
    }
}
