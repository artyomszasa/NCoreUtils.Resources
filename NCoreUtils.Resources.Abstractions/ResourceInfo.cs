namespace NCoreUtils;

public readonly record struct ResourceInfo(string? MediaType, long? Length = default)
{
    public ResourceInfo(long? length) : this(default, length) { }
}