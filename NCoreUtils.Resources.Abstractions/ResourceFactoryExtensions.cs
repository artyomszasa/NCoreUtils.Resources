using System;

namespace NCoreUtils;

public static class ResourceFactoryExtensions
{
    public static IReadableResource CreateReadable(this IResourceFactory factory, Uri uri)
    {
        if (factory is null)
        {
            throw new ArgumentNullException(nameof(factory));
        }
        return factory.TryCreateReadable(uri, out var resource)
            ? resource
            : throw new NotSupportedException($"Unable to create readable resource from \"{uri}\".");
    }

    public static IWritableResource CreateWritable(this IResourceFactory factory, Uri uri)
    {
        if (factory is null)
        {
            throw new ArgumentNullException(nameof(factory));
        }
        return factory.TryCreateWritable(uri, out var resource)
            ? resource
            : throw new NotSupportedException($"Unable to create writable resource from \"{uri}\".");
    }
}