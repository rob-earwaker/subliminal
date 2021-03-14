namespace Subliminal
{
    public interface ILogEntry
    {
        Context Context { get; }
        OperationSnapshot ParentOperation { get; }
    }
}
