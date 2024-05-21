using System;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NCoreUtils.Google;
using NCoreUtils.Resources;

namespace NCoreUtils;

public static class OptionsBuilderGoogleCloudStorageExtensions
{
    /// <summary>
    /// Adds google cloud storage resource factory to a composite resource factory.
    /// </summary>
    /// <param name="optionsBuilder">Options builder.</param>
    /// <param name="passthrough">Whether to use passthrough mode.</param>
    /// <param name="publicByDefault">Whether to mark resources as public when Uri does not contain publicity information.</param>
    /// <returns>Options builder.</returns>
    public static OptionsBuilder<CompositeResourceFactoryConfiguration> AddGoogleCloudStorageResourceFactory(
        this OptionsBuilder<CompositeResourceFactoryConfiguration> optionsBuilder,
        bool passthrough = false,
        bool publicByDefault = false)
    {
        optionsBuilder.Configure<IServiceProvider>((config, serviceProvider) =>
        {
            var utils = serviceProvider.GetService<GoogleCloudStorageUtils>() switch
            {
                null => new GoogleCloudStorageUtils(serviceProvider.GetService<IHttpClientFactory>()),
                var instance => instance
            };
            var accessTokenProvider = serviceProvider.GetService<IGoogleAccessTokenProvider>();
            var logger = serviceProvider.GetRequiredService<ILogger<GoogleCloudStorageResourceFactory>>();
            config.AddFactory(new GoogleCloudStorageResourceFactory(utils, accessTokenProvider, logger, passthrough, publicByDefault));
        });
        return optionsBuilder;
    }

    /// <summary>
    /// Adds google cloud storage resource factory to a composite resource factory.
    /// </summary>
    /// <param name="optionsBuilder">Options builder.</param>
    /// <param name="passthrough">Whether to use passthrough mode.</param>
    /// <returns>Options builder.</returns>
    public static OptionsBuilder<CompositeResourceFactoryConfiguration> AddGoogleCloudStorageResourceFactory(
        this OptionsBuilder<CompositeResourceFactoryConfiguration> optionsBuilder,
        bool passthrough)
        => optionsBuilder.AddGoogleCloudStorageResourceFactory(passthrough, publicByDefault: false);
}