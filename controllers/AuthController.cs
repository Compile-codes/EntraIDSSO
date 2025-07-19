using System;
using System.Threading.Tasks;

public class AuthController
{
    /// <summary>
    /// Initiates the authentication process by constructing the authorization request URI
    /// and printing it to the console. The user is instructed to manually visit this URL
    /// in their browser to log in with Entra ID. This method does not involve any
    /// asynchronous operations or external calls.
    /// </summary>
    public static void RedirectToEntraID()
    {
        var authority = "https://login.microsoftonline.com/{tenantId}";
        var clientId = "{clientId}";
        var redirectUri = "http://localhost:5000/callback";

        var authorizationRequestUri = $"{authority}/oauth2/v2.0/authorize" +
            $"?client_id={clientId}" +
            $"&response_type=code" +
            $"&redirect_uri={Uri.EscapeDataString(redirectUri)}" +
            $"&scope=openid profile" +
            $"&state=some_state";

        Console.WriteLine("Please visit the following URL to log in: " + authorizationRequestUri);
        Console.WriteLine("After logging in, the browser will redirect, and I will capture the authorization code.");
        Console.WriteLine("Press any key to exit after you've authenticated in the browser and the callback has been processed.");
    }

    /// <summary>
    /// Processes the callback from Microsoft Entra ID, receiving the authorization code.
    /// It then attempts to exchange this code for an access token using <see cref="AuthService.ExchangeCodeForAccessToken(string)"/>.
    /// Upon successful acquisition, the access token is validated, and the user session is handled
    /// via <see cref="HandleUserSession(string)"/>. Error messages are displayed to the console
    /// if the code is missing or token acquisition/validation fails.
    /// </summary>
    /// <param name="code">The authorization code provided by Microsoft Entra ID in the callback.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation of handling the callback.</returns>
    public static async Task HandleCallback(string code)
    {
        if (string.IsNullOrEmpty(code))
        {
            Console.WriteLine("Authorization code is missing.");
            return;
        }

        Console.WriteLine($"Received authorization code: {code}");

        string? accessToken = await AuthService.ExchangeCodeForAccessToken(code);
        if (string.IsNullOrEmpty(accessToken))
        {
            Console.WriteLine("Failed to acquire access token.");
            return;
        }

        if (AuthService.ValidateToken(accessToken))
        {
            Console.WriteLine("Access token is valid!");
            await HandleUserSession(accessToken);
            Console.WriteLine("Access token acquired successfully! Redirecting user to the main application...");
        }
        else
        {
            Console.WriteLine("Access token is INVALID. Aborting session.");
        }
    }

    /// <summary>
    /// Handles the user's session once a valid access token has been obtained.
    /// This method is designed for asynchronous operations related to managing the user's state,
    /// such as storing the token, retrieving user profile information, or setting up application-specific sessions.
    /// For demonstration, it simply prints the access token.
    /// </summary>
    /// <param name="accessToken">The valid access token string for the authenticated user.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation of session management.</returns>
    private static async Task HandleUserSession(string accessToken)
    {
        Console.WriteLine("Storing the access token in session (example).");
        Console.WriteLine($"Access token: {accessToken}");

        await Task.CompletedTask;
    }
}