using FluentValidation;
using Riok.Mapperly.Abstractions;

namespace IssueTracker.Api.Catalog;

// "Models" are the things that leave or arrive from outside our service boundary.
// they are C# types that will be deserialized or serialized from the json coming into our out of our api.

public record CreateCatalogItemRequest(string Title, string Description);

public static class ModelExtensions
{
    //public static CatalogItemResponse MapToResponse(this CatalogItem item)
    //{
    //    return new CatalogItemResponse(item.Id, item.Title, item.Description);
    //}
    public static CatalogItem MapToCatalogItem(this CreateCatalogItemRequest request, string addedBy)
    {
        return new CatalogItem
        {
            Id = Guid.NewGuid(),
            Description = request.Description,
            Title = request.Title,
            AddedBy = addedBy,
            CreatedAt = DateTimeOffset.Now
        };
    }
}


[Mapper]
public static partial class CatalogMappers
{
    public static partial CatalogItemResponse MapToResponse(this CatalogItem item);
}


public class CreateCatalogItemRequestValidator : AbstractValidator<CreateCatalogItemRequest>
{
    public CreateCatalogItemRequestValidator()
    {
        RuleFor(r => r.Title).NotEmpty().WithMessage("We Need A Title");

        RuleFor(r => r.Title).MinimumLength(5).MaximumLength(256).WithMessage("Has to be between 5 and 256 characters");
        RuleFor(r => r.Description).NotEmpty().MaximumLength(1024);
    }
}
public record CatalogItemResponse(Guid Id, string Title, string Description);