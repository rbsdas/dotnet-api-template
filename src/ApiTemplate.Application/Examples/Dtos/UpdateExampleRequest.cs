namespace ApiTemplate.Application.Examples.Dtos;

/// <summary>Request payload for updating an existing example.</summary>
public record UpdateExampleRequest(
    string Title,
    string Description,
    ExampleStatus Status
);
