using System;
using Microsoft.AspNetCore.Http;
using NCoreUtils.IO;

namespace NCoreUtils.Resources;

public class AspNetCoreResponseResource : IWritableResource
{
    public HttpResponse Response { get; }

    public bool Reusable => false;

    public AspNetCoreResponseResource(HttpResponse response)
    {
        Response = response ?? throw new ArgumentNullException(nameof(response));
    }

    public IStreamConsumer CreateConsumer(ResourceInfo writeOptions = default)
        => StreamConsumer.Delay(_ =>
        {
            if (!string.IsNullOrEmpty(writeOptions.MediaType))
            {
                Response.ContentType = writeOptions.MediaType;
            }
            if (writeOptions.Length.HasValue)
            {
                Response.ContentLength = writeOptions.Length.Value;
            }
            return new System.Threading.Tasks.ValueTask<IStreamConsumer>(StreamConsumer.ToStream(Response.Body));
        });
}