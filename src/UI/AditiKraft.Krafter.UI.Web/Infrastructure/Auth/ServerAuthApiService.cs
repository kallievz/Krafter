using System.Text;
using AditiKraft.Krafter.UI.Web.Client.Features.Auth;
using Microsoft.Extensions.Caching.Distributed;
using Refit;

namespace AditiKraft.Krafter.UI.Web.Infrastructure.Auth;

public class ServerAuthApiService(
    IAuthApi authApi,
    IAuthStorageService localStorage,
    IDistributedCache cache,
    ILogger<ServerAuthApiService> logger) : IAuthApiService
{
    public async Task<Response<TokenResponse>> CreateTokenAsync(TokenRequest request,
        CancellationToken cancellation)
    {
        try
        {
            return await authApi.CreateTokenAsync(request.TenantId, request, cancellation);
        }
        catch (ApiException ex)
        {
            logger.LogError(ex, "Error during server-side token creation");
            return new Response<TokenResponse>
            {
                IsError = true,
                Message = "Failed to create token. Please log in again.",
                StatusCode = (int)ex.StatusCode
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during server-side token creation");
            return new Response<TokenResponse>
            {
                IsError = true,
                Message = "Failed to create token. Please log in again.",
                StatusCode = (int)System.Net.HttpStatusCode.InternalServerError
            };
        }
    }

    public async Task<Response> SelfRegisterAsync(SelfRegisterRequest request, CancellationToken cancellation)
    {
        try
        {
            return await authApi.SelfRegisterAsync(request.TenantId, request, cancellation);
        }
        catch (ApiException ex)
        {
            logger.LogError(ex, "Error during server-side self registration");
            return new Response
            {
                IsError = true,
                Message = "Failed to register user. Please try again.",
                StatusCode = (int)ex.StatusCode
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during server-side self registration");
            return new Response
            {
                IsError = true,
                Message = "Failed to register user. Please try again.",
                StatusCode = (int)System.Net.HttpStatusCode.InternalServerError
            };
        }
    }

    public async Task<Response<TokenResponse>> RefreshTokenAsync(RefreshTokenRequest request,
        CancellationToken cancellation)
    {
        try
        {
            string key = "auth-refresh:" + Convert.ToHexString(
                System.Security.Cryptography.SHA256.HashData(Encoding.UTF8.GetBytes($"{request.Token}\0{request.RefreshToken}")));

            byte[]? cachedBytes = await cache.GetAsync(key, cancellation);
            if (cachedBytes is not null)
            {
                string cachedJson = Encoding.UTF8.GetString(cachedBytes);
                Response<TokenResponse>? cachedResponse = System.Text.Json.JsonSerializer.Deserialize<Response<TokenResponse>>(cachedJson);
                if (cachedResponse is not null)
                {
                    return cachedResponse;
                }
            }

            Response<TokenResponse> response = await authApi.RefreshTokenAsync(request, cancellation);
            if (response is not { IsError: false, Data: not null } ||
                string.IsNullOrWhiteSpace(response.Data.Token) ||
                string.IsNullOrWhiteSpace(response.Data.RefreshToken))
            {
                return response;
            }

            string serialized = System.Text.Json.JsonSerializer.Serialize(response);
            DistributedCacheEntryOptions entryOptions = new()
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(5)
            };

            await cache.SetAsync(key, Encoding.UTF8.GetBytes(serialized), entryOptions, cancellation);
            return response;
        }
        catch (OperationCanceledException) when (cancellation.IsCancellationRequested)
        {
            throw;
        }
        catch (ApiException ex)
        {
            logger.LogError(ex, "Error during server-side token refresh");
            return new Response<TokenResponse>
            {
                IsError = true,
                Message = "Failed to refresh token. Please log in again.",
                StatusCode = (int)ex.StatusCode
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during server-side token refresh");
            return new Response<TokenResponse>
            {
                IsError = true,
                Message = "Failed to refresh token. Please log in again.",
                StatusCode = (int)System.Net.HttpStatusCode.InternalServerError
            };
        }
    }

    public async Task<Response<TokenResponse>> ExternalAuthAsync(TokenRequest request,
        CancellationToken cancellation)
    {
        try
        {
            GoogleAuthRequest googleRequest = new() { Code = request.Code ?? string.Empty };
            return await authApi.GoogleAuthAsync(googleRequest, cancellation);
        }
        catch (ApiException ex)
        {
            logger.LogError(ex, "Error during server-side external auth");
            return new Response<TokenResponse>
            {
                IsError = true,
                Message = "Failed to authenticate. Please try again.",
                StatusCode = (int)ex.StatusCode
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during server-side external auth");
            return new Response<TokenResponse>
            {
                IsError = true,
                Message = "Failed to authenticate. Please try again.",
                StatusCode = (int)System.Net.HttpStatusCode.InternalServerError
            };
        }
    }

    public async Task<Response<TokenResponse>> GetCurrentTokenAsync(CancellationToken cancellation)
    {
        try
        {
            string? token = await localStorage.GetCachedAuthTokenAsync();
            string? refreshToken = await localStorage.GetCachedRefreshTokenAsync();
            DateTime authTokenExpiryDate = await localStorage.GetAuthTokenExpiryDate();
            DateTime refreshTokenExpiry = await localStorage.GetRefreshTokenExpiryDate();
            ICollection<string>? permissions = await localStorage.GetCachedPermissionsAsync();

            if (!string.IsNullOrWhiteSpace(token) && !string.IsNullOrWhiteSpace(refreshToken))
            {
                string preferredLanguageCode = "en";
                try
                {
                    JwtSecurityToken jwtToken =
                        new JwtSecurityTokenHandler().ReadJwtToken(token);
                    string? tokenLanguageCode = jwtToken.Claims
                        .FirstOrDefault(claim => claim.Type == AppClaimTypes.PreferredLanguageCode)?.Value;
                    if (!string.IsNullOrWhiteSpace(tokenLanguageCode))
                    {
                        preferredLanguageCode = tokenLanguageCode.Trim().ToLowerInvariant();
                    }
                }
                catch
                {
                    preferredLanguageCode = "en";
                }

                return new Response<TokenResponse>
                {
                    Data = new TokenResponse(
                        token,
                        refreshToken,
                        refreshTokenExpiry,
                        authTokenExpiryDate,
                        permissions?.ToList() ?? [],
                        preferredLanguageCode),
                    StatusCode = (int)System.Net.HttpStatusCode.OK
                };
            }

            return new Response<TokenResponse>
            {
                IsError = true,
                Message = "No valid token found. Please log in again.",
                StatusCode = (int)System.Net.HttpStatusCode.Unauthorized
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting current token");
            return new Response<TokenResponse>
            {
                IsError = true,
                Message = "Failed to get current token.",
                StatusCode = (int)System.Net.HttpStatusCode.InternalServerError
            };
        }
    }

    public Task LogoutAsync(CancellationToken cancellation) => localStorage.ClearCacheAsync();
}
