using System;
using System.Diagnostics.CodeAnalysis;

namespace NCoreUtils.Resources;

public class FileSystemResourceFactory : IResourceFactory
{
    public bool TryCreateReadable(Uri uri, [MaybeNullWhen(false)] out IReadableResource resource)
    {
        if (uri is not null && uri.Scheme == "file")
        {
            // FIXME: handle buffer size query argument
            resource = new FileSystemResource(uri.AbsolutePath, default);
            return true;
        }
        resource = default;
        return false;
    }

    public bool TryCreateWritable(Uri uri, [MaybeNullWhen(false)] out IWritableResource resource)
    {
        if (uri is not null && uri.Scheme == "file")
        {
            // FIXME: handle buffer size query argument
            resource = new FileSystemResource(uri.AbsolutePath, default);
            return true;
        }
        resource = default;
        return false;
    }
}