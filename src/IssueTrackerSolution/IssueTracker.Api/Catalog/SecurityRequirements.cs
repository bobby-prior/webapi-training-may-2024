using Marten;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace IssueTracker.Api.Catalog;

public class ShouldBeCreatorToAlterCatalogItemRequirement : IAuthorizationRequirement
{
}

public class
    ShouldBeCreatorOfCatalogItemRequirementHandler(IQuerySession session, IHttpContextAccessor httpContext) : AuthorizationHandler<ShouldBeCreatorToAlterCatalogItemRequirement>
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
                    var userId = context.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier);
                    var isUsers = await session.Query<CatalogItem>()
                        .Where(s => s.Id == itemId && s.AddedBy == userId.Value).CountAsync() == 1;

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