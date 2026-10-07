namespace AditiKraft.Krafter.UI.Web.Client.Infrastructure.Services;

public interface IRequestContextAccessor
{
    Uri? GetRequestBaseUri();

    bool TryRedirectToLogin();
}
