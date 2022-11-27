using System;
using System.Threading;
using System.Threading.Tasks;
using NCoreUtils.IO;

namespace NCoreUtils.Resources
{
    public class GoogleCloudStorageResource : IReadableResource, IWritableResource, ISerializableResource
    {
        private static string[] RoScopes { get; } = new[] { GoogleCloudStorageUtils.ReadOnlyScope };

        private static string[] RwScopes { get; } = new[] { GoogleCloudStorageUtils.ReadWriteScope };

        internal GoogleCloudStorageUtils Utils { get; }

        internal bool Passthrough { get; }

        public bool Reusable => true;

        public string BucketName { get; }

        public string ObjectName { get; }

        public string? ContentType { get; }

        public string? CacheControl { get; }

        public bool IsPublic { get; }

        public GoogleStorageCredential Credential { get; }

        public GoogleCloudStorageResource(
            GoogleCloudStorageUtils utils,
            string bucketName,
            string objectName,
            string? contentType = default,
            string? cacheControl = default,
            bool isPublic = true,
            GoogleStorageCredential credential = default,
            bool passthrough = false)
        {
            Utils = utils ?? throw new ArgumentNullException(nameof(utils));
            BucketName = bucketName;
            ObjectName = objectName;
            ContentType = contentType;
            CacheControl = cacheControl;
            IsPublic = isPublic;
            Credential = credential;
            Passthrough = passthrough;
        }

        private ValueTask<string> GetAccessTokenAsync(string[] scopes, CancellationToken cancellationToken)
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
            => GoogleCloudStorageResourceSerializer.SerializeAsync(this, cancellationToken);
    }
}