using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NCoreUtils.Google;
using NCoreUtils.Resources;

namespace NCoreUtils;

public static class ServiceCollectionGoogleCloudStorageResourceExtensions
{
    /// <summary>
    /// Adds Google cloud storage resource factory as STANDALONE implementation of
    /// <see cref="IResourceFactory" />.
    /// </summary>
    /// <param name="services">Service collection.</param>
    /// <param name="passthrough">Whether to use passthrough mode.</param>
    /// <param name="publicByDefault">Whether to mark resources as public when Uri does not contain publicity information.</param>
    /// <returns>Service collection.</returns>
    public static IServiceCollection AddStandaloneGoogleCloudStorageResourceFactory(
        this IServiceCollection services,
        bool passthrough = false,
        bool publicByDefault = false)
    {
        return services.AddSingleton<IResourceFactory>(serviceProvider =>
        {
            var utils = serviceProvider.GetRequiredService<GoogleCloudStorageUtils>();
            var accessTokenProvider = serviceProvider.GetService<IGoogleAccessTokenProvider>();
            var logger = serviceProvider.GetRequiredService<ILogger<GoogleCloudStorageResourceFactory>>();
            return new GoogleCloudStorageResourceFactory(utils, accessTokenProvider, logger, passthrough, publicByDefault);
        });
    }

    /// <summary>
    /// Adds Google cloud storage resource factory as STANDALONE implementation of
    /// <see cref="IResourceFactory" />.
    /// </summary>
    /// <param name="services">Service collection.</param>
    /// <param name="passthrough">Whether to use passthrough mode.</param>
    /// <returns>Service collection.</returns>
    public static IServiceCollection AddStandaloneGoogleCloudStorageResourceFactory(
        this IServiceCollection services,
        bool passthrough = false)
        => services.AddStandaloneGoogleCloudStorageResourceFactory(passthrough, publicByDefault: false);
}