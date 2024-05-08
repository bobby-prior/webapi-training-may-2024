using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace IssueTracker.Api.Status;
/// <summary>
/// This is the API For the Status Stuff
/// </summary>
/// <param name="logger">Used to log some stuff</param>
[ApiExplorerSettings(GroupName = "Admin")]
public class Api(ILogger<Api> logger) : ControllerBase
{
    /// <summary>
    /// Use this to get the status of the API
    /// </summary>
    /// <param name="token">Cancellation Token</param>
    /// <returns>Some status stuff</returns>
    /// <response code="200">The status of the system, including the.... </response>
    [HttpGet("/status")]
    [Produces("application/json")]
    [SwaggerOperation(Tags = ["Admin"])]

    public async Task<ActionResult<StatusResponseModel>> GetTheStatus(CancellationToken token)
    {
        // Some real work here, that we have to await (it's going to be a database call, an API call, whatever.
        logger.LogInformation("Starting the Async Call");
        await Task.Delay(3000, token); // the API call, the database lookup, etc. 
        var response = new StatusResponseModel
        {
            Message = "Looks Good",
            CheckedAt = DateTime.UtcNow,

        };
        logger.LogInformation("Finished The Call");
        return Ok(response);
    }

    [HttpPost("/status")]
    [SwaggerOperation(Tags = ["Admin"])]
    public async Task<ActionResult> AddANewStatusMessage([FromBody] StatusRequestModel request)
    {
        return Ok();
    }
}


public record StatusRequestModel
{
    [Required, MinLength(5), MaxLength(30)]
    public string Message { get; set; } = string.Empty;
}

public record StatusResponseModel
{
    [Required, MinLength(5), MaxLength(30)]
    public string Message { get; init; } = string.Empty;
    [Required]
    public DateTimeOffset CheckedAt { get; init; }
    public string? CheckedBy { get; init; }
}