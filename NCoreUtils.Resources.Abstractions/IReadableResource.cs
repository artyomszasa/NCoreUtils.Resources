using System.Threading;
using System.Threading.Tasks;
using NCoreUtils.IO;

namespace NCoreUtils;

public interface IReadableResource
{
#if NETFRAMEWORK
    bool Reusable { get; }
#else
    bool Reusable => false;
#endif

    ValueTask<ResourceInfo> GetInfoAsync(CancellationToken cancellationToken = default);

    IStreamProducer CreateProducer();
}