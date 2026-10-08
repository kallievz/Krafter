namespace AditiKraft.Krafter.UI.Web.Infrastructure.Services;

public sealed class ServerRequestContextAccessor(IHttpContextAccessor httpContextAccessor) : IRequestContextAccessor
{
    public Uri? GetRequestBaseUri()
    {
        HttpRequest? request = httpContextAccessor.HttpContext?.Request;
        if (request is null)
        {
            return null;
        }

        return new Uri($"{request.Scheme}://{request.Host}");
    }

    public bool TryRedirectToLogin()
    {
        HttpContext? httpContext = httpContextAccessor.HttpContext;
        if (httpContext is null || httpContext.Response.HasStarted)
        {
            return false;
        }

        httpContext.Response.Redirect("/login");
        return true;
    }
}
