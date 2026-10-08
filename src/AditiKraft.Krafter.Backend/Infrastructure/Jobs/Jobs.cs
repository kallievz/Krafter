using AditiKraft.Krafter.Backend.Infrastructure.Notifications;
using TickerQ.Utilities.Base;

namespace AditiKraft.Krafter.Backend.Infrastructure.Jobs;

public class Jobs(IEmailService emailService)
{
    [TickerFunction(nameof(SendEmailJobAsync))]
    public async Task SendEmailJobAsync(TickerFunctionContext<SendEmailLogRequestInput> tickerContext,
        CancellationToken cancellationToken)
    {
        await emailService.ProcessEmailAsync(tickerContext.Request.EmailLogId, cancellationToken).ConfigureAwait(false);
    }
}
