
using Catalog_API.Products.GetProductById;

namespace Catalog_API.Products.GetProductByCategory;

//public record GetProductByCategoryRequest();
public record GetProductByCAategoryResponse(IEnumerable<Product> Products);

public class GetProductByCategoryEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/products/category/{category}", 
            async (string category, ISender sender) =>
        {
            var result = await sender.Send(new GetProductByCategoryQuery(category));
            var response = result.Adapt<GetProductByCAategoryResponse>();
            return Results.Ok(response);
        })
        .WithName("Get Product by Category")
        .Produces<GetProductByIdResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Get Product by Category")
        .WithDescription("Get Product by Category");
    }
}
