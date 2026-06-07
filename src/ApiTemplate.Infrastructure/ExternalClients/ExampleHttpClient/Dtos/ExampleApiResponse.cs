namespace ApiTemplate.Infrastructure.ExternalClients.ExampleHttpClient.Dtos;

/// <summary>Response DTO received from the example external API.</summary>
public record ExampleApiResponse(
    string Id,
    string Name,
    string Data
);
