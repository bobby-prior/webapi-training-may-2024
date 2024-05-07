using FluentValidation;
using Marten;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IssueTracker.Api.Catalog;
[Authorize]
[Route("/catalog")]
public class ApiQueries(IQuerySession session) : ControllerBase
{

    [HttpGet]
    [ResponseCache(Duration = 5, Location = ResponseCacheLocation.Client)]
    public async Task<ActionResult> GetAllCatalogItemsAsync(CancellationToken token)
    {
        var data = await session.Query<CatalogItem>()
            .Where(c => c.RemovedAt == null)
             .Select(c => new CatalogItemResponse(c.Id, c.Title, c.Description))
            .ToListAsync(token);
        return Ok(new { data });
    }



    [HttpGet("{id:guid}", Name = "catalog#get-by-id")]
    public async Task<ActionResult> GetCatalogItemByIdAsync(Guid id, CancellationToken token)
    {
        var response = await session.Query<CatalogItem>()
            .Where(c => c.Id == id && c.RemovedAt == null)
            .Select(c => new CatalogItemResponse(c.Id, c.Title, c.Description))
            .SingleOrDefaultAsync(token);

        if (response is null)
        {
            return NotFound();
        }
        else
        {
            return Ok(response);
        }
    }


}

