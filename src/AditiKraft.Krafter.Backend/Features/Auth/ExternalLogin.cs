using System.Text.Json.Serialization;
using AditiKraft.Krafter.Backend.Features.Auth.Common;
using AditiKraft.Krafter.Backend.Features.Roles.Common;
using AditiKraft.Krafter.Backend.Features.Users.Common;
using AditiKraft.Krafter.Backend.Infrastructure.Persistence;
using AditiKraft.Krafter.Backend.Web;
using AditiKraft.Krafter.Contracts.Common;
using AditiKraft.Krafter.Contracts.Common.Models;
using AditiKraft.Krafter.Contracts.Contracts.Auth;
using AditiKraft.Krafter.Contracts.Contracts.Roles;
using Microsoft.AspNetCore.Identity;

namespace AditiKraft.Krafter.Backend.Features.Auth;

public sealed class ExternalAuth
{
    public class GoogleAuthClient(HttpClient httpClient, IConfiguration configuration, ILogger<GoogleAuthClient> logger)
    {
        public class GoogleTokens
        {
            [JsonPropertyName("access_token")] public string AccessToken { get; set; } = string.Empty;
            [JsonPropertyName("id_token")] public string IdToken { get; set; } = string.Empty;
        }

        public class GoogleUserInfo
        {
            [JsonPropertyName("email")] public string Email { get; set; } = string.Empty;
            [JsonPropertyName("verified_email")] public bool VerifiedEmail { get; set; }
            [JsonPropertyName("name")] public string Name { get; set; } = string.Empty;
            [JsonPropertyName("given_name")] public string GivenName { get; set; } = string.Empty;
            [JsonPropertyName("family_name")] public string FamilyName { get; set; } = string.Empty;
        }

        public async Task<GoogleTokens?> ExchangeCodeForTokensAsync(string code)
        {
            string? clientId = configuration["Authentication:Google:ClientId"];
            string? clientSecret = configuration["Authentication:Google:ClientSecret"];
            AppUrls urls = configuration.GetSection(AppUrls.SectionName).Get<AppUrls>() ?? new AppUrls();
            string redirectUri = urls.GetGoogleRedirectUri().AbsoluteUri;

            var tokenRequestParams = new Dictionary<string, string?>
            {
                ["client_id"] = clientId,
                ["client_secret"] = clientSecret,
                ["code"] = code,
                ["grant_type"] = "authorization_code",
                ["redirect_uri"] = redirectUri
            };

            HttpResponseMessage response = await httpClient.PostAsync(
                "https://oauth2.googleapis.com/token",
                new FormUrlEncodedContent(tokenRequestParams!));

            if (!response.IsSuccessStatusCode)
            {
                logger.LogError("Error exchanging code for tokens: {StatusCode} {ReasonPhrase}",
                    response.StatusCode, response.ReasonPhrase);
                logger.LogError("Response: {ResponseContent}",
                    await response.Content.ReadAsStringAsync().ConfigureAwait(false));

                return null;
            }

            return await response.Content.ReadFromJsonAsync<GoogleTokens>().ConfigureAwait(false);
        }

        public async Task<GoogleUserInfo?> GetUserInfoAsync(string accessToken)
        {
            var requestMessage =
                new HttpRequestMessage(HttpMethod.Get, "https://www.googleapis.com/oauth2/v2/userinfo");
            requestMessage.Headers.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            HttpResponseMessage response = await httpClient.SendAsync(requestMessage).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return await response.Content.ReadFromJsonAsync<GoogleUserInfo>().ConfigureAwait(false);
        }
    }

    internal sealed class Handler(
        ITokenService tokenService,
        ApplicationDbContext db,
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        GoogleAuthClient googleAuthClient) : IScopedHandler
    {
        public async Task<Response<TokenResponse>> GetTokenAsync(
            GoogleAuthRequest request,
            CancellationToken cancellationToken)
        {
            // Get the auth info using the code
            GoogleAuthClient.GoogleTokens? tokens = await googleAuthClient.ExchangeCodeForTokensAsync(request.Code).ConfigureAwait(false);

            if (tokens == null)
            {
                return Response<TokenResponse>.Unauthorized("Invalid token");
            }

            // Get user info from Google
            GoogleAuthClient.GoogleUserInfo? userInfo = await googleAuthClient.GetUserInfoAsync(tokens.AccessToken).ConfigureAwait(false);
            if (userInfo == null)
            {
                return Response<TokenResponse>.Unauthorized("Invalid user info");
            }

            // Find or create user based on email
            ApplicationUser? user = await userManager.FindByEmailAsync(userInfo.Email).ConfigureAwait(false);
            if (user == null)
            {
                ApplicationRole? basic = await roleManager.FindByNameAsync(RoleConstants.Basic).ConfigureAwait(false);
                if (basic is null)
                {
                    return Response<TokenResponse>.NotFound("Basic Role Not Found.");
                }

                user = new ApplicationUser
                {
                    IsActive = true,
                    FirstName = userInfo.GivenName,
                    LastName = userInfo.FamilyName,
                    Email = userInfo.Email,
                    EmailConfirmed = userInfo.VerifiedEmail,
                    PhoneNumber = userInfo.Email,
                    UserName = userInfo.Email,
                    Id = Guid.CreateVersion7().ToString()
                };
                if (string.IsNullOrWhiteSpace(user.UserName))
                {
                    user.UserName = user.Email;
                }

                IdentityResult result = await userManager.CreateAsync(user).ConfigureAwait(false);
                if (!result.Succeeded)
                {
                    return Response<TokenResponse>.BadRequest("An error occurred while creating user.");
                }

                db.UserRoles.Add(new ApplicationUserRole { RoleId = basic.Id, UserId = user.Id });
                await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }

            Response<TokenResponse> res = await tokenService.GenerateTokensAndUpdateUser(user.Id, string.Empty).ConfigureAwait(false);
            return res;
        }
    }

    public sealed class GoogleAuthRoute : IRouteRegistrar
    {
        public void MapRoute(IEndpointRouteBuilder app)
        {
            RouteGroupBuilder productGroup = app.MapGroup(ApiRoutes.ExternalAuth)
                .AddFluentValidationFilter();

            productGroup.MapPost($"/{RouteSegment.Google}", async (
                HttpContext context,
                GoogleAuthRequest request,
                Handler externalAuthService,
                ApplicationDbContext db,
                UserManager<ApplicationUser> userManager,
                RoleManager<ApplicationRole> roleManager) =>
            {
                Response<TokenResponse> res = await externalAuthService.GetTokenAsync(request, CancellationToken.None).ConfigureAwait(false);
                return Results.Json(res, statusCode: res.StatusCode);
            }).AllowAnonymous();
        }
    }
}
