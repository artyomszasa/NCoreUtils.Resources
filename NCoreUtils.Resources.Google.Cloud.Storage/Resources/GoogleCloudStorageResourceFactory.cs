using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using NCoreUtils.Google;

namespace NCoreUtils.Resources;

public class GoogleCloudStorageResourceFactory(
    GoogleCloudStorageUtils utils,
    IGoogleAccessTokenProvider? accessTokenProvider,
    ILogger<GoogleCloudStorageResourceFactory> logger,
    bool passthrough,
    bool publicByDefault)
    : IResourceFactory
{
    public GoogleCloudStorageUtils Utils { get; } = utils ?? throw new ArgumentNullException(nameof(utils));

    public IGoogleAccessTokenProvider? AccessTokenProvider { get; } = accessTokenProvider;

    public ILogger Logger { get; } = logger;

    public bool Passthrough { get; } = passthrough;

    public bool PublicByDefault { get; } = publicByDefault;

    private bool TryDeserialize(Uri uri, [MaybeNullWhen(false)] out GoogleCloudStorageResource gcsResource)
        => GoogleCloudStorageResourceSerializer.TryDeserialize(
            uri,
            Logger,
            Utils,
            AccessTokenProvider,
            Passthrough,
            PublicByDefault,
            out gcsResource
        );

    public virtual bool TryCreateReadable(Uri uri, [NotNullWhen(true)] out IReadableResource? resource)
    {
        if (TryDeserialize(uri, out var gcsResource))
        {
            resource = gcsResource;
            return true;
        }
        resource = default;
        return false;
    }

    public bool TryCreateWritable(Uri uri, [NotNullWhen(true)] out IWritableResource? resource)
    {
        if (TryDeserialize(uri, out var gcsResource))
        {
            resource = gcsResource;
            return true;
        }
        resource = default;
        return false;
    }
}