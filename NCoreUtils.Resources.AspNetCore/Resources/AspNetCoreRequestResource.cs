using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using NCoreUtils.IO;

namespace NCoreUtils.Resources;

public class AspNetCoreRequestResource : IReadableResource
{
    public HttpRequest Request { get; }

    public bool Reusable => false;

    public AspNetCoreRequestResource(HttpRequest request)
        => Request = request ?? throw new ArgumentNullException(nameof(request));

    public ValueTask<ResourceInfo> GetInfoAsync(CancellationToken cancellationToken = default)
        => new(new ResourceInfo(
            Request.ContentType,
            Request.ContentLength
        ));

    public IStreamProducer CreateProducer()
        => StreamProducer.FromStream(Request.Body, leaveOpen: true);
}