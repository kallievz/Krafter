using AditiKraft.Krafter.Contracts.Contracts.Auth;
using Refit;

namespace AditiKraft.Krafter.UI.Web.Client.Features.Auth;

public interface IAuthApi
{
    [Post("/api/tokens")]
    public Task<Response<TokenResponse>> CreateTokenAsync(
        [Header("x-tenant-identifier")] string? tenantIdentifier,
        [Body] TokenRequest request,
        CancellationToken cancellationToken = default);

    [Post("/api/tokens/refresh")]
    public Task<Response<TokenResponse>> RefreshTokenAsync([Body] RefreshTokenRequest request,
        CancellationToken cancellationToken = default);

    [Post("/api/external-auth/google")]
    public Task<Response<TokenResponse>> GoogleAuthAsync([Body] GoogleAuthRequest request,
        CancellationToken cancellationToken = default);

    [Post("/api/tokens/self-register")]
    public Task<Response> SelfRegisterAsync(
        [Header("x-tenant-identifier")] string? tenantIdentifier,
        [Body] SelfRegisterRequest request,
        CancellationToken cancellationToken = default);

    [Post("/api/tokens/logout")]
    public Task LogoutAsync(CancellationToken cancellationToken = default);

    [Get("/api/tokens/current")]
    public Task<Response<TokenResponse>> GetCurrentTokenAsync(CancellationToken cancellationToken = default);
}
