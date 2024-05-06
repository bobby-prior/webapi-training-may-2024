using Alba;
using Alba.Security;
using IssueTracker.Api.Catalog;
using System.Security.Claims;

namespace IssueTracker.ContractTests.Catalog;
public class CatalogTests
{
    [Fact]
    public async Task CanAddAnItemToTheCatalog()
    {
        var stubbedToken = new AuthenticationStub()
            .With(ClaimTypes.NameIdentifier, "carl@aol.com")
            .With(ClaimTypes.Role, "SoftwareCenter");

        await using var host = await AlbaHost.For<Program>(stubbedToken);

        var itemToAdd = new CreateCatalogItemRequest("Notepad", "A Text Editor on Windows");

        var response = await host.Scenario(api =>
        {
            api.Post.Json(itemToAdd).ToUrl("/catalog");
            api.StatusCodeShouldBeOk();
        });

        var actualResponse = await response.ReadAsJsonAsync<CatalogItemResponse>();

        Assert.NotNull(actualResponse);
        Assert.Equal("Notepad", actualResponse.Title);
        Assert.Equal("A Text Editor on Windows", actualResponse.Description);
    }

    [Fact]
    public async Task OnlySoftwareCenterPeopleCanAddThings()
    {
        var stubbedToken = new AuthenticationStub()
            .With(ClaimTypes.NameIdentifier, "carl@aol.com")
            .With(ClaimTypes.Role, "TacoNose");

        await using var host = await AlbaHost.For<Program>(stubbedToken);

        var itemToAdd = new CreateCatalogItemRequest("Notepad", "A Text Editor on Windows");

        var response = await host.Scenario(api =>
        {
            api.Post.Json(itemToAdd).ToUrl("/catalog");
            api.StatusCodeShouldBe(403);
        });
    }
}
