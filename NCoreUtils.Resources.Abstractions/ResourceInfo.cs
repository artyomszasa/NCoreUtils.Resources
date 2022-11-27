namespace NCoreUtils;

public readonly partial record struct ResourceInfo(string? MediaType, long? Length = default);

public readonly partial record struct ResourceInfo
{
    public ResourceInfo(long? length)
        : this(default, length)
    { }
}