using System.Net.Http.Json;
using ApiTemplate.Infrastructure.ExternalClients.ExampleHttpClient.Dtos;

namespace ApiTemplate.Infrastructure.ExternalClients.ExampleHttpClient;

/// <summary>Typed HTTP client for communicating with the example external API, with Polly retry resilience.</summary>
public class ExampleHttpClient : IExampleHttpClient
{
    private readonly HttpClient _httpClient;

    /// <summary>Initializes a new instance with the configured HTTP client.</summary>
    public ExampleHttpClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <inheritdoc/>
    public async Task<ExampleApiResponse?> GetAsync(string id, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync($"resources/{id}", cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<ExampleApiResponse>(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<ExampleApiResponse?> PostAsync(string payload, CancellationToken cancellationToken = default)
    {
        var content = new StringContent(payload, System.Text.Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("resources", content, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<ExampleApiResponse>(cancellationToken);
    }
}
