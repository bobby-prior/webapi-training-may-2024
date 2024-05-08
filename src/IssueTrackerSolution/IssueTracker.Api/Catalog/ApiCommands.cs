using FluentValidation;
using IssueTracker.Api.Shared;
using Marten;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;

namespace IssueTracker.Api.Catalog;

[Authorize(Policy = "IsSoftwareAdmin")]
[Route("/catalog")]
[Produces("application/json")]

[ApiExplorerSettings(GroupName = "Software Catalog")]


public class ApiCommands(IValidator<CreateCatalogItemRequest> validator, IDocumentSession session) : ControllerBase
{

    /// <summary>
    /// Add an Item to the Software Catalog
    /// </summary>
    /// <param name="request"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    /// <response code="201">The new software item</response>
    /// <response code="400">A application/problems+json response</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [SwaggerOperation(Tags = ["Software Catalog"], OperationId = "AddCatalog")]
    public async Task<ActionResult<CatalogItemResponse>> AddACatalogItemAsync(
       [FromBody] CreateCatalogItemRequest request,
      [FromServices] UserIdentityService userIdentityService,
       CancellationToken token)
    {
        var userId = await userIdentityService.GetUserSubAsync();

        var validation = await validator.ValidateAsync(request, token);
        if (!validation.IsValid)
        {
            return this.CreateProblemDetailsForModelValidation("Cannot Add Catalog Item", validation.ToDictionary());
        }

        var entityToSave = request.MapToCatalogItem(userId);


        session.Store(entityToSave);
        await session.SaveChangesAsync(token); // Do the actual work!


        var response = entityToSave.MapToResponse();
        return CreatedAtRoute("catalog#get-by-id", new { id = response.Id }, response);
        // part of this collection. 
    }
    [HttpDelete("{id:guid}")]
    [SwaggerOperation(Tags = ["Software Catalog"])]
    public async Task<ActionResult> RemoveCatalogItemAsync(Guid id, CancellationToken token)
    {

        // see if the thing exists.
        var storedItem = await session.LoadAsync<CatalogItem>(id, token);
        if (storedItem != null)
        {
            var user = this.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier);

            storedItem.RemovedAt = DateTimeOffset.Now;
            session.Store(storedItem); // "Upsert"
            await session.SaveChangesAsync(token); // save it.
        }
        return NoContent();
    }

    // PUT /catalog/38798398938983
    [HttpPut("{id:guid}")]
    [SwaggerOperation(Tags = ["Software Catalog"], OperationId = "Replace")]
    public async Task<ActionResult> ReplaceCatalogItemAsync(Guid id, [FromBody] ReplaceCatalogItemRequest request, CancellationToken token)
    {
        var item = await session.LoadAsync<CatalogItem>(id);

        if (item is null)
        {
            return NotFound(); // or do an upsert?
        }
        // I'd also validate the id in the request matches the route id, but you do you.
        if (id != request.Id)
        {
            return BadRequest("Ids don't match");
        }
        item.Title = request.Title;
        item.Description = request.Description;
        session.Store(item);
        await session.SaveChangesAsync(token);
        return Ok();

    }

}

