using Microsoft.Extensions.DependencyInjection;
using NCoreUtils.Resources;

namespace NCoreUtils;

public static class ServiceCollectionFileSystemResourceExtensions
{
    /// <summary>
    /// Adds file system storage resource factory as STANDALONE implementation of
    /// <see cref="IResourceFactory" />.
    /// </summary>
    /// <param name="services">Service collection.</param>
    /// <returns>Service collection.</returns>
    public static IServiceCollection AddStandaloneFileSystemResourceFactory(this IServiceCollection services)
        => services.AddSingleton<IResourceFactory, FileSystemResourceFactory>();
}