using Microsoft.Extensions.DependencyInjection;
using NCoreUtils.Resources;

namespace NCoreUtils;

public static class ServiceCollectionAspNetCoreResourceExtensions
{
    /// <summary>
    /// Adds ASP.NET Core resource factory as STANDALONE implementation of <see cref="IResourceFactory" />.
    /// </summary>
    /// <param name="services">Service collection.</param>
    /// <returns>Service collection.</returns>
    public static IServiceCollection AddAspNetCoreResourceFactory(this IServiceCollection services)
        => services.AddSingleton<IResourceFactory, AspNetCoreResourceFactory>();
}