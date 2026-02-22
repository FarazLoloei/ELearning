using ELearning.Application.Common.Behaviors;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace ELearning.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Register MediatR
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
        });

        // Register AutoMapper
        services.AddAutoMapper(_ => { }, Assembly.GetExecutingAssembly());

        // Register FluentValidation
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly(), includeInternalTypes: true);

        // Register MediatR pipeline behaviors
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>));

        return services;
    }
}


