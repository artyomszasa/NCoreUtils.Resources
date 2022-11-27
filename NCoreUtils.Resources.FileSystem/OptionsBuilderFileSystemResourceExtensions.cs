using Microsoft.Extensions.Options;
using NCoreUtils.Resources;

namespace NCoreUtils;

public static class OptionsBuilderFileSystemResourceExtensions
{
    /// <summary>
    /// Adds file system storage resource factory to a composite resource factory.
    /// </summary>
    /// <param name="optionsBuilder">Options builder.</param>
    /// <returns>Options builder.</returns>
    public static OptionsBuilder<CompositeResourceFactoryConfiguration> AddFileSystemResourceFactory(
        this OptionsBuilder<CompositeResourceFactoryConfiguration> optionsBuilder)
    {
        optionsBuilder.Configure(static config =>
        {
            config.AddFactory(new FileSystemResourceFactory());
        });
        return optionsBuilder;
    }
}