namespace AditiKraft.Krafter.UI.Web.Client.Infrastructure.Services;

public interface IRequestContextAccessor
{
    public Uri? GetRequestBaseUri();

    public bool TryRedirectToLogin();
}
