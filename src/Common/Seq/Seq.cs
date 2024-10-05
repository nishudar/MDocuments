using Serilog;
using Serilog.Core;

namespace Common.Seq;

public static class Seq
{
    public static Logger CreateSeqLogger(string seqServerUrl)
    {
        return new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console()
            .WriteTo.Seq(seqServerUrl)
            .Enrich.FromLogContext()
            .CreateLogger();
    }
}