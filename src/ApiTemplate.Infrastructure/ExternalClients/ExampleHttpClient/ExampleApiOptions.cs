namespace ApiTemplate.Infrastructure.ExternalClients.ExampleHttpClient;

/// <summary>Configuration for the example external API client.</summary>
public class ExampleApiOptions
{
    /// <summary>The configuration section name.</summary>
    public const string SectionName = "ExampleApi";

    /// <summary>Gets or sets the base URL of the external API.</summary>
    public string BaseUrl { get; init; } = string.Empty;

    /// <summary>Gets or sets the API key for authenticating with the external API.</summary>
    public string ApiKey { get; init; } = string.Empty;
}
