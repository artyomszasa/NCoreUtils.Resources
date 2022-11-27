using System;
using System.Diagnostics.CodeAnalysis;

namespace NCoreUtils
{
    public interface IResourceFactory
    {
        bool TryCreateReadable(Uri uri, [MaybeNullWhen(false)] out IReadableResource resource);

        bool TryCreateWritable(Uri uri, [MaybeNullWhen(false)] out IWritableResource resource);
    }
}