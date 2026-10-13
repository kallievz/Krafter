using AditiKraft.Krafter.Backend.Infrastructure.Localization;
using AditiKraft.Krafter.Backend.Infrastructure.Notifications;
using TickerQ.Utilities.Base;

namespace AditiKraft.Krafter.Backend.Infrastructure.Jobs;

public class Jobs(
    IEmailService emailService,
    ILocalizationBackgroundTranslationService localizationBackgroundTranslationService,
    ILocalizationTranslationJobTracker localizationTranslationJobTracker,
    ILogger<Jobs> logger)
{
    [TickerFunction(nameof(SendEmailJobAsync))]
    public async Task SendEmailJobAsync(TickerFunctionContext<SendEmailLogRequestInput> tickerContext,
        CancellationToken cancellationToken)
    {
        await emailService.ProcessEmailAsync(tickerContext.Request.EmailLogId, cancellationToken).ConfigureAwait(false);
    }

    [TickerFunction(nameof(GenerateMissingLocalizationTranslationsJobAsync))]
    public async Task GenerateMissingLocalizationTranslationsJobAsync(
        TickerFunctionContext<GenerateMissingLocalizationTranslationsRequestInput> tickerContext,
        CancellationToken cancellationToken)
    {
        string resourceId = tickerContext.Request.ResourceId;
        localizationTranslationJobTracker.MarkExecuting(resourceId);
        logger.LogInformation("TickerQ executing localization translation job for resource '{ResourceId}'.", resourceId);

        try
        {
            await localizationBackgroundTranslationService
                .GenerateMissingTranslationsAsync(resourceId, cancellationToken)
                .ConfigureAwait(false);

            localizationTranslationJobTracker.MarkCompleted(resourceId);
        }
        catch (Exception ex)
        {
            localizationTranslationJobTracker.MarkFailed(resourceId, ex.Message);
            logger.LogError(ex, "TickerQ localization translation job failed for resource '{ResourceId}'.", resourceId);
            throw;
        }
    }
}
