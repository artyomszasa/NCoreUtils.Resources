using System;

namespace NCoreUtils;

public static class ResourceFactoryExtensions
{
    public static IReadableResource CreateReadable(this IResourceFactory factory, Uri uri)
    {
        return factory.ThrowIfNull().TryCreateReadable(uri, out var resource)
            ? resource
            : throw new NotSupportedException($"Unable to create readable resource from \"{uri}\".");
    }

    public static IWritableResource CreateWritable(this IResourceFactory factory, Uri uri)
    {
        return factory.ThrowIfNull().TryCreateWritable(uri, out var resource)
            ? resource
            : throw new NotSupportedException($"Unable to create writable resource from \"{uri}\".");
    }
}