using System;
using System.IO;
using GitHub.Extensions;
using Serilog;
using Serilog.Core;

namespace GitHub.Infrastructure
{
    public static class LogManager
    {
        static Logger CreateLogger()
        {
            var environment = new Rothko.Environment();
            var logPath = Path.Combine(environment.GetLocalGitHubApplicationDataPath(), "extension.log");

            const string outputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff}|{Level}|Thread:{ThreadId}|{SourceContext}|{Message}{NewLine}{Exception}";

            //2MBs
            const long fileSizeLimitBytes = 2L*1024L*1024L;

            return new LoggerConfiguration()
                .Enrich.WithThreadId()
                .WriteTo.File(logPath,
                    fileSizeLimitBytes: fileSizeLimitBytes,
                    outputTemplate: outputTemplate)
                .CreateLogger();
        }

        static Lazy<Logger> Logger { get; } = new Lazy<Logger>(CreateLogger);

        //Violates CA1004 - Generic methods should provide type parameter
        #pragma warning disable CA1004
        public static ILogger ForContext<T>() => Logger.Value.ForContext<T>();
        #pragma warning restore CA1004

        public static ILogger ForContext(Type type) => Logger.Value.ForContext(type);
    }
}