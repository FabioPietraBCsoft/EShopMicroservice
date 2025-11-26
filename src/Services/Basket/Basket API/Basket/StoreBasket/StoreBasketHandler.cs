namespace Basket_API.Basket.StoreBasket;

public record StoreBasketCommand(ShoppingCart Cart) : ICommand<StoreBasketResult>;
public record StoreBasketResult(string username);

public class StoreCommandValidator : AbstractValidator<StoreBasketCommand>
{
	public StoreCommandValidator()
	{
		RuleFor(x => x.Cart).NotNull().WithMessage("Cart non può essere null");
        RuleFor(x => x.Cart.UserName).NotEmpty().WithMessage("il campo Username è richiesto");

    }
}
public class StoreBasketCommandHandler(IBasketRepository repository)
    : ICommandHandler<StoreBasketCommand, StoreBasketResult>
{
    public async Task<StoreBasketResult> Handle(StoreBasketCommand command, CancellationToken cancellationToken)
    {
        ShoppingCart cart = command.Cart;
        await repository.StoreBasket(command.Cart, cancellationToken);
        return new StoreBasketResult(command.Cart.UserName);
    }
}
