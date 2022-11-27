using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using NCoreUtils.IO;

namespace NCoreUtils.Resources;

public class FileSystemResource : IReadableResource, IWritableResource, ISerializableResource
{
    public const int DefaultBufferSize = 16 * 1024;

    public string AbsolutePath { get; }

    public int? BufferSize { get; }

    public FileSystemResource(string absolutePath, int? bufferSize)
    {
        AbsolutePath = absolutePath ?? throw new ArgumentNullException(nameof(absolutePath));
        BufferSize = bufferSize;
    }

    public ValueTask<ResourceInfo> GetInfoAsync(CancellationToken cancellationToken = default) => new FileInfo(AbsolutePath) switch
    {
        { Exists: true, Length: var length } => new(new ResourceInfo(length)),
        _ => default
    };

    public IStreamProducer CreateProducer() => StreamProducer.FromStream(new FileStream(
        AbsolutePath,
        FileMode.Open,
        FileAccess.Read,
        FileShare.Read,
        BufferSize ?? DefaultBufferSize,
        true
    ), BufferSize ?? DefaultBufferSize);

    public IStreamConsumer CreateConsumer(ResourceInfo writeOptions = default)=> StreamConsumer.ToStream(new FileStream(
        AbsolutePath,
        FileMode.Create,
        FileAccess.Write,
        FileShare.ReadWrite,
        BufferSize ?? DefaultBufferSize,
        FileOptions.WriteThrough | FileOptions.Asynchronous
    ), BufferSize ?? DefaultBufferSize);

    public ValueTask<Uri> GetUriAsync(CancellationToken cancellationToken)
        => new(new Uri($"file://{AbsolutePath}", UriKind.Absolute));

    public override string ToString()
        => $"file://{AbsolutePath}";
}