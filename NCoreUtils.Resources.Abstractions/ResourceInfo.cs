namespace NCoreUtils;

public readonly partial record struct ResourceInfo(string? MediaType, long? Length = default)
{
    public ResourceInfo(long? length) : this(default, length) { }
}