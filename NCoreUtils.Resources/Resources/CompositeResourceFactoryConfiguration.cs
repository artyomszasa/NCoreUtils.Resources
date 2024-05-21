using System.Collections.Generic;

namespace NCoreUtils.Resources;

public class CompositeResourceFactoryConfiguration
{
    public List<IResourceFactory> Factories { get; } = [];

    public CompositeResourceFactoryConfiguration AddFactory(IResourceFactory factory)
    {
        Factories.Add(factory);
        return this;
    }
}