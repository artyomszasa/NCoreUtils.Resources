using System;
using System.Threading;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using NCoreUtils.IO;

namespace NCoreUtils.Resources;

public class AzureBlobResource : IReadableResource, IWritableResource, ISerializableResource
{
    private BlobClient? _blobClient;

    public string ContainerName { get; }

    public string BlobName { get; }

    protected BlobClient BlobClient => _blobClient ??= new BlobClient(
        connectionString: Environment.GetEnvironmentVariable("AZURE_STORAGE_CONNECTION_STRING"),
        blobContainerName: ContainerName,
        blobName: BlobName
    );

    public bool Reusable => true;

    public AzureBlobResource(string containerName, string blobName)
    {
        ContainerName = containerName ?? throw new ArgumentNullException(nameof(containerName));
        BlobName = blobName ?? throw new ArgumentNullException(nameof(blobName));
    }

    public async ValueTask<ResourceInfo> GetInfoAsync(CancellationToken cancellationToken = default)
    {
        var props = await BlobClient.GetPropertiesAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
        return props?.Value switch
        {
            null => default,
            { ContentType: var contentType, ContentLength: var contentLength } => new(contentType, contentLength)
        };
    }

    public IStreamProducer CreateProducer() => StreamProducer.Create(async (stream, cancellationToken) =>
    {
        await using var source = await BlobClient.OpenReadAsync(new BlobOpenReadOptions(false), cancellationToken)
            .ConfigureAwait(false);
        await source.CopyToAsync(stream, 32 * 1024, cancellationToken).ConfigureAwait(false);
        await stream.FlushAsync(cancellationToken).ConfigureAwait(false);
    });

    public IStreamConsumer CreateConsumer(ResourceInfo writeOptions = default)=> StreamConsumer.Create((stream, cancellationToken) =>
    {
        var options = new BlobUploadOptions();
        options.HttpHeaders ??= new BlobHttpHeaders();
        options.HttpHeaders.ContentType = writeOptions.MediaType ?? "application/octet-stream";
        return new(BlobClient.UploadAsync(stream, options, cancellationToken));
    });

    public ValueTask<Uri> GetUriAsync(CancellationToken cancellationToken)
        => new(AzureBlobResourceSerializer.Serialize(this));

    public override string ToString()
        => $"az://{ContainerName}/{BlobName}";
}