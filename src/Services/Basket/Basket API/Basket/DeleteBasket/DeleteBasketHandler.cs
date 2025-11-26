using Basket_API.Basket.StoreBasket;
using System.Data;

public record DeleteBasketCommand(string UserName) : ICommand<DeleteBasketResult>;
public record DeleteBasketResult(bool IsSuccess);

public class DeleteCommandValidator : AbstractValidator<DeleteBasketCommand>
{
	public DeleteCommandValidator()
	{
		RuleFor(x => x.UserName).NotEmpty().WithMessage("Username è richiesto");
	}
}
public class DeleteBasketCommandHandler(IBasketRepository repository)
    : ICommandHandler<DeleteBasketCommand, DeleteBasketResult>
{
    public async Task<DeleteBasketResult> Handle(DeleteBasketCommand command, CancellationToken cancellationToken)
    {
        await repository.DeleteBasket(command.UserName,cancellationToken);
        
        return new DeleteBasketResult(true);
    }
}

