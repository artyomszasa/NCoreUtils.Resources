using Microsoft.Extensions.Options;
using NCoreUtils.Resources;

namespace NCoreUtils
{
    public static class OptionsBuilderGoogleCloudStorageExtensions
    {
        /// <summary>
        /// Adds google cloud storage resource factory to a composite resource factory.
        /// </summary>
        /// <param name="optionsBuilder">Options builder.</param>
        /// <param name="passthrough">Whether to use passthrough mode.</param>
        /// <returns>Options builder.</returns>
        public static OptionsBuilder<CompositeResourceFactoryConfiguration> AddGoogleCloudStorageResourceFactory(
            this OptionsBuilder<CompositeResourceFactoryConfiguration> optionsBuilder,
            bool passthrough = false)
        {
            optionsBuilder.Configure<GoogleCloudStorageUtils>((config, utils) =>
            {
                config.AddFactory(new GoogleCloudStorageResourceFactory(utils, passthrough));
            });
            return optionsBuilder;
        }
    }
}