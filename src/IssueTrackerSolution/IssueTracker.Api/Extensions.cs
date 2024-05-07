using Microsoft.AspNetCore.Mvc;

namespace IssueTracker.Api;

public static class Extensions
{
    public static ObjectResult CreateProblemDetailsForModelValidation(
        this ControllerBase controller,
        string details,
        IDictionary<string, string[]> errors)
    {

        var problemDetails = new ProblemDetails()
        {
            Detail = details,
            Status = 400,
            Title = "Validation Errors"
        };
        problemDetails.Extensions.Add("invalidParams", errors);
        return new ObjectResult(problemDetails)
        {
            StatusCode = 400
        };
    }
}
