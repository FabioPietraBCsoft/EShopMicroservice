using BuildingBolcks.Exceptions;
using FluentValidation;           // Namespace per l'eccezione di validazione di FluentValidation
using Microsoft.AspNetCore.Diagnostics; // Contiene l'interfaccia IExceptionHandler
using Microsoft.AspNetCore.Http;        // Contiene HttpContext e StatusCodes
using Microsoft.AspNetCore.Mvc;         
using Microsoft.Extensions.Logging;     // Per la registrazione degli errori

namespace BuildingBlocks.Exceptions.Handler;

/// <summary>
/// Gestore di eccezioni personalizzato che implementa l'interfaccia IExceptionHandler
/// per centralizzare la gestione degli errori e la creazione delle risposte HTTP.
/// </summary>
public class CustomExceptionHandler
    // Iniezione di dipendenza nel costruttore primario per ottenere l'oggetto ILogger
    (ILogger<CustomExceptionHandler> logger)
    : IExceptionHandler
{
    /// <summary>
    /// Metodo chiamato dal middleware di gestione degli errori di ASP.NET Core per tentare di gestire l'eccezione.
    /// </summary>
    /// <param name="context">Il contesto HTTP corrente.</param>
    /// <param name="exception">L'eccezione che deve essere gestita.</param>
    /// <param name="cancellationToken">Token per la cancellazione.</param>
    /// <returns>Restituisce true se l'eccezione è stata gestita e il flusso si interrompe.</returns>
    public async ValueTask<bool> TryHandleAsync(
        HttpContext context,
        Exception exception,
        CancellationToken cancellationToken)
    {
        // 1. Logging dell'Errore
        logger.LogError(
            "Error Message: {exceptionMessage}, Time of occurrence {time}",
            exception.Message, DateTime.UtcNow);

        // 2. Mapping delle Eccezioni a Dettagli HTTP
        // Usa un 'switch expression' (espressione switch) per mappare diversi tipi di eccezioni
        // a una tupla contenente (Dettaglio del Messaggio, Titolo, Codice di Stato HTTP).
        (string Detail, string Title, int StatusCode) details = exception switch
        {
            // Caso 1: Eccezione del Server Interno (es. errore di sistema generico)
            InternalServerException =>
            (
                exception.Message,
                exception.GetType().Name,
                // Imposta lo status code della risposta a 500 (Internal Server Error)
                context.Response.StatusCode = StatusCodes.Status500InternalServerError
            ),

            // Caso 2: Errore di Validazione (tipico di FluentValidation)
            ValidationException =>
            (
                exception.Message,
                exception.GetType().Name,
                // Imposta lo status code della risposta a 400 (Bad Request)
                context.Response.StatusCode = StatusCodes.Status400BadRequest
            ),

            // Caso 3: Richiesta Non Valida (eccezione personalizzata per problemi lato client)
            BadRequestException =>
            (
                exception.Message,
                exception.GetType().Name,
                // Imposta lo status code della risposta a 400 (Bad Request)
                context.Response.StatusCode = StatusCodes.Status400BadRequest
            ),

            // Caso 4: Risorsa Non Trovata (eccezione personalizzata)
            NotFoundException =>
            (
                exception.Message,
                exception.GetType().Name,
                // Imposta lo status code della risposta a 404 (Not Found)
                context.Response.StatusCode = StatusCodes.Status404NotFound
            ),

            // Caso Predefinito (_): Tutte le altre eccezioni non gestite esplicitamente
            _ =>
            (
                exception.Message,
                exception.GetType().Name,
                // Imposta lo status code predefinito a 500
                context.Response.StatusCode = StatusCodes.Status500InternalServerError
            )
        };

        // 3. Creazione dell'Oggetto ProblemDetails
        // ProblemDetails è lo standard RFC 7807 per le risposte di errore HTTP in formato JSON.
        var problemDetails = new ProblemDetails
        {
            Title = details.Title,       // Titolo (es. "NotFoundException")
            Detail = details.Detail,     // Dettaglio del messaggio di errore
            Status = details.StatusCode, // Codice di stato HTTP (es. 404, 500)
            Instance = context.Request.Path // Percorso della richiesta che ha generato l'errore
        };

        // Aggiunge l'ID di tracciamento della richiesta come estensione per il debug
        problemDetails.Extensions.Add("traceId", context.TraceIdentifier);

        // 4. Gestione Specifica dell'Errore di Validazione
        // Controlla se l'eccezione è un'eccezione di validazione specifica (FluentValidation)
        if (exception is ValidationException validationException)
        {
            // Se lo è, aggiunge un campo extra ("ValidationErrors") con l'elenco degli errori di validazione
            problemDetails.Extensions.Add("ValidationErrors", validationException.Errors);
        }

        // 5. Scrittura della Risposta HTTP
        // Scrive l'oggetto ProblemDetails serializzato in JSON nel corpo della risposta HTTP.
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(problemDetails, cancellationToken: cancellationToken);

        // Indica che l'eccezione è stata gestita con successo.
        return true;
    }
}