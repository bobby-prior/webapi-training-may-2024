
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
            .With(ClaimTypes.NameIdentifier, "carl@aol.com") // Sub claim
            .With(ClaimTypes.Role, "SoftwareCenter");  // this adds this role.

        await using var host = await AlbaHost.For<Program>(stubbedToken);

        var itemToAdd = new CreateCatalogItemRequest("Notepad", "A Text Editor on Windows");

        var response = await host.Scenario(api =>
        {
            api.Post.Json(itemToAdd).ToUrl("/catalog");
            api.StatusCodeShouldBe(201);
            api.Header("Location").SingleValueShouldMatch("http://localhost/catalog/*.");
        });

        var actualResponse = await response.ReadAsJsonAsync<CatalogItemResponse>();

        Assert.NotNull(actualResponse);
        Assert.Equal("Notepad", actualResponse.Title);
        Assert.Equal("A Text Editor on Windows", actualResponse.Description);

        // We will do the "GET" Tomorrow to round this out...


    }
    [Fact]
    public async Task OnlySoftwareCenterPeopleCanAddThings()
    {
        var stubbedToken = new AuthenticationStub()
           .With(ClaimTypes.NameIdentifier, "carl@aol.com") // Sub claim
           .With(ClaimTypes.Role, "TacoNose");  // this adds this role.

        await using var host = await AlbaHost.For<Program>(stubbedToken);

        var itemToAdd = new CreateCatalogItemRequest("Notepad", "A Text Editor on Windows");

        var response = await host.Scenario(api =>
        {
            api.Post.Json(itemToAdd).ToUrl("/catalog");
            api.StatusCodeShouldBe(403); // Unauthorized
        });



    }
}
