using AditiKraft.Krafter.Backend.Common.Auth;
using AditiKraft.Krafter.Contracts.Common.Extensions;
using Serilog.Context;

namespace AditiKraft.Krafter.Backend.Web.Middleware;

public class CurrentUserMiddleware(ICurrentUserInitializer currentUserInitializer) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        currentUserInitializer.SetCurrentUser(context.User);

        using (PushProperty("UserId", context.User.GetUserId()))
        using (PushProperty("UserName", context.User.Identity?.Name))
        using (PushProperty("UserEmail", context.User.GetEmail()))
        {
            await next(context).ConfigureAwait(false);
        }
    }

    private static IDisposable? PushProperty(string propertyName, string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : LogContext.PushProperty(propertyName, value);
}
