using Microsoft.Extensions.DependencyInjection;
using NCoreUtils.Resources;

namespace NCoreUtils
{
    public static class ServiceCollectionGoogleCloudStorageResourceExtensions
    {
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
        {
            return services.AddSingleton<IResourceFactory>(serviceProvider =>
            {
                var utils = serviceProvider.GetRequiredService<GoogleCloudStorageUtils>();
                return new GoogleCloudStorageResourceFactory(utils, passthrough);
            });
        }
    }
}