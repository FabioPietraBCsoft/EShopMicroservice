
namespace Catalog_API.Products.CreateProduct;

public record CreateProductCommand(string Name, List<string> Category, string Description, string ImageFile, decimal Price)
    : ICommand<CreateProductResult>;
public record CreateProductResult(Guid Id);

public class CreateProductValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Il nome è richiesto");
        RuleFor(x => x.Category).NotEmpty().WithMessage("la categoria è richiesto");
        RuleFor(x => x.Description).NotEmpty().WithMessage("la descrizione richiesto");
        RuleFor(x => x.ImageFile).NotEmpty().WithMessage("ImageFile è richiesta");
        RuleFor(x => x.Price).GreaterThan(0).WithMessage("Il prezzo deve essere maggiore di 0");

    }
}

internal class CreateProductCommandHandler(IDocumentSession session)
    : ICommandHandler<CreateProductCommand, CreateProductResult>
{
    public async Task<CreateProductResult> Handle(CreateProductCommand command, CancellationToken cancellationToken)
    {

        //Creo Product entity da command object
        var product = new Product
        {
            Name = command.Name,
            Category = command.Category,
            Description = command.Description,
            ImageFile = command.ImageFile,
            Price = command.Price
        };

        //Salvo nel db
        session.Store(product);
        await session.SaveChangesAsync(cancellationToken);




        //return CreateProductResult result
        return new CreateProductResult(product.Id);
    }
}

