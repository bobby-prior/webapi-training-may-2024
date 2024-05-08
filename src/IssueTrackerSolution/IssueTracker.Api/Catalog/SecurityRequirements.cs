using IssueTracker.Api.Shared;
using Marten;
using Microsoft.AspNetCore.Authorization;

namespace IssueTracker.Api.Catalog;

public class ShouldBeCreatorToAlterCatalogItemRequirement : IAuthorizationRequirement
{
}

public class
    ShouldBeCreatorOfCatalogItemRequirementHandler(IQuerySession session, IHttpContextAccessor httpContext, UserIdentityService userIdentityService) : AuthorizationHandler<ShouldBeCreatorToAlterCatalogItemRequirement>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, ShouldBeCreatorToAlterCatalogItemRequirement requirement)
    {

        if (httpContext.HttpContext is null)
        {
            return;
        }

        if (httpContext.HttpContext.Request.Method == "DELETE" || httpContext.HttpContext.Request.Method == "PUT")
        {
            if (httpContext.HttpContext.Request.RouteValues["id"] is string routeParamId)
            {
                if (Guid.TryParse(routeParamId, out Guid itemId))
                {
                    var userName = await userIdentityService.GetUserSubAsync();
                    var isUsers = await session.Query<CatalogItem>()
                        .Where(s => s.Id == itemId && s.AddedBy == userName).CountAsync() == 1;

                    if (isUsers)
                    {
                        context.Succeed(requirement);

                    }
                    else
                    {

                        return;
                    }

                }

            }
            else
            {
                return;
            }
        }
        else
        {
            context.Succeed(requirement);
        }

    }
}