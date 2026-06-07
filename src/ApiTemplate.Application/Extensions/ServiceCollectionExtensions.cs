using ApiTemplate.Application.Auth;
using ApiTemplate.Application.Auth.Validators;
using ApiTemplate.Application.Examples;
using ApiTemplate.Application.Examples.Validators;
using ApiTemplate.Application.Auth.Dtos;
using ApiTemplate.Application.Examples.Dtos;

namespace ApiTemplate.Application.Extensions;

/// <summary>Registers all Application-layer services into the DI container.</summary>
public static class ServiceCollectionExtensions
{
    /// <summary>Adds application services, validators, and FluentValidation integration.</summary>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IExampleService, ExampleService>();
        services.AddScoped<IAuthService, AuthService>();

        services.AddScoped<IValidator<CreateExampleRequest>, CreateExampleRequestValidator>();
        services.AddScoped<IValidator<UpdateExampleRequest>, UpdateExampleRequestValidator>();
        services.AddScoped<IValidator<LoginRequest>, LoginRequestValidator>();
        services.AddScoped<IValidator<RegisterRequest>, RegisterRequestValidator>();

        return services;
    }
}
