using AditiKraft.Krafter.Backend.Infrastructure.Notifications;
using TickerQ.Utilities.Base;

namespace AditiKraft.Krafter.Backend.Infrastructure.Jobs;

public class Jobs(IEmailService emailService)
{
    [TickerFunction(nameof(SendEmailJobAsync))]
    public async Task SendEmailJobAsync(TickerFunctionContext<SendEmailRequestInput> tickerContext,
        CancellationToken cancellationToken)
    {
        await emailService.SendEmailAsync(
            tickerContext.Request.Email,
            tickerContext.Request.Subject,
            tickerContext.Request.HtmlMessage, cancellationToken
        );
    }
}
