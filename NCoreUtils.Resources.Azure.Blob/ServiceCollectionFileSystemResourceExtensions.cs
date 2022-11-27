using Microsoft.Extensions.DependencyInjection;
using NCoreUtils.Resources;

namespace NCoreUtils;

public static class ServiceCollectionAzureBlobResourceExtensions
{
    /// <summary>
    /// Adds azure blob storage resource factory as STANDALONE implementation of
    /// <see cref="IResourceFactory" />.
    /// </summary>
    /// <param name="services">Service collection.</param>
    /// <returns>Service collection.</returns>
    public static IServiceCollection AddStandaloneAzureBlobResourceFactory(this IServiceCollection services)
        => services.AddSingleton<IResourceFactory, AzureBlobResourceFactory>();
}