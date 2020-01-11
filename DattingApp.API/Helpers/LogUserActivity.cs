using System;
using System.Security.Claims;
using System.Threading.Tasks;
using DattingApp.API.Data;
using DattingApp.API.Model;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection; //To get rid of the GetService error.

namespace DattingApp.API.Helpers
{
    public class LogUserActivity : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(
            ActionExecutingContext context, //When the action is being executed
            ActionExecutionDelegate next) //After the action was executed
        {
            var resultContext = await next(); //to get the context after the action's been completed.
            int userId = int.Parse(resultContext.HttpContext.User
                        .FindFirst(ClaimTypes.NameIdentifier).Value);
            var repo = resultContext.HttpContext.RequestServices.GetService<IDatingRepository>();
            User user = await repo.GetUser(userId);
            user.LastActive = DateTime.Now;
            await repo.SaveAll();
        }
    }
}