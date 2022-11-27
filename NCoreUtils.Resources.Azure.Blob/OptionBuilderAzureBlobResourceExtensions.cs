using Microsoft.Extensions.Options;
using NCoreUtils.Resources;

namespace NCoreUtils;

public static class OptionBuilderAzureBlobResourceExtensions
{
    /// <summary>
    /// Adds azure blob storage resource factory to a composite resource factory.
    /// </summary>
    /// <param name="optionsBuilder">Options builder.</param>
    /// <returns>Options builder.</returns>
    public static OptionsBuilder<CompositeResourceFactoryConfiguration> AddAzureBlobResourceFactory(
        this OptionsBuilder<CompositeResourceFactoryConfiguration> optionsBuilder)
    {
        optionsBuilder.Configure(static config =>
        {
            config.AddFactory(new AzureBlobResourceFactory());
        });
        return optionsBuilder;
    }
}