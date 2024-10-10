namespace Documents.Domain.Enums;

public static class ProcessStatus
{
    public const string Started = "started";
    public const string Finished = "finished";
    public const string Abandoned = "abandoned";
    public const string Unset = "unset";
    
    public static string[] ProcessStatuses => [Unset, Started, Finished, Abandoned];
    
}