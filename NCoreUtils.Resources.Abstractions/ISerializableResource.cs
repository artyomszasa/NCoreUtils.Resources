using System;
using System.Threading;
using System.Threading.Tasks;

namespace NCoreUtils;

public interface ISerializableResource
{
    ValueTask<Uri> GetUriAsync(CancellationToken cancellationToken);
}