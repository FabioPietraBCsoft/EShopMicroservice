using BuildingBolcks.CQRS;
using FluentValidation;
using MediatR;

namespace BuildingBolcks.Behaviors;
public class ValidationBehavior<TRequest, TResponse>
    (IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICommand<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        //creo un oggetto di contesto al quale si passa la richiesta in arrivo (request) 
        var context = new ValidationContext<TRequest>(request);

        /*eseguo tutti i validatori usando l'oggetto validatori (validitors)
        e restituire i risultati della validazione*/
        var validationResults =
            await Task.WhenAll(validators.Select(v => v.ValidateAsync(context,cancellationToken)));

        /*Verifico se c'è qualche errore nei risultati della convalida (validationResults) 
         filtrando gli errori e selezionando questi errori nell'oggetto (failures)*/
        var failures =
            validationResults
            .Where(r => r.Errors.Any())
            .SelectMany(r => r.Errors)
            .ToList();

        //in caso di errori lanciamo un'eccezione
        if (failures.Any())
        {
            throw new ValidationException(failures);

        }

        //restituisco il gestore del delegato della richiesta successiva (next())
        return await next();
    }
}

