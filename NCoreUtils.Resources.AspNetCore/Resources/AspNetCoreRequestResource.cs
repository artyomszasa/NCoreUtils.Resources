using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using NCoreUtils.IO;

namespace NCoreUtils.Resources;

public class AspNetCoreRequestResource(HttpRequest request) : IReadableResource
{
    public HttpRequest Request { get; } = request ?? throw new ArgumentNullException(nameof(request));

    public bool Reusable => false;

    public ValueTask<ResourceInfo> GetInfoAsync(CancellationToken cancellationToken = default)
        => new(new ResourceInfo(
            Request.ContentType,
            Request.ContentLength
        ));

    public IStreamProducer CreateProducer()
        => StreamProducer.FromStream(Request.Body, leaveOpen: true);

    public override string ToString()
        => $"Request[Content-Type = {Request.ContentType}, Content-Length = {Request.ContentLength}]";
}