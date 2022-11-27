using System;
using System.Diagnostics.CodeAnalysis;

namespace NCoreUtils.Resources;

public class AzureBlobResourceFactory : IResourceFactory
{
    public bool TryCreateReadable(Uri uri, [MaybeNullWhen(false)] out IReadableResource resource)
    {
        if (uri is not null && AzureBlobResourceSerializer.TryDeserialize(uri, out var azureBlobResource))
        {
            resource = azureBlobResource;
            return true;
        }
        resource = default;
        return false;
    }

    public bool TryCreateWritable(Uri uri, [MaybeNullWhen(false)] out IWritableResource resource)
    {
        if (uri is not null && AzureBlobResourceSerializer.TryDeserialize(uri, out var azureBlobResource))
        {
            resource = azureBlobResource;
            return true;
        }
        resource = default;
        return false;
    }
}