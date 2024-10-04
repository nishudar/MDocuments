using Serilog;

namespace Common.Seq;

public static class Seq
{
    public static Serilog.Core.Logger CreateSeqLogger(string seqServerUrl) 
        => new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console() 
            .WriteTo.Seq(seqServerUrl)
            .Enrich.FromLogContext()
            .CreateLogger();
}