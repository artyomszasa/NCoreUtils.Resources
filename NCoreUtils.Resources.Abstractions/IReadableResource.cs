using System.Threading;
using System.Threading.Tasks;
using NCoreUtils.IO;

namespace NCoreUtils;

public interface IReadableResource
{
    bool Reusable => false;

    ValueTask<ResourceInfo> GetInfoAsync(CancellationToken cancellationToken = default);

    IStreamProducer CreateProducer();
}