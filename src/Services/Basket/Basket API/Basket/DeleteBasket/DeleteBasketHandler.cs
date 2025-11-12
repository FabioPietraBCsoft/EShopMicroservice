using Basket_API.Basket.StoreBasket;
using System.Data;

public record DeleteBasketCommand(string UserName) : ICommand<DeleteBasketResult>;
public record DeleteBasketResult(bool IsSucces);

public class DeleteCommandValidator : AbstractValidator<DeleteBasketCommand>
{
	public DeleteCommandValidator()
	{
		RuleFor(x => x.UserName).NotEmpty().WithMessage("Username è richiesto");
	}
}
public class DeleteBasketCommandHandler
    : ICommandHandler<DeleteBasketCommand, DeleteBasketResult>
{
    public async Task<DeleteBasketResult> Handle(DeleteBasketCommand command, CancellationToken cancellationToken)
    {
        return new DeleteBasketResult(true);
    }
}

