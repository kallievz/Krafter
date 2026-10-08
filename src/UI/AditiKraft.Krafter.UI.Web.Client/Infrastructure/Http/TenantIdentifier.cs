using AditiKraft.Krafter.Contracts.Common;
using AditiKraft.Krafter.Contracts.Common.Enums;
using AditiKraft.Krafter.UI.Web.Client.Common.Models;

namespace AditiKraft.Krafter.UI.Web.Client.Infrastructure.Http;

public class TenantIdentifier(IServiceProvider serviceProvider, AppUrls urls)
{
    public (string tenantIdentifier, string backendUrl, string rootDomain, string clientBaseAddress, bool isServerSide)
        Get()
    {
        string formFactor = serviceProvider.GetRequiredService<IFormFactor>().GetFormFactor();
        bool isServerSide = formFactor == "Web";
        Uri uiUri = GetCurrentUiUri(formFactor);
        if (TenantSettings.TenancyMode == TenancyMode.Multi && urls.IsInvalidUiTenantHost(uiUri.Host))
        {
            throw new InvalidOperationException("Tenant not found.");
        }
        string? subdomain = TenantSettings.TenancyMode == TenancyMode.Multi
            ? urls.GetUiTenantIdentifier(uiUri.Host)
            : null;
        string tenantIdentifier = !string.IsNullOrWhiteSpace(subdomain)
            ? subdomain
            : string.IsNullOrWhiteSpace(TenantInfo.Identifier)
                ? DefaultTenantConstants.Identifier
                : TenantInfo.Identifier;
        string clientBaseAddress = uiUri.GetLeftPart(UriPartial.Authority);
        Uri apiUri;

        if (isServerSide)
        {
            // Server requests use the configured connection address and carry the tenant in a header.
            apiUri = urls.GetServerApiUri();
        }
        else
        {
            apiUri = urls.GetApiUri() ?? uiUri;
        }

        string rootDomain = urls.GetTenantBaseUri().Host;
        return (tenantIdentifier, apiUri.GetLeftPart(UriPartial.Authority), rootDomain, clientBaseAddress, isServerSide);
    }

    private Uri GetCurrentUiUri(string formFactor)
    {
        if (formFactor == "Web")
        {
            IRequestContextAccessor? requestContextAccessor = serviceProvider.GetService<IRequestContextAccessor>();
            Uri? requestUri = requestContextAccessor?.GetRequestBaseUri();
            if (requestUri is not null)
            {
                return requestUri;
            }

            Uri? fallbackBaseUri = TryGetNavigationBaseUri();
            if (fallbackBaseUri is not null)
            {
                return fallbackBaseUri;
            }

            return urls.GetRootUiUri();
        }

        if (formFactor == "WebAssembly")
        {
            Uri? webAssemblyBaseUri = TryGetNavigationBaseUri();
            if (webAssemblyBaseUri is not null)
            {
                return webAssemblyBaseUri;
            }

            return urls.GetRootUiUri();
        }

        return urls.GetRootUiUri();
    }

    private Uri? TryGetNavigationBaseUri()
    {
        NavigationManager? navigationManager = serviceProvider.GetService<NavigationManager>();
        if (navigationManager is null)
        {
            return null;
        }

        try
        {
            return Uri.TryCreate(navigationManager.BaseUri, UriKind.Absolute, out Uri? baseUri) ? baseUri : null;
        }
        catch (InvalidOperationException)
        {
            return null;
        }
    }
}
