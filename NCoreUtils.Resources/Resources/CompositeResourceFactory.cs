using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Options;

namespace NCoreUtils.Resources
{
    public class CompositeResourceFactory : IResourceFactory
    {
        private readonly IOptionsMonitor<CompositeResourceFactoryConfiguration> _configuration;

        public CompositeResourceFactory(IOptionsMonitor<CompositeResourceFactoryConfiguration> configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public bool TryCreateReadable(Uri uri, [NotNullWhen(true)] out IReadableResource? resource)
        {
            foreach (var factory in _configuration.CurrentValue.Factories)
            {
                if (factory.TryCreateReadable(uri, out var r))
                {
                    resource = r;
                    return true;
                }
            }
            resource = default;
            return false;
        }

        public bool TryCreateWritable(Uri uri, [NotNullWhen(true)] out IWritableResource? resource)
        {
            foreach (var factory in _configuration.CurrentValue.Factories)
            {
                if (factory.TryCreateWritable(uri, out var r))
                {
                    resource = r;
                    return true;
                }
            }
            resource = default;
            return false;
        }
    }
}