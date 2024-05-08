using IssueTracker.Api.Catalog;
using IssueTracker.Api.Shared;
using Marten;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace IssueTracker.Api.Issues;
[ApiExplorerSettings(GroupName = "Issues")]
public class Api(UserIdentityService userIdentityService, IDocumentSession session) : ControllerBase
{
    // POST /catalog/{id}/issues
    [HttpPost("/catalog/{catalogItemId:guid}/issues")]
    [SwaggerOperation(Tags = ["Issues", "Software Catalog"])]
    [Authorize]
    public async Task<ActionResult<UserIssueResponse>> AddAnIssueAsync(Guid catalogItemId, [FromBody] UserCreateIssueRequestModel request)
    {

        var softwareExists = await session.Query<CatalogItem>().Where(c => c.Id == catalogItemId).AnyAsync();
        if (!softwareExists)
        {
            return NotFound("No Software With That Id In The Catalog.");
        }
        var userInfo = await userIdentityService.GetUserInformationAsync();

        var fakeResponse = new UserIssueResponse
        {
            Id = Guid.NewGuid(),
            Status = IssueStatusType.Submitted,
            User = "/users/" + userInfo.Id,
            Software = new IssueSoftwareEmbeddedResponse(catalogItemId, "Fake Title", "Fake Description")
        };
        return Ok(fakeResponse);
    }
}

public record UserCreateIssueRequestModel(string Description);

public record UserIssueResponse
{
    public Guid Id { get; set; } // the created issue ID
    public string User { get; set; } = string.Empty; // The user id, or the route to the user...
    public IssueSoftwareEmbeddedResponse? Software { get; set; } // the id of the software, or the route
    public IssueStatusType Status { get; set; } = IssueStatusType.Submitted;
}

public record IssueSoftwareEmbeddedResponse(Guid Id, string Title, string Description);

public enum IssueStatusType { Submitted }
/* "id": 93939,
    "description": "Thing is busted!",
    "user": "/users/839893893",
    "software": "/catalog/3983989893",
    "status": "Submitted"
*/