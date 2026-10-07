namespace AditiKraft.Krafter.UI.Web.Client.Infrastructure.Services;

public sealed class BrowserRequestContextAccessor : IRequestContextAccessor
{
    public Uri? GetRequestBaseUri()
    {
        return null;
    }

    public bool TryRedirectToLogin()
    {
        return false;
    }
}
