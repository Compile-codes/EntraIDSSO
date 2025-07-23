using Microsoft.Identity.Client;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

public class AuthService
{
    private static string tenantId = "{tenantId}";
    private static string clientId = "{clientId}";

    /// <summary>
    /// Exchanges an authorization code received from Microsoft Entra ID for an access token.
    /// This method builds a confidential client application and uses MSAL.NET to perform
    /// the OAuth 2.0 authorization code flow. It requires the client secret to be
    /// set as an environment variable named "AZURE_CLIENT_SECRET".
    /// The requested scopes include standard OIDC scopes ("openid", "profile"),
    /// an optional refresh token scope ("offline_access"), and the custom API scope
    /// ("api://c6fccf6a-40e2-4a9a-8887-60b86c621e43/access_as_user") required to access a protected API.
    /// </summary>
    /// <param name="authorizationCode">The authorization code obtained from the Entra ID callback.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> that represents the asynchronous operation.
    /// The task result contains the acquired access token as a <see cref="string"/>,
    /// or <see langword="null"/> if the token acquisition fails due to missing client secret,
    /// MSAL errors, or other exceptions.
    /// </returns>
    public static async Task<string?> ExchangeCodeForAccessToken(string authorizationCode)
    {
        string? clientSecret = Environment.GetEnvironmentVariable("AZURE_CLIENT_SECRET");
        if (string.IsNullOrEmpty(clientSecret))
        {
            Console.WriteLine("Error: AZURE_CLIENT_SECRET environment variable is not set.");
            Console.WriteLine("Please set it to your Azure AD Application Client Secret.");
            return null;
        }

        string redirectUri = "http://localhost:5000/callback";

        try
        {
            var cca = ConfidentialClientApplicationBuilder.Create(clientId)
                .WithClientSecret(clientSecret)
                .WithAuthority(new Uri($"https://login.microsoftonline.com/{tenantId}"))
                .WithRedirectUri(redirectUri)
                .Build();

            var scopes = new string[]
            {
                "openid",
                "profile",
                "offline_access"
            };

            var result = await cca.AcquireTokenByAuthorizationCode(scopes, authorizationCode)
                .ExecuteAsync();

            string accessToken = result.AccessToken;
            return accessToken;
        }
        catch (MsalException ex)
        {
            Console.WriteLine($"MSAL Error acquiring token: {ex.ErrorCode} - {ex.Message}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
            }
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"General Error acquiring token: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Performs basic validation of an access token obtained from Microsoft Entra ID.
    /// This method checks the token's readability as a JWT, its issuer, its audience,
    /// and its expiration time.
    /// </summary>
    /// <remarks>
    /// For production-grade security, full cryptographic signature validation using
    /// the Identity Provider's JSON Web Key Set (JWKS) endpoint is essential.
    /// This basic validation is for demonstration purposes and focuses on
    /// structural integrity, issuer, audience, and expiry claims.
    /// </remarks>
    /// <param name="accessToken">The access token string to be validated. Can be <see langword="null"/> or empty.</param>
    /// <returns>
    /// <see langword="true"/> if the token passes the basic validation checks;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    public static bool ValidateToken(string? accessToken)
    {
        if (string.IsNullOrEmpty(accessToken))
        {
            Console.WriteLine("Access token is null or empty for validation.");
            return false;
        }

        try
        {
            var handler = new JwtSecurityTokenHandler();

            if (!handler.CanReadToken(accessToken))
            {
                Console.WriteLine("Token is not a readable JWT.");
                return false;
            }

            var jsonToken = handler.ReadToken(accessToken) as JwtSecurityToken;

            if (jsonToken == null)
            {
                Console.WriteLine("Failed to parse token as JwtSecurityToken.");
                return false;
            }

            var validIssuer = $"https://sts.windows.net/{tenantId}/";
            var expectedAudience = $"api://{clientId}";

            if (jsonToken.Issuer != validIssuer)
            {
                Console.WriteLine($"Token Issuer Mismatch: Expected '{validIssuer}', Got '{jsonToken.Issuer}'");
                return false;
            }

            if (!jsonToken.Audiences.Contains(expectedAudience))
            {
                Console.WriteLine($"Token Audience Mismatch: Expected '{expectedAudience}', Not found in [{string.Join(", ", jsonToken.Audiences)}]");
                return false;
            }

            var expiryClaim = jsonToken.Claims.FirstOrDefault(c => c.Type == "exp");
            if (expiryClaim != null && long.TryParse(expiryClaim.Value, out long expirationUnixTime))
            {
                var expirationTime = DateTimeOffset.FromUnixTimeSeconds(expirationUnixTime).UtcDateTime;
                if (expirationTime < DateTime.UtcNow)
                {
                    Console.WriteLine($"Token Expired: Expiration Time: {expirationTime.ToLocalTime()}, Current UTC: {DateTime.UtcNow}");
                    return false;
                }
            }
            else
            {
                Console.WriteLine("Warning: 'exp' claim not found or not parsable in token.");
            }

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error validating token: {ex.Message}");
            return false;
        }
    }
}