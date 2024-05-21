using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NCoreUtils.Resources;

namespace NCoreUtils;

public static class ServiceCollectionCompositeResourceFactoryExtensions
{
    public static IServiceCollection AddCompositeResourceFactory(
        this IServiceCollection services,
        Action<OptionsBuilder<CompositeResourceFactoryConfiguration>>? configure)
    {
        var opts = services.AddOptions<CompositeResourceFactoryConfiguration>();
        configure?.Invoke(opts);
        return services
            .AddSingleton<IResourceFactory, CompositeResourceFactory>();
    }
}