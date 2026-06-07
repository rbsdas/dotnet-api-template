using ApiTemplate.Infrastructure.ExternalClients.ExampleHttpClient.Dtos;

namespace ApiTemplate.Infrastructure.ExternalClients.ExampleHttpClient;

/// <summary>Contract for communicating with the example external API.</summary>
public interface IExampleHttpClient
{
    /// <summary>Retrieves a resource from the external API by ID.</summary>
    Task<ExampleApiResponse?> GetAsync(string id, CancellationToken cancellationToken = default);

    /// <summary>Posts data to the external API and returns the response.</summary>
    Task<ExampleApiResponse?> PostAsync(string payload, CancellationToken cancellationToken = default);
}
