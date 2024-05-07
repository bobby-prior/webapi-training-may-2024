
using FluentValidation.TestHelper;
using IssueTracker.Api.Catalog;

namespace IssueTracker.UnitTests;
public class AddCatalogItemValidationTests
{
    [Fact]
    public async Task CanValidate()
    {
        var validator = new CreateCatalogItemRequestValidator();
        var model = new CreateCatalogItemRequest("", new string('X', 1026));

        var results = await validator.TestValidateAsync(model);

        results.ShouldHaveValidationErrorFor(m => m.Title).WithErrorMessage("We Need A Title");

        results.ShouldHaveValidationErrorFor(m => m.Description);

    }
}
