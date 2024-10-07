namespace Documents.Domain.Enums;

public static class ProcessStatus
{
    public const string Started = "started";
    public const string Finished = "finished";
    public const string Abandoned = "abandoned";
    
    public static string[] ProcessStatuses => [Started, Finished, Abandoned];
}