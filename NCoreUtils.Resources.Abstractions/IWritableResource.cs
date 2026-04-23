using NCoreUtils.IO;

namespace NCoreUtils;

public interface IWritableResource
{
#if NETFRAMEWORK
    bool Reusable { get; }
#else
    bool Reusable => false;
#endif


    IStreamConsumer CreateConsumer(ResourceInfo writeOptions = default);
}