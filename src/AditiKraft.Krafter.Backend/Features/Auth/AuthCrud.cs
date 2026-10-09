using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using AditiKraft.Krafter.Backend.Common.Tenants;
using AditiKraft.Krafter.Backend.Features.Auth.Common;
using AditiKraft.Krafter.Backend.Features.Roles.Common;
using AditiKraft.Krafter.Backend.Features.Users.Common;
using AditiKraft.Krafter.Backend.Infrastructure.Jobs;
using AditiKraft.Krafter.Backend.Infrastructure.Notifications;
using AditiKraft.Krafter.Backend.Infrastructure.Persistence;
using AditiKraft.Krafter.Backend.Web;
using AditiKraft.Krafter.Backend.Web.Configuration;
using AditiKraft.Krafter.Contracts.Common;
using AditiKraft.Krafter.Contracts.Common.Auth;
using AditiKraft.Krafter.Contracts.Common.Extensions;
using AditiKraft.Krafter.Contracts.Common.Models;
using AditiKraft.Krafter.Contracts.Contracts.Auth;
using AditiKraft.Krafter.Contracts.Contracts.Roles;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace AditiKraft.Krafter.Backend.Features.Auth;

public sealed class AuthCrud
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

            Dictionary<string, string?> tokenRequestParams = new Dictionary<string, string?>
            {
                ["client_id"] = clientId,
                ["client_secret"] = clientSecret,
                ["code"] = code,
                ["grant_type"] = "authorization_code",
                ["redirect_uri"] = redirectUri
            };

            HttpResponseMessage response = await httpClient.PostAsync(
                "https://oauth2.googleapis.com/token",
                new FormUrlEncodedContent(tokenRequestParams!)).ConfigureAwait(false);

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
            HttpRequestMessage requestMessage =
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
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        ApplicationDbContext applicationDbContext,
        ITokenService tokenService,
        ITenantGetterService tenantGetterService,
        IPasswordPolicyService passwordPolicyService,
        IJobService jobService,
        IEmailService emailService,
        GoogleAuthClient googleAuthClient,
        IOptions<SecuritySettings> securitySettings,
        IOptions<JwtSettings> jwtSettings) : IScopedHandler
    {
        private readonly SecuritySettings _securitySettings = securitySettings.Value;
        private readonly JwtSettings _jwtSettings = jwtSettings.Value;

        public async Task<Response<TokenResponse>> LoginAsync(
            TokenRequest request,
            string ipAddress,
            CancellationToken cancellationToken)
        {
            ApplicationUser? user = await userManager.FindByEmailAsync(request.Email.Trim().Normalize()).ConfigureAwait(false);
            if (user is null)
            {
                return Response<TokenResponse>.Unauthorized("Invalid Email or Password");
            }

            if (!await userManager.CheckPasswordAsync(user, request.Password).ConfigureAwait(false))
            {
                return Response<TokenResponse>.BadRequest("Invalid Email or Password");
            }

            if (!user.IsActive)
            {
                return Response<TokenResponse>.BadRequest("User Not Active. Please contact the administrator.");
            }

            if (_securitySettings.RequireConfirmedAccount && !user.EmailConfirmed)
            {
                return Response<TokenResponse>.BadRequest("E-Mail not confirmed.");
            }

            if (await passwordPolicyService.IsPasswordExpiredAsync(user, cancellationToken).ConfigureAwait(false))
            {
                if (await passwordPolicyService.ShouldSendPasswordExpiryNotificationAsync(user, cancellationToken).ConfigureAwait(false) &&
                    !string.IsNullOrWhiteSpace(user.Email))
                {
                    string subject = "Password Expired";
                    string userName = $"{user.FirstName} {user.LastName}";
                    string htmlBody = $@"
<html>
<head>
    <title>Password Expired</title>
</head>
<body>
    <p>Hello {userName},</p>
    <p>Your password has expired and must be reset before you can sign in.</p>
    <p>Please use the <strong>Forgot Password</strong> option on the login page to set a new password.</p>
    <p>Best regards,</p>
    <p>{tenantGetterService.Tenant.Name} Team</p>
</body>
</html>";

                    string emailLogId = await emailService.QueueEmailAsync(
                        new SendEmailRequestInput { Email = user.Email, Subject = subject, HtmlMessage = htmlBody },
                        cancellationToken).ConfigureAwait(false);
                    await jobService.EnqueueByLogIdAsync(emailLogId, nameof(Jobs.SendEmailJobAsync), cancellationToken).ConfigureAwait(false);

                    passwordPolicyService.MarkPasswordExpiryNotificationSent(user);
                    _ = await userManager.UpdateAsync(user).ConfigureAwait(false);
                }

                return Response<TokenResponse>.Forbidden("Password expired. Please reset your password.");
            }

            return Response<TokenResponse>.Success(await tokenService.GenerateTokensAndUpdateUser(user, ipAddress).ConfigureAwait(false));
        }

        public async Task<Response<TokenResponse>> RefreshTokenAsync(
            RefreshTokenRequest request,
            string ipAddress,
            CancellationToken cancellationToken)
        {
            ClaimsPrincipal? userPrincipal = GetPrincipalFromExpiredToken(request.Token);
            if (userPrincipal is null)
            {
                return Response<TokenResponse>.Unauthorized("Invalid token.");
            }

            string? userEmail = userPrincipal.GetEmail();
            if (string.IsNullOrEmpty(userEmail))
            {
                return Response<TokenResponse>.Unauthorized("Invalid token.");
            }

            ApplicationUser? user = await userManager.FindByEmailAsync(userEmail).ConfigureAwait(false);
            if (user is null)
            {
                return Response<TokenResponse>.Unauthorized("Authentication failed.");
            }

            UserRefreshToken? refreshToken = await applicationDbContext.UserRefreshTokens
                .FirstOrDefaultAsync(current => current.UserId == user.Id, cancellationToken).ConfigureAwait(false);

            if (refreshToken is null ||
                refreshToken.RefreshToken != request.RefreshToken ||
                refreshToken.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                return Response<TokenResponse>.Unauthorized("Invalid or expired refresh token.");
            }

            return Response<TokenResponse>.Success(await tokenService.GenerateTokensAndUpdateUser(user, ipAddress).ConfigureAwait(false));
        }

        public async Task<Response> RegisterAsync(SelfRegisterRequest request)
        {
            CurrentTenantDetails currentTenant = tenantGetterService.Tenant;
            if (string.IsNullOrWhiteSpace(currentTenant.Id) || string.IsNullOrWhiteSpace(currentTenant.Identifier))
            {
                return Response.BadRequest("Tenant context is missing.");
            }

            if (string.IsNullOrWhiteSpace(request.TenantId) == false &&
                string.Equals(request.TenantId, currentTenant.Identifier, StringComparison.OrdinalIgnoreCase) == false)
            {
                return Response.BadRequest("Tenant context does not match the registration request.");
            }

            ApplicationUser? existingUser = await userManager.FindByEmailAsync(request.Email.Trim().Normalize()).ConfigureAwait(false);
            if (existingUser is not null)
            {
                return Response.BadRequest("Email is already registered.");
            }

            ApplicationUser? existingByUserName = await userManager.FindByNameAsync(request.UserName.Trim().Normalize()).ConfigureAwait(false);
            if (existingByUserName is not null)
            {
                return Response.BadRequest("Username is already taken.");
            }

            ApplicationUser user = new ApplicationUser
            {
                Id = Guid.CreateVersion7().ToString(),
                TenantId = currentTenant.Id,
                UserName = request.UserName.Trim(),
                FirstName = request.FirstName.Trim(),
                LastName = request.LastName.Trim(),
                Email = request.Email.Trim(),
                PhoneNumber = request.PhoneNumber,
                IsActive = true,
                EmailConfirmed = false,
                IsDeleted = false,
                CreatedOn = DateTime.UtcNow
            };

            IdentityResult createResult = await userManager.CreateAsync(user, request.Password).ConfigureAwait(false);
            if (!createResult.Succeeded)
            {
                return Response.BadRequest(string.Join(" ", createResult.Errors.Select(error => error.Description)));
            }

            ApplicationRole? basicRole = await roleManager.FindByNameAsync(RoleConstants.Basic).ConfigureAwait(false);
            if (basicRole is null)
            {
                await userManager.DeleteAsync(user).ConfigureAwait(false);
                return Response.BadRequest("Default user role is not configured.");
            }

            IdentityResult addRoleResult = await userManager.AddToRoleAsync(user, basicRole.Name!).ConfigureAwait(false);
            if (!addRoleResult.Succeeded)
            {
                await userManager.DeleteAsync(user).ConfigureAwait(false);
                return Response.BadRequest(string.Join(" ", addRoleResult.Errors.Select(error => error.Description)));
            }

            return Response.Success("Registration successful.");
        }

        public async Task<Response<TokenResponse>> GoogleAuthAsync(
            GoogleAuthRequest request,
            CancellationToken cancellationToken)
        {
            GoogleAuthClient.GoogleTokens? tokens = await googleAuthClient.ExchangeCodeForTokensAsync(request.Code).ConfigureAwait(false);
            if (tokens is null)
            {
                return Response<TokenResponse>.Unauthorized("Invalid token");
            }

            GoogleAuthClient.GoogleUserInfo? userInfo = await googleAuthClient.GetUserInfoAsync(tokens.AccessToken).ConfigureAwait(false);
            if (userInfo is null)
            {
                return Response<TokenResponse>.Unauthorized("Invalid user info");
            }

            ApplicationUser? user = await userManager.FindByEmailAsync(userInfo.Email).ConfigureAwait(false);
            if (user is null)
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

                applicationDbContext.UserRoles.Add(new ApplicationUserRole { RoleId = basic.Id, UserId = user.Id });
                await applicationDbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }

            return await tokenService.GenerateTokensAndUpdateUser(user.Id, string.Empty).ConfigureAwait(false);
        }

        private ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
        {
            TokenValidationParameters tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key)),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false,
                ClockSkew = TimeSpan.Zero
            };

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            ClaimsPrincipal principal =
                tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                    StringComparison.InvariantCultureIgnoreCase))
            {
                return null;
            }

            return principal;
        }
    }

    public sealed class Route : IRouteRegistrar
    {
        public void MapRoute(IEndpointRouteBuilder endpointRouteBuilder)
        {
            RouteGroupBuilder tokenGroup = endpointRouteBuilder.MapGroup(ApiRoutes.Tokens)
                .AddFluentValidationFilter();

            tokenGroup.MapPost("/", async (
                [FromBody] TokenRequest request,
                HttpContext context,
                [FromServices] Handler handler,
                CancellationToken cancellationToken) =>
            {
                if (string.IsNullOrWhiteSpace(request.TenantId))
                {
                    request.TenantId = context.Request.Headers["x-tenant-identifier"].ToString();
                }

                string ipAddress = GetIpAddress(context);
                Response<TokenResponse> response = await handler.LoginAsync(request, ipAddress, cancellationToken).ConfigureAwait(false);
                return Results.Json(response, statusCode: response.StatusCode);
            }).Produces<Response<TokenResponse>>();

            tokenGroup.MapPost($"/{RouteSegment.Refresh}", async (
                    [FromBody] RefreshTokenRequest request,
                    HttpContext context,
                    [FromServices] Handler handler,
                    CancellationToken cancellationToken) =>
                {
                    string ipAddress = GetIpAddress(context);
                    Response<TokenResponse> response = await handler.RefreshTokenAsync(request, ipAddress, cancellationToken)
                        .ConfigureAwait(false);
                    return Results.Json(response, statusCode: response.StatusCode);
                })
                .Produces<Response<TokenResponse>>()
                .AllowAnonymous();

            tokenGroup.MapPost("/self-register", async (
                    [FromBody] SelfRegisterRequest request,
                    [FromServices] Handler handler,
                    CancellationToken cancellationToken) =>
                {
                    Response response = await handler.RegisterAsync(request).ConfigureAwait(false);
                    return Results.Json(response, statusCode: response.StatusCode);
                })
                .Produces<Response>();

            RouteGroupBuilder externalAuthGroup = endpointRouteBuilder.MapGroup(ApiRoutes.ExternalAuth)
                .AddFluentValidationFilter();

            externalAuthGroup.MapPost($"/{RouteSegment.Google}", async (
                    [FromBody] GoogleAuthRequest request,
                    [FromServices] Handler handler,
                    CancellationToken cancellationToken) =>
                {
                    Response<TokenResponse> response = await handler.GoogleAuthAsync(request, cancellationToken)
                        .ConfigureAwait(false);
                    return Results.Json(response, statusCode: response.StatusCode);
                })
                .Produces<Response<TokenResponse>>()
                .AllowAnonymous();
        }

        private static string GetIpAddress(HttpContext httpContext)
        {
            if (httpContext.Request.Headers.TryGetValue("X-Forwarded-For", out Microsoft.Extensions.Primitives.StringValues value))
            {
                return value.ToString();
            }

            return httpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString() ?? "N/A";
        }
    }
}
