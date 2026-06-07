namespace ApiTemplate.Application.Examples.Dtos;

/// <summary>Response DTO for an example resource.</summary>
public record ExampleResponse(
    Guid Id,
    string Title,
    string Description,
    ExampleStatus Status,
    Guid CreatedByUserId,
    DateTime CreatedAt,
    DateTime UpdatedAt
);
