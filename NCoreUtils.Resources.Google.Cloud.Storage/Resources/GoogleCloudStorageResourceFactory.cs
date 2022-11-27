using System;
using System.Diagnostics.CodeAnalysis;

namespace NCoreUtils.Resources
{
    public class GoogleCloudStorageResourceFactory : IResourceFactory
    {
        public GoogleCloudStorageUtils Utils { get; }

        public bool Passthrough { get; }

        public GoogleCloudStorageResourceFactory(GoogleCloudStorageUtils utils, bool passthrough)
        {
            Utils = utils ?? throw new ArgumentNullException(nameof(utils));
            Passthrough = passthrough;
        }

        public bool TryCreateReadable(Uri uri, [NotNullWhen(true)] out IReadableResource? resource)
        {
            if (GoogleCloudStorageResourceSerializer.TryDeserialize(uri, Utils, Passthrough, out var gcsResource))
            {
                resource = gcsResource;
                return true;
            }
            resource = default;
            return false;
        }

        public bool TryCreateWritable(Uri uri, [NotNullWhen(true)] out IWritableResource? resource)
        {
            if (GoogleCloudStorageResourceSerializer.TryDeserialize(uri, Utils, Passthrough, out var gcsResource))
            {
                resource = gcsResource;
                return true;
            }
            resource = default;
            return false;
        }
    }
}