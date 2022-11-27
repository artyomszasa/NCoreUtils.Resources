using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Http;

namespace NCoreUtils.Resources;

public class AspNetCoreResourceFactory : IResourceFactory
{
    public IHttpContextAccessor HttpContextAccessor { get; }

    public AspNetCoreResourceFactory(IHttpContextAccessor httpContextAccessor)
        => HttpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));

    public bool TryCreateReadable(Uri uri, [NotNullWhen(true)] out IReadableResource? resource)
    {
        if ((uri is null || uri.AbsoluteUri == "file:///dev/stdin") && HttpContextAccessor.HttpContext is not null)
        {
            resource = new AspNetCoreRequestResource(HttpContextAccessor.HttpContext.Request);
            return true;
        }
        resource = default;
        return false;
    }

    public bool TryCreateWritable(Uri uri, [NotNullWhen(true)] out IWritableResource? resource)
    {
        if ((uri is null || uri.AbsoluteUri == "file:///dev/stdout") && HttpContextAccessor.HttpContext is not null)
        {
            resource = new AspNetCoreResponseResource(HttpContextAccessor.HttpContext.Response);
            return true;
        }
        resource = default;
        return false;
    }
}