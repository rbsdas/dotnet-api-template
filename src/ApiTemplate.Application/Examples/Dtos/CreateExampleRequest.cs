namespace ApiTemplate.Application.Examples.Dtos;

/// <summary>Request payload for creating a new example.</summary>
public record CreateExampleRequest(
    string Title,
    string Description,
    ExampleStatus Status = ExampleStatus.Draft
);
