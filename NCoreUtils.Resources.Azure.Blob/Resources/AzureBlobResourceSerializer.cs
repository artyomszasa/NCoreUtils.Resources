using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace NCoreUtils.Resources;

public static class AzureBlobResourceSerializer
{
    public static Uri Serialize(AzureBlobResource resource)
        => new($"az://{resource.ContainerName}/{resource.BlobName}", UriKind.Absolute);

    public static bool TryDeserialize(Uri uri, [NotNullWhen(true)] out AzureBlobResource? resource)
    {
        if (uri?.Scheme == "az")
        {
            resource = new AzureBlobResource(uri.Host, uri.LocalPath.Trim('/'));
            return true;
        }
        resource = default;
        return false;
    }
}