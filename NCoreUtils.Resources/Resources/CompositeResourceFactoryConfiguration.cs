using System.Collections.Generic;

namespace NCoreUtils.Resources
{
    public class CompositeResourceFactoryConfiguration
    {
        public List<IResourceFactory> Factories { get; } = new List<IResourceFactory>();

        public CompositeResourceFactoryConfiguration AddFactory(IResourceFactory factory)
        {
            Factories.Add(factory);
            return this;
        }
    }
}