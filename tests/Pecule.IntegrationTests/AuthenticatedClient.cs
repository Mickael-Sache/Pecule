using System.Net.Http.Json;

namespace Pecule.IntegrationTests;

internal static class AuthenticatedClient
{
    internal const string Password = "Pecule-Test-2026!";

    public static async Task<HttpClient> CreateAsync(PeculeApiFactory factory, string email)
    {
        var client = factory.CreateClient();   // Keep cookies between requests to maintain the session

        await RefreshXsrfTokenAsync(client);
        (await client.PostAsJsonAsync("/api/auth/register", new { email, Password })).EnsureSuccessStatusCode();
        (await client.PostAsJsonAsync("/api/auth/login?useCookies=true", new { email, Password })).EnsureSuccessStatusCode();

        await RefreshXsrfTokenAsync(client);   // Token linked to the session cookie, so we need to refresh it after login
        return client;
    }

    internal static async Task RefreshXsrfTokenAsync(HttpClient client)
    {
        var response = await client.GetAsync("/api/antiforgery/token");
        response.EnsureSuccessStatusCode();

        var token = response.Headers.GetValues("Set-Cookie")
            .Select(cookie => cookie.Split(';')[0].Split('=', 2))
            .Single(parts => parts[0] == "XSRF-TOKEN")[1];

        client.DefaultRequestHeaders.Remove("X-XSRF-TOKEN");
        client.DefaultRequestHeaders.Add("X-XSRF-TOKEN", token);
    }
}
