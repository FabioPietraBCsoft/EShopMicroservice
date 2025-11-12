using Mapster;

namespace Basket_API.Basket.GetBasket;
//public record GetBasketRequest(string Username);
public record GetBasketResponse(ShoppingCart cart);
public class GetBasketEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/basket/{username}", async (string userName, ISender sender) =>
        {
            var result = await sender.Send(new GetBasketQuery(userName));
            var respone = result.Adapt<GetBasketResponse>();
            return Results.Ok(respone);
        })
        .WithName("GetBasket")
        .Produces<GetBasketResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Get Basket")
        .WithDescription("Get Basket");
    }
}
