using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

using Microsoft.AspNetCore.Http;

using Pecule.Accounts.Contracts;

namespace Pecule.IntegrationTests;

public sealed class AccountsEndpointsTests(PeculeApiFactory factory) : IClassFixture<PeculeApiFactory>
{
    // Same JsonSerializerOptions as the API to avoid discrepancies in serialization
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() },
    };

    // Unique email generator for test accounts to avoid conflicts
    private static string NewEmail() => $"{Guid.NewGuid()}@test.local";

    [Fact]
    public async Task GetAccountsWithoutLoginReturnsUnauthorized()
    {
        // Arrange : anonymous client without authentication
        using var client = factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/accounts");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreatedAccountAppearsInAccountList()
    {
        // Arrange : authenticated client
        using var client = await AuthenticatedClient.CreateAsync(factory, NewEmail());
        var request = new CreateAccountRequest("Compte courant", AccountType.Checking, 1250.50m);

        // Act : create a new account
        var createResponse = await client.PostAsJsonAsync("/api/accounts", request, JsonOptions);

        // Act : retrieve the list of accounts
        var listResponse = await client.GetAsync("/api/accounts");
        var accounts = await listResponse.Content.ReadFromJsonAsync<List<AccountDto>>(JsonOptions);

        // Assert : the account creation was successful
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        Assert.NotNull(accounts);
        var account = Assert.Single(accounts);
        Assert.Equal(request.Name, account.Name);
        Assert.Equal(request.Type, account.Type);
        Assert.Equal(request.InitialBalance, account.InitialBalance);
        Assert.Equal("EUR", account.Currency);
    }

    [Fact]
    public async Task UserCannotSeeAccountsOfAnotherUser()
    {
        // Arrange : authenticated client
        using var clientA = await AuthenticatedClient.CreateAsync(factory, NewEmail());
        using var clientB = await AuthenticatedClient.CreateAsync(factory, NewEmail());

        var request = new CreateAccountRequest("Compte courant", AccountType.Checking, 1150.25m);

        // Act : Client A create bank account
        var createResponseClientA = await clientA.PostAsJsonAsync("/api/accounts", request, JsonOptions);
        var listeResponseA = await clientA.GetAsync("/api/accounts");
        var accounts = await listeResponseA.Content.ReadFromJsonAsync<List<AccountDto>>(JsonOptions);

        // Assert : Creation for clientA account succesful
        Assert.Equal(HttpStatusCode.Created, createResponseClientA.StatusCode);
        Assert.NotNull(accounts);
        var accountClientA = Assert.Single(accounts);
        Assert.Equal(request.Name, accountClientA.Name);

        // But Client B can't read it
        var listResponseB = await clientB.GetAsync("/api/accounts");
        var accountsForB = await listResponseB.Content.ReadFromJsonAsync<List<AccountDto>>(JsonOptions);

        Assert.Equal(HttpStatusCode.OK, listResponseB.StatusCode);
        Assert.NotNull(accountsForB);
        Assert.Empty(accountsForB);
    }

    [Fact]
    public async Task InvalidCreationRequestReturnError()
    {
        // Arrange : authenticated client
        using var client = await AuthenticatedClient.CreateAsync(factory, NewEmail());
        var request = new CreateAccountRequest(string.Empty, AccountType.Checking, 1250.50m);

        // Act : client create an account with no name
        var createResponse = await client.PostAsJsonAsync("/api/accounts", request, JsonOptions);
        var problem = await createResponse.Content.ReadFromJsonAsync<HttpValidationProblemDetails>(JsonOptions);

        Assert.Equal(HttpStatusCode.BadRequest, createResponse.StatusCode);
        Assert.NotNull(problem);
        Assert.Contains("Name", problem.Errors.Keys);
    }

    [Fact]
    public async Task CreateAccountWithoutXsrfTokenIsRejected()
    {
        // Arrange : authenticated client
        using var client = await AuthenticatedClient.CreateAsync(factory, NewEmail());
        var request = new CreateAccountRequest("Compte courant", AccountType.Checking, 1250.50m);

        client.DefaultRequestHeaders.Remove("X-XSRF-TOKEN");

        // Act : trying to post in accounts without xsrf token
        var createResponse = await client.PostAsJsonAsync("/api/accounts", request, JsonOptions);
        var problem = await createResponse.Content.ReadFromJsonAsync<HttpValidationProblemDetails>(JsonOptions);

        // Assert : Bad request and anti-forgery token missing
        Assert.Equal(HttpStatusCode.BadRequest, createResponse.StatusCode);
        Assert.NotNull(problem);
        Assert.Empty(problem.Errors);
        Assert.Contains("Antiforgery", problem.Detail);

        // But we have to verify none account created during process
        var listResponse = await client.GetAsync("/api/accounts");
        var accounts = await client.GetFromJsonAsync<List<AccountDto>>("/api/accounts", JsonOptions);

        Assert.NotNull(accounts);
        Assert.Empty(accounts);
    }

    [Fact]
    public async Task DeletedUserCannotLogInAgain()
    {
        var email = NewEmail();

        // Arrange : authenticated client
        using var client = await AuthenticatedClient.CreateAsync(factory, email);

        // Act : Delete current user
        var deleteResponse = await client.DeleteAsync("/api/auth/account");
        await AuthenticatedClient.RefreshXsrfTokenAsync(client);
        var loginResponse = await client.PostAsJsonAsync("/api/auth/login?useCookies=true", new { email, password = AuthenticatedClient.Password });

        // Assert : Client succesfully deleted and can't login
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, loginResponse.StatusCode); // If StatusCode is 401 user if definitely deleted not related to anti-forgery token
    }
}
