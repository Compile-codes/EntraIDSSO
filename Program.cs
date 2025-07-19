using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;

public class Program
{
    /// <summary>
    /// The asynchronous main method that initializes and runs the web host,
    /// initiates the Entra ID authentication redirection, and awaits the callback.
    /// It configures a Kestrel server to listen on `http://localhost:5000`
    /// and defines an endpoint (`/callback`) to process the authorization code.
    /// </summary>
    /// <param name="args">Command-line arguments passed to the application.</param>
    /// <returns>A <see cref="Task"/> that represents the asynchronous operation of the application's execution.</returns>
    public static async Task Main(string[] args)
    {
        Console.WriteLine("Starting EntraID SSO Application...");

        var host = Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseKestrel()
                    .UseUrls("http://localhost:5000")
                    .Configure(app =>
                    {
                        app.MapWhen(context => context.Request.Path == "/callback", appBranch =>
                        {
                            appBranch.Run(async context =>
                            {
                                Console.WriteLine("\nReceived request at /callback endpoint.");
                                if (context.Request.Query.TryGetValue("code", out var codeValue))
                                {
                                    string code = codeValue.ToString();
                                    Console.WriteLine($"Received authorization code: {code}");
                                    await AuthController.HandleCallback(code);

                                    await context.Response.WriteAsync("Authorization code processed. Check console for details. You can close this browser tab.");
                                }
                                else
                                {
                                    Console.WriteLine("Callback received, but no authorization code found in query parameters.");
                                    await context.Response.WriteAsync("Error: Authorization code not found in callback.");
                                }
                            });
                        });

                        app.Run(async context =>
                        {
                            await context.Response.WriteAsync("SSO Application Running. Waiting for EntraID authentication callback...");
                        });
                    });
            })
            .Build();

        var listenerTask = host.RunAsync();

        AuthController.RedirectToEntraID();

        Console.WriteLine("\nKestrel server is running. Awaiting callback...");
        Console.ReadKey();

        await host.StopAsync();
        Console.WriteLine("Application shutting down.");
    }
}