using System;
using Microsoft.AspNetCore.Http;
using NCoreUtils.IO;

namespace NCoreUtils.Resources;

public class AspNetCoreResponseResource(HttpResponse response) : IWritableResource
{
    public HttpResponse Response { get; } = response ?? throw new ArgumentNullException(nameof(response));

    public bool Reusable => false;

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

    public override string ToString()
        => "Response";
}