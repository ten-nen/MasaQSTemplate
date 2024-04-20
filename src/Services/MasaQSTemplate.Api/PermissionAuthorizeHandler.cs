using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

public class PermissionAuthorizeHandler : AuthorizationHandler<DenyAnonymousAuthorizationRequirement>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, DenyAnonymousAuthorizationRequirement requirement)
    {
        DefaultHttpContext httpContext;
        if (context.Resource is DefaultHttpContext defaultContext) httpContext = defaultContext;
        else if (context.Resource is AuthorizationFilterContext mvcContext) httpContext = (DefaultHttpContext)mvcContext.HttpContext;
        else httpContext = null;
        if (httpContext != null && httpContext.User.Identity!.IsAuthenticated)
        {
            //if (context.User.IsInRole("admin"))
            //    context.Succeed(requirement);
            var currentUser = httpContext.RequestServices.GetRequiredService<IUserContext>();
            if (currentUser.GetUserRoles<string>().Any(x => x.Equals("admin")))
            {
                context.Succeed(requirement);
                return;
            }

            var isAuthenticated = httpContext.GetEndpoint()
                                            .Metadata
                                            .Any(x => x is AuthorizeAttribute && ((AuthorizeAttribute)x).Policy == null);
            if (isAuthenticated)
            {
                context.Succeed(requirement);
                return;
            }

            var handler = httpContext.GetEndpoint()?
                                        .Metadata
                                        .GetMetadata<ControllerActionDescriptor>();

            var eventBus = httpContext.RequestServices.GetRequiredService<IEventBus>();
            var userQueryEvent = new UserQuery(new UserQueryDto() { Id = currentUser.GetUserId<Guid>(), IncludePermissions = true });
            await eventBus.PublishAsync(userQueryEvent);
            var user = userQueryEvent.Result.Result.FirstOrDefault();

            var serviceName = handler?.ControllerName;
            var methodName = handler?.ActionName;

            if (user != null && user.Permissions.Any(x => x.Code.Equals($"{serviceName}.{methodName}", StringComparison.OrdinalIgnoreCase)))
            {
                context.Succeed(requirement);
                return;
            }

            context.Fail();
        }
    }
}