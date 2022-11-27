using NCoreUtils.IO;

namespace NCoreUtils
{
    public interface IWritableResource
    {
        bool Reusable => false;

        IStreamConsumer CreateConsumer(ResourceInfo writeOptions = default);
    }
}