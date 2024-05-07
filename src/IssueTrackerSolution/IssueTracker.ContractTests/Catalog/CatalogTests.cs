
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
            api.Header("Location").SingleValueShouldMatch(Constants.GetLocationForPathRegex("catalog"));
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

    [Fact]
    public async Task NewItemsCanBeAddedAndDeleted()
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
        });

        var actualResponse = await response.ReadAsJsonAsync<CatalogItemResponse>();

        Assert.NotNull(actualResponse);

        var id = actualResponse.Id;

        var response2 = await host.Scenario(api =>
        {
            api.Delete.Url("/catalog/" + id);
            api.StatusCodeShouldBe(204);
        });

        await host.Scenario(api =>
        {
            api.Get.Url("/catalog/" + id);
            api.StatusCodeShouldBe(404);
        });
    }
    [Fact]
    public async Task UsersCanOnlyDeleteItemsTheyCreated()
    {
        var user1Token = new AuthenticationStub()
         .With(ClaimTypes.NameIdentifier, "carl@aol.com") // Sub claim
         .With(ClaimTypes.Role, "SoftwareCenter");  // this adds this role.

        await using var host = await AlbaHost.For<Program>(user1Token);
        var itemToAdd = new CreateCatalogItemRequest("Notepad", "A Text Editor on Windows");

        var response = await host.Scenario(api =>
        {
            api.Post.Json(itemToAdd).ToUrl("/catalog");
            api.StatusCodeShouldBe(201);
        });

        var actualResponse = await response.ReadAsJsonAsync<CatalogItemResponse>();

        Assert.NotNull(actualResponse);

        var id = actualResponse.Id;

        var user2Token = new AuthenticationStub()
        .With(ClaimTypes.NameIdentifier, "beth@aol.com") // Sub claim
        .With(ClaimTypes.Role, "SoftwareCenter");  // this add

        await using var host2 = await AlbaHost.For<Program>(user2Token);
        var response2 = await host2.Scenario(api =>
        {
            api.Delete.Url("/catalog/" + id);
            api.StatusCodeShouldBe(403);
        });

        await host.Scenario(api =>
        {
            api.Get.Url("/catalog/" + id);
            api.StatusCodeShouldBe(200);
        });
    }
}
