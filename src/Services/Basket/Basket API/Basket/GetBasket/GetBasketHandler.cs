
namespace Basket_API.Basket.GetBasket;


public record GetBasketQuery(string UserName) : IQuery<GetBasketResult>;
public record GetBasketResult(ShoppingCart Cart);
public class GetBasketQueryHandler : IQueryHandler<GetBasketQuery, GetBasketResult>
{
    public async Task<GetBasketResult> Handle(GetBasketQuery query, CancellationToken cancellationToken)
    {
        //TODO: get basket from DB
        //var basket = await _repository.GetBasket(query.Username);
        return new GetBasketResult(new ShoppingCart("swn"));

    }
}

