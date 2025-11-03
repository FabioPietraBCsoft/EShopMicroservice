using MediatR;                // Contiene le interfacce per la pipeline (IPipelineBehavior, IRequest)
using Microsoft.Extensions.Logging; // Per il logging (LogInformation, LogWarning)
using System.Diagnostics;       // Contiene la classe Stopwatch per misurare il tempo

namespace BuildingBolcks.Behaviors
{
    /// <summary>
    /// Implementa un Behavior della pipeline di MediatR per registrare l'inizio,
    /// la fine e la durata di ogni richiesta (Request) gestita.
    /// </summary>
    /// <typeparam name="TRequest">Il tipo di richiesta (comando o query).</typeparam>
    /// <typeparam name="TResponse">Il tipo di risposta attesa.</typeparam>
    public class LoggingBehavior<TRequest, TResponse>
        // Costruttore primario che riceve l'oggetto ILogger tramite iniezione di dipendenza.
        (ILogger<LoggingBehavior<TRequest, TResponse>> logger)
        // Implementa l'interfaccia IPipelineBehavior per intercettare le richieste.
        : IPipelineBehavior<TRequest, TResponse>
        // Vincoli: TRequest deve essere non nullo e implementare IRequest<TResponse> (standard MediatR).
        where TRequest : notnull, IRequest<TResponse>
        // Vincoli: TResponse deve essere non nullo.
        where TResponse : notnull
    {
        /// <summary>
        /// Metodo Handle che viene eseguito attorno all'handler effettivo della richiesta.
        /// </summary>
        /// <param name="request">La richiesta corrente.</param>
        /// <param name="next">Il delegato che chiama il prossimo Behavior o l'Handler finale della richiesta.</param>
        /// <param name="cancellationToken">Token per la cancellazione.</param>
        /// <returns>La risposta (TResponse) generata dall'handler.</returns>
        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            // 1. Log Iniziale
            // Registra l'inizio della gestione della richiesta con i tipi di richiesta e risposta e i dati della richiesta.
            logger.LogInformation("[START] handle request = {Rquest} - response {Response} - {RequestData}",
                typeof(TRequest).Name, typeof(TResponse).Name, request);

            // 2. Misurazione del Tempo (Setup)
            var timer = new Stopwatch();
            timer.Start();

            // 3. Esecuzione del Handler
            // Chiama il delegato 'next' per passare il controllo al prossimo behavior o all'handler finale.
            var response = await next();

            // 4. Misurazione del Tempo (Stop)
            timer.Stop();
            var timeTaken = timer.Elapsed; // Ottiene il tempo trascorso (TimeSpan)

            // 5. Controllo Performance (ATTENZIONE: Uso TotalSeconds per misurare i secondi totali)
            // Se il tempo totale impiegato supera i 3 secondi, viene registrato un Warning.
            if (timeTaken.TotalSeconds > 3)
            {
                logger.LogWarning("[PERFORMANCE] The request {Request} took {TimeTaken} seconds.",
                   typeof(TRequest).Name, timeTaken.TotalSeconds);
                // Usa TotalSeconds per mostrare il tempo totale corretto nel log
            }

            // 6. Log Finale
            // Registra la fine della gestione della richiesta.
            logger.LogInformation("[END] Handled {Request} with {Response}",
                typeof(TRequest).Name, typeof(TResponse).Name);

            // Restituisce la risposta ottenuta dal Handler.
            return response;
        }
    }
}