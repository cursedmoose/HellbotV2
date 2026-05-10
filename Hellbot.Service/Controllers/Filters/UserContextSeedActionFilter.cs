using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Hellbot.Service.Controllers.Filters
{
    public sealed class UserContextSeedActionFilter : IAsyncActionFilter
    {
        public Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!ShouldApply(context))
                return next();

            var reject = UserContextSeeder.TryParseQuery(
                context.HttpContext.Request.Query,
                out var seedContext);

            if (reject is not null)
            {
                context.Result = new BadRequestObjectResult(reject);
                return Task.CompletedTask;
            }

            if (seedContext.HasValue)
                context.HttpContext.Items[UserContextSeeder.PendingContextItemKey] = seedContext.Value;

            return next();
        }

        private static bool ShouldApply(ActionExecutingContext context)
        {
            if (context.ActionDescriptor.EndpointMetadata.OfType<IncludeUserContextAttribute>().Any())
                return true;

            return context.ActionDescriptor is ControllerActionDescriptor cad
                   && (
                       Attribute.IsDefined(cad.ControllerTypeInfo, typeof(IncludeUserContextAttribute), inherit: true)
                       || Attribute.IsDefined(cad.MethodInfo, typeof(IncludeUserContextAttribute), inherit: false)
                   );
        }
    }
}
