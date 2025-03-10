using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NCoreUtils.Google;
using NCoreUtils.IO;

namespace NCoreUtils.Resources;

public class GoogleCloudStorageResource
    : IReadableResource
    , IWritableResource
    , ISerializableResource
{
    private static string[] RoScopes { get; } = [GoogleCloudStorageUtils.ReadOnlyScope];

    private static string[] RwScopes { get; } = [GoogleCloudStorageUtils.ReadWriteScope];

    internal GoogleCloudStorageUtils Utils { get; }

    public bool Reusable => true;

    public string BucketName { get; }

    public string ObjectName { get; }

    public string? ContentType { get; }

    public string? CacheControl { get; }

    public bool IsPublic { get; }

    public ILogger? Logger { get; }

    public GoogleCloudStorageCredential Credential { get; }

    private GoogleCloudStorageResource(
        GoogleCloudStorageUtils utils,
        GoogleCloudStorageCredential credential,
        string bucketName,
        string objectName,
        string? contentType = default,
        string? cacheControl = default,
        bool isPublic = true,
        ILogger? logger = default)
    {
        Utils = utils ?? throw new ArgumentNullException(nameof(utils));
        // Passthrough = passthrough;
        BucketName = bucketName;
        ObjectName = objectName;
        ContentType = contentType;
        CacheControl = cacheControl;
        IsPublic = isPublic;
        Credential = credential;
        Logger = logger;
    }

    public GoogleCloudStorageResource(
        GoogleCloudStorageUtils utils,
        string accessToken,
        string bucketName,
        string objectName,
        string? contentType = default,
        string? cacheControl = default,
        bool isPublic = true,
        ILogger? logger = default)
        : this(utils, GoogleCloudStorageCredential.ViaAccessToken(accessToken), bucketName, objectName, contentType, cacheControl, isPublic, logger)
    { }

    public GoogleCloudStorageResource(
        GoogleCloudStorageUtils utils,
        IGoogleAccessTokenProvider accessTokenProvider,
        string bucketName,
        string objectName,
        string? contentType = default,
        string? cacheControl = default,
        bool isPublic = true,
        ILogger? logger = default)
        : this(utils, GoogleCloudStorageCredential.ViaProvider(accessTokenProvider), bucketName, objectName, contentType, cacheControl, isPublic, logger)
    { }

    private ValueTask<string> GetAccessTokenAsync(ScopeCollection scopes, CancellationToken cancellationToken)
        => Credential.GetAccessTokenAsync(scopes, cancellationToken);

    public IStreamProducer CreateProducer()
        => StreamProducer.Create(async (stream, cancellationToken) =>
        {
            var accessToken = await GetAccessTokenAsync(RoScopes, cancellationToken).ConfigureAwait(false);
            await Utils.DownloadAsync(BucketName, ObjectName, stream, accessToken, cancellationToken)
                .ConfigureAwait(false);
        });

    public async ValueTask<ResourceInfo> GetInfoAsync(CancellationToken cancellationToken = default)
    {
        var accessToken = await GetAccessTokenAsync(RoScopes, cancellationToken).ConfigureAwait(false);
        var data = await Utils.GetAsync(BucketName, ObjectName, accessToken, cancellationToken)
            .ConfigureAwait(false);
        if (data is null)
        {
            return default;
        }
        return new ResourceInfo(data.ContentType, data.Size.HasValue ? (long?)(long)data.Size.Value : default);
    }

    public IStreamConsumer CreateConsumer(ResourceInfo writeOptions = default)
        => StreamConsumer.Create(async (stream, cancellationToken) =>
        {
            var accessToken = await GetAccessTokenAsync(RwScopes, cancellationToken).ConfigureAwait(false);
            var contentType = writeOptions.MediaType.Supply(ContentType).Supply("application/octet-stream");
            await Utils.UploadAsync(
                bucket: BucketName,
                name: ObjectName,
                source: stream,
                contentType: contentType,
                cacheControl: CacheControl,
                isPublic: IsPublic,
                accessToken: accessToken,
                progress: default,
                buffer: default,
                cancellationToken: cancellationToken
            ).ConfigureAwait(false);
        });

    public ValueTask<Uri> GetUriAsync(CancellationToken cancellationToken)
        => GoogleCloudStorageResourceSerializer.SerializeAsync(Logger, this, cancellationToken);

    public override string ToString()
        => IsPublic
            ? $"gs://{BucketName}/{ObjectName} [public, {Credential}]"
            : $"gs://{BucketName}/{ObjectName} [{Credential}]";
}